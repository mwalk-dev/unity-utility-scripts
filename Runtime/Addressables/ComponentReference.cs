#if HAS_ADDRESSABLES
#if HAS_UNITASK
using Cysharp.Threading.Tasks;
#endif
using UnityEditor;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

// Slightly modified from https://github.com/Unity-Technologies/Addressables-Sample/blob/master/Basic/ComponentReference/Assets/Samples/Addressables/1.19.19/ComponentReference/ComponentReference.cs
namespace MWUtilityScripts.Addressables
{
    /// <summary>
    /// Creates an AssetReference that is restricted to having a specific Component.
    /// * This is the class that inherits from AssetReference.  It is generic and does not specify which Components it might care about.  A concrete child of this class is required for serialization to work.* At edit-time it validates that the asset set on it is a GameObject with the required Component.
    /// * At edit-time it validates that the asset set on it is a GameObject with the required Component.
    /// * At runtime it can load/instantiate the GameObject, then return the desired component.  API matches base class (LoadAssetAsync & InstantiateAsync).
    /// </summary>
    /// <typeparam name="TComponent"> The component type.</typeparam>
    public class ComponentReference<TComponent> : AssetReferenceGameObject, IComponentReference<TComponent>
        where TComponent : MonoBehaviour
    {
        public TComponent Component => GetAndCacheComponent();

        private TComponent _cachedComponent;

        public ComponentReference(string guid)
            : base(guid) { }

        public new AsyncOperationHandle<TComponent> InstantiateAsync(
            Vector3 position,
            Quaternion rotation,
            Transform parent = null
        )
        {
            return UnityEngine.AddressableAssets.Addressables.ResourceManager.CreateChainOperation(
                base.InstantiateAsync(position, Quaternion.identity, parent),
                GameObjectReady
            );
        }

        public new AsyncOperationHandle<TComponent> InstantiateAsync(
            Transform parent = null,
            bool instantiateInWorldSpace = false
        )
        {
            return UnityEngine.AddressableAssets.Addressables.ResourceManager.CreateChainOperation(
                base.InstantiateAsync(parent, instantiateInWorldSpace),
                GameObjectReady
            );
        }

        public new AsyncOperationHandle<TComponent> LoadAssetAsync()
        {
            return UnityEngine.AddressableAssets.Addressables.ResourceManager.CreateChainOperation(LoadAssetAsync<GameObject>(), GameObjectReady);
        }

        AsyncOperationHandle<TComponent> GameObjectReady(AsyncOperationHandle<GameObject> arg)
        {
            var comp = arg.Result.GetComponent<TComponent>();
            return UnityEngine.AddressableAssets.Addressables.ResourceManager.CreateCompletedOperation(comp, string.Empty);
        }

        public override bool ValidateAsset(Object obj)
        {
            var go = obj as GameObject;
            return go != null && go.GetComponent<TComponent>() != null;
        }

        public override bool ValidateAsset(string path)
        {
#if UNITY_EDITOR
            //this load can be expensive...
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            return go != null && go.GetComponent<TComponent>() != null;
#else
            return false;
#endif
        }
        
#if HAS_UNITASK
        public new UniTask<TComponent> LoadAssetAsyncTask()
        {
            return LoadAssetAsync().ToUniTask();
        }
#endif

        public void ReleaseInstance(AsyncOperationHandle<TComponent> op)
        {
            // Release the instance
            var component = op.Result as Component;
            if (component != null)
            {
                UnityEngine.AddressableAssets.Addressables.ReleaseInstance(component.gameObject);
            }

            // Release the handle
            UnityEngine.AddressableAssets.Addressables.Release(op);
        }

        private TComponent GetAndCacheComponent()
        {
            if (!IsValid())
            {
                return null;
            }
            if (_cachedComponent == null)
            {
                _cachedComponent = (Asset as GameObject).GetComponent<TComponent>();
            }
            return _cachedComponent;
        }
    }
}
#endif