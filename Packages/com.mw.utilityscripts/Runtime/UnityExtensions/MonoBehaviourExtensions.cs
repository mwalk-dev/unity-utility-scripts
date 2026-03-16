using System;
using System.Linq.Expressions;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Runtime
{
    public static class MonoBehaviourExtensions
    {
        public static bool TryGetComponentInParent<T>(this MonoBehaviour self, out T component)
        {
            component = self.GetComponentInParent<T>();
            return component != null;
        }

        /// <summary>
        /// If the MonoBehaviour member returned from the expression is null, this method will look at the root
        /// GameObject in the hierarchy and attempt to find a component of the correct type. If one is found, it
        /// will be assigned to the member.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TMember"></typeparam>
        /// <param name="mb"></param>
        /// <param name="lambda"></param>
        /// <param name="throwIfNotFound"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void AssignComponent<TSource, TMember>(
            this TSource mb,
            Expression<Func<TSource, TMember>> lambda,
            bool throwIfNotFound = false
        )
            where TSource : MonoBehaviour
            where TMember : Component
        {
#if UNITY_EDITOR
            if (!mb.CanModifyAsset())
            {
                return;
            }
            var memberInfo = GetMemberInfo(lambda);
            var (name, currentValue, type, setter) = memberInfo switch
            {
                FieldInfo fieldInfo
                    => (
                        fieldInfo.Name,
                        fieldInfo.GetValue(mb),
                        fieldInfo.FieldType,
                        (Action<Component>)(cmp => fieldInfo.SetValue(mb, cmp))
                    ),
                PropertyInfo propertyInfo
                    => (
                        propertyInfo.Name,
                        propertyInfo.GetValue(mb),
                        propertyInfo.PropertyType,
                        cmp => propertyInfo.SetValue(mb, cmp)
                    ),
                _ => throw new ArgumentException($"Expression '{lambda}' did not return a field or property"),
            };
            if (currentValue == null)
            {
                var components = mb.transform.root.GetComponentsInChildren(type);
                if (components.Length == 1)
                {
                    // Unity doesn't allow saving prefabs from inside OnValidate, which is often when we want to use
                    // this function. So we defer execution of the save to make it permissible.
                    EditorApplication.delayCall += () =>
                    {
                        // Since we delayed after finding the component, something might have happened
                        if (components[0] == null)
                            return;
                        setter(components[0]);
                        Debug.Log($"{nameof(AssignComponent)} successfully assigned {components[0].name} to {mb.name}");
                        Undo.RecordObject(mb, "TryAssign");
                        EditorUtility.SetDirty(mb);
                        AssetDatabase.SaveAssetIfDirty(mb);
                    };
                }
                else if (components.Length > 1)
                {
                    Debug.LogError(
                        $"{mb.name} has {components.Length} components of type {name} under it and {nameof(AssignComponent)} expects at most one, so no reference was automatically assigned."
                    );
                }
                else if (throwIfNotFound)
                {
                    Debug.LogError(
                        $"{mb.name} has no components of type {name} under it and {nameof(AssignComponent)} expected exactly one!"
                    );
                }
            }
#endif
        }

        private static MemberInfo GetMemberInfo<TSource, TProperty>(Expression<Func<TSource, TProperty>> memberLambda)
        {
            if (memberLambda.Body is not MemberExpression member)
            {
                throw new ArgumentException($"Expression '{memberLambda}' refers to a method, not a property.");
            }

            Type type = typeof(TSource);
            if (
                member.Member.ReflectedType != null
                && type != member.Member.ReflectedType
                && !type.IsSubclassOf(member.Member.ReflectedType)
            )
            {
                throw new ArgumentException(
                    $"Expression '{memberLambda}' refers to a property that is not from type {type}."
                );
            }
            return member.Member;
        }
    }
}
