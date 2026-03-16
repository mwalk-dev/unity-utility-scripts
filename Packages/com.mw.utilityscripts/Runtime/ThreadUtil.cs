#if UNITY_EDITOR
using System.Threading;
using UnityEditor;

namespace MWUtilityScripts
{
    [InitializeOnLoad]
    public static class ThreadUtil
    {
        private static readonly Thread MainThread;

        static ThreadUtil()
        {
            MainThread = Thread.CurrentThread;
        }

        public static bool IsMainThread(this Thread thread)
        {
            return thread == MainThread;
        }

        public static bool IsMainThreadCurrent()
        {
            return Thread.CurrentThread == MainThread;
        }
    }
}
#endif
