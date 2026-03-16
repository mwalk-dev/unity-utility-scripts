using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MWUtilityScripts.Components
{
#pragma warning disable IDE1006 // Naming Styles
    /// <summary>
    /// An interface with commonly used functionality from MonoBehaviour. It makes working with interfaces a little nicer
    /// when we know that all implementations will extend MonoBehaviour.
    /// </summary>
    public interface IMonoBehaviour
    {
        // Object
        string name { get; }

        // Component
        GameObject gameObject { get; }
        Transform transform { get; }
        T GetComponent<T>();
        T GetComponentInChildren<T>(bool includeInactive = false);
        T GetComponentInChildren<T>();
        T GetComponentInParent<T>();
        T[] GetComponents<T>();
        void GetComponents<T>(List<T> results);
        void GetComponentsInChildren<T>(List<T> results);
        T[] GetComponentsInChildren<T>(bool includeInactive);
        void GetComponentsInChildren<T>(bool includeInactive, List<T> result);
        T[] GetComponentsInChildren<T>();
        T[] GetComponentsInParent<T>();
        T[] GetComponentsInParent<T>(bool includeInactive);
        void GetComponentsInParent<T>(bool includeInactive, List<T> results);

        //bool TryGetComponent(Type type, out Component component);
        bool TryGetComponent<T>(out T component);

        // Behaviour
        bool enabled { get; set; }
        bool isActiveAndEnabled { get; }

        // MonoBehaviour
        Coroutine StartCoroutine(IEnumerator routine);
        void StopAllCoroutines();
        void StopCoroutine(IEnumerator routine);
        void StopCoroutine(Coroutine routine);
    }
#pragma warning restore IDE1006 // Naming Styles
}
