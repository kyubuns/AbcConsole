namespace AbcConsole
{
    public static class EmbeddedDebugMethods
    {
        [AbcCommand("Clear Logs")]
        public static void Clear()
        {
            Instance.ClearLogs();
        }

        [AbcCommand]
        public static void FpsCounter()
        {
            Instance.FpcCounterEnabled = !Instance.FpcCounterEnabled;
        }

        [AbcCommand]
        public static void CallErrorCallback()
        {
            UnityEngine.Debug.LogError("DebugMethod CallErrorCallback");
        }
    }
}