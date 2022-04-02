using AbcConsole.Internal;

namespace AbcConsole
{
    public static class EmbeddedDebugMethods
    {
        [AbcCommand("Clear Logs")]
        public static void Clear()
        {
            Root.CurrentInstance.ClearLogs();
        }

        [AbcCommand]
        public static void FpsCounter()
        {
            Root.CurrentInstance.FpcCounterEnabled = !Root.CurrentInstance.FpcCounterEnabled;
        }

        [AbcCommand]
        public static void CallErrorCallback()
        {
            UnityEngine.Debug.LogError("DebugMethod CallErrorCallback");
        }
    }
}