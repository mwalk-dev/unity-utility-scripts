
#if HAS_UNITASK
using Cysharp.Threading.Tasks;
#endif
using UnityEngine;

namespace MWUtilityScripts.Addressables
{
    // This only exists so we can apply the IAssetReference interface
    public class AssetReferenceGameObject
        : UnityEngine.AddressableAssets.AssetReferenceGameObject,
            IAssetReference<GameObject>
    {
        // ***** NOTE *****
        // The below SHOULD work in theory for making sure we can't accidentally modify assets while running in-editor
        // However, as of Unity 2023.2.20f1 it immediately crashes the editor
        // My theory is that something inside Addressables.ResourceManager gets very unhappy if we try to create a
        // completed operation with something that isn't actually an asset.

        // So instead we're forced to add PreventAssetUpdates() to all of our code paths within AddressablePrefabLoader
        // instead :(

        //public override AsyncOperationHandle<GameObject> LoadAssetAsync()
        //{
        //    return Addressables.ResourceManager.CreateChainOperation(LoadAssetAsync<GameObject>(), GameObjectReady);
        //}

        // This is necessary because for whatever stupid reason, AssetReferenceT doesn't constrain TObject to
        // UnityEngine.Object subtypes
        //public new AsyncOperationHandle<TObject> LoadAssetAsync<TObject>()
        //    where TObject : Object
        //{
        //    return Addressables.ResourceManager.CreateChainOperation(LoadAssetAsync<TObject>(), GameObjectReady);
        //}

        //AsyncOperationHandle<TObject> GameObjectReady<TObject>(AsyncOperationHandle<TObject> arg)
        //    where TObject : Object
        //{
        //    return Addressables.ResourceManager.CreateCompletedOperation(
        //        arg.Result.PreventAssetUpdates(),
        //        string.Empty
        //    );
        //}

        public AssetReferenceGameObject(string guid)
            : base(guid) { }

#if HAS_UNITASK
        public UniTask<GameObject> LoadAssetAsyncTask()
        {
            return LoadAssetAsync().ToUniTask();
        }
#endif
    }
}
