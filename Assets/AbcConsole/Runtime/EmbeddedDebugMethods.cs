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
    }
}