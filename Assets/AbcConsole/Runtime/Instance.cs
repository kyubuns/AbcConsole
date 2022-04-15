using System;
using System.Collections.Generic;
using AbcConsole.Internal;
using UnityEngine;

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

        public static void Trigger() => Root.CurrentInstance.OnClickTriggerButton();
        public static void ShowFullMode() => Root.CurrentInstance.ShowFullMode();
        public static void ShowMiniMode() => Root.CurrentInstance.ShowMiniMode();
        public static void Hide() => Root.CurrentInstance.Hide();

        public static GameObject GetRootObject() => Root.CurrentInstance.gameObject;
    }

    public enum ConsoleState
    {
        None,
        Full,
        Mini,
    }

    public class Log
    {
        public int Id { get; }
        public string Condition { get; }
        public string StackTrace { get; }
        public LogType Type { get; }
        public DateTime DateTime { get; }

        public Log(int id, string condition, string stackTrace, LogType type, DateTime dateTime)
        {
            Id = id;
            Condition = condition;
            StackTrace = stackTrace;
            Type = type;
            DateTime = dateTime;
        }
    }
}
