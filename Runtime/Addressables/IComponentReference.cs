#if HAS_ADDRESSABLES
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
#if HAS_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace MWUtilityScripts.Addressables
{
    public interface IComponentReference<TComponent> : IAssetReference<GameObject>
        where TComponent : MonoBehaviour
    {
        public TComponent Component { get; }

        public AsyncOperationHandle<TComponent> InstantiateAsync(
            Vector3 position,
            Quaternion rotation,
            Transform parent = null
        );
    
        public bool ValidateAsset(Object obj);
        public void ReleaseInstance(AsyncOperationHandle<TComponent> op);
        
#if HAS_UNITASK
        UniTask<TComponent> LoadAssetAsyncTask();
#endif
    }
}
#endif