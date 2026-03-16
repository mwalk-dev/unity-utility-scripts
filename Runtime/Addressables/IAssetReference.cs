#if HAS_ADDRESSABLES
#if HAS_UNITASK
using Cysharp.Threading.Tasks;
#endif
using UnityEngine;

namespace MWUtilityScripts.Addressables
{
    public interface IAssetReference
    {
        public string AssetGUID { get; }
        public bool IsValid();
        public bool IsDone { get; }
        public Object Asset { get; }
        public object RuntimeKey { get; }

#if UNITY_EDITOR
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Unity")]
        public Object editorAsset { get; }
#endif

        public void ReleaseAsset();
    }

    public interface IAssetReference<T> : IAssetReference
        where T : Object
    {
#if HAS_UNITASK
        UniTask<T> LoadAssetAsyncTask();
#endif
    }
}
#endif