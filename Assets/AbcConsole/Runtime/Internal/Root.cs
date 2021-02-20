using System.Collections.Generic;
using AnKuchen.KuchenList;
using AnKuchen.Map;
using UnityEngine;

namespace AbcConsole.Internal
{
    public class Root : MonoBehaviour
    {
        public static Root CurrentInstance { get; private set; }

        public IReadOnlyList<Log> Logs => _logs;
        public int LogCount => _logCount;

        private const int MaxLogSize = 1000;

        private AbcConsoleUiElements _ui;
        private readonly List<Log> _logs = new List<Log>(MaxLogSize);
        private int _logCount;

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
            _logs.Add(new Log(_logCount, condition, stacktrace, type));
            _logCount++;
            while(_logs.Count > MaxLogSize) _logs.RemoveAt(0);
        }

        public void OnClickTriggerButton()
        {
            _ui.Console.gameObject.SetActive(!_ui.Console.gameObject.activeSelf);
        }
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