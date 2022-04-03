using System;
using System.Collections.Generic;
using AbcConsole.Internal;

namespace AbcConsole
{
    public static class Instance
    {
        public static ConsoleState State => Root.CurrentInstance.State;

        public static IReadOnlyList<Log> Logs => Root.CurrentInstance.Logs;

        public static Action<IReadOnlyList<Log>> ErrorCallback
        {
            get => Root.CurrentInstance.ErrorCallback;
            set => Root.CurrentInstance.ErrorCallback = value;
        }

        public static bool AutoOpenWhenError
        {
            get => Root.CurrentInstance.AutoOpenWhenError;
            set => Root.CurrentInstance.AutoOpenWhenError = value;
        }

        public static bool FpcCounterEnabled
        {
            get => Root.CurrentInstance.FpcCounterEnabled;
            set => Root.CurrentInstance.FpcCounterEnabled = value;
        }

        public static void ClearLogs() => Root.CurrentInstance.ClearLogs();
    }
}
