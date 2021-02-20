using System.Collections.Generic;
using AnKuchen.Map;
using UnityEngine;

namespace AbcConsole.Internal
{
    public class Root : MonoBehaviour
    {
        public static Root CurrentInstance { get; private set; }

        public IReadOnlyList<Log> Logs => _logs;
        public int LogCount { get; private set; }
        public ConsoleState State { get; private set; } = ConsoleState.None;

        private const int MaxLogSize = 1000;

        private AbcConsoleUiElements _ui;
        private readonly List<Log> _logs = new List<Log>(MaxLogSize);

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);

            CurrentInstance = this;
            Application.logMessageReceivedThreaded += ReceiveLogMessage;

            _ui = new AbcConsoleUiElements(GetComponentInChildren<UICache>());
            _ui.TriggerButton.onClick.AddListener(OnClickTriggerButton);
        }

        public void OnDestroy()
        {
            if (CurrentInstance == this) CurrentInstance = null;
            Application.logMessageReceivedThreaded -= ReceiveLogMessage;
        }

        private void ReceiveLogMessage(string condition, string stacktrace, LogType type)
        {
            _logs.Add(new Log(LogCount, condition, stacktrace, type));
            LogCount++;
            while (_logs.Count > MaxLogSize) _logs.RemoveAt(0);
        }

        public void OnClickTriggerButton()
        {
            if (State == ConsoleState.None)
            {
                State = ConsoleState.Full;
                _ui.Console.gameObject.SetActive(true);
                _ui.LogRoot.SetActive(true);
                _ui.InputRoot.SetActive(true);
            }
            else if (State == ConsoleState.Full)
            {
                State = ConsoleState.Mini;
                _ui.Console.gameObject.SetActive(true);
                _ui.LogRoot.SetActive(false);
                _ui.InputRoot.SetActive(true);
            }
            else if (State == ConsoleState.Mini)
            {
                State = ConsoleState.None;
                _ui.Console.gameObject.SetActive(false);
                _ui.LogRoot.SetActive(false);
                _ui.InputRoot.SetActive(false);
            }
        }
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

        public Log(int id, string condition, string stackTrace, LogType type)
        {
            Id = id;
            Condition = condition;
            StackTrace = stackTrace;
            Type = type;
        }
    }
}