namespace MWUtilityScripts
{
    /// <summary>
    /// Interface for ScriptableObjects to work around stupid Unity logic that results in ScriptableObjects not having
    /// OnEnable/OnDisable called when starting/exiting play mode, the way that those methods are called in a built
    /// player.
    /// </summary>
    public interface IManagedObject
    {
        void OnBegin();
        void OnEnd();
    }
}
