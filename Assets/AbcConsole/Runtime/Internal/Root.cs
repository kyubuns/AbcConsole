using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AnKuchen.Map;
using UnityEngine;

namespace AbcConsole.Internal
{
    public class Root : MonoBehaviour, IRoot
    {
        public static IRoot CurrentInstance => _currentInstance;
        private static Root _currentInstance;

        public IReadOnlyList<Log> Logs => _logs;
        public int LogCount { get; private set; }
        public ConsoleState State { get; private set; } = ConsoleState.None;
        public Executor Executor { get; private set; }
        public bool AutoOpenWhenError { get; set; }

        public const int MaxLogSize = 1000;
        public const int MaxAutocompleteSize = 10;

        private AbcConsoleUiElements _ui;
        private readonly List<Log> _logs = new List<Log>(MaxLogSize);

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);

            _currentInstance = this;
            Application.logMessageReceivedThreaded += ReceiveLogMessage;

            _ui = new AbcConsoleUiElements(GetComponentInChildren<UICache>());
            _ui.TriggerButton.onClick.AddListener(OnClickTriggerButton);
            _ui.LogDetail.onClick.AddListener(() =>
            {
                _ui.LogDetail.gameObject.SetActive(false);
            });

            Executor = new Executor();

            State = ConsoleState.None;
            _ui.Console.gameObject.SetActive(false);
            _ui.LogRoot.SetActive(false);
            _ui.InputRoot.SetActive(false);
            _ui.LogDetail.gameObject.SetActive(false);
        }

        public void OnDestroy()
        {
            if (_currentInstance == this) _currentInstance = null;
            Application.logMessageReceivedThreaded -= ReceiveLogMessage;
        }

        private void ReceiveLogMessage(string condition, string stacktrace, LogType type)
        {
            _logs.Add(new Log(LogCount, condition, stacktrace, type, DateTime.Now));
            LogCount++;
            while (_logs.Count > MaxLogSize) _logs.RemoveAt(0);

            if (AutoOpenWhenError && (type == LogType.Error || type == LogType.Assert || type == LogType.Exception))
            {
                ShowFullMode();
            }
        }

        public void OnClickTriggerButton()
        {
            if (State == ConsoleState.None)
            {
                ShowFullMode();
            }
            else if (State == ConsoleState.Full)
            {
                ShowMiniMode();
            }
            else if (State == ConsoleState.Mini)
            {
                Hide();
            }
        }

        public void ShowFullMode()
        {
            State = ConsoleState.Full;
            _ui.Console.gameObject.SetActive(true);
            _ui.LogRoot.SetActive(true);
            _ui.InputRoot.SetActive(true);
            _ui.LogDetail.gameObject.SetActive(false);
        }

        public void ShowMiniMode()
        {
            State = ConsoleState.Mini;
            _ui.Console.gameObject.SetActive(true);
            _ui.LogRoot.SetActive(false);
            _ui.InputRoot.SetActive(true);
            _ui.LogDetail.gameObject.SetActive(false);
        }

        public void Hide()
        {
            State = ConsoleState.None;
            _ui.Console.gameObject.SetActive(false);
            _ui.LogRoot.SetActive(false);
            _ui.InputRoot.SetActive(false);
            _ui.LogDetail.gameObject.SetActive(false);
        }

        public void ClearLogs()
        {
            _logs.Clear();
        }
    }

    public enum ConsoleState
    {
        None,
        Full,
        Mini,
    }

    public interface IRoot
    {
        ConsoleState State { get; }
        IReadOnlyList<Log> Logs { get; }
        bool AutoOpenWhenError { get; set; }
        void ClearLogs();
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

    public class DebugCommand
    {
        public string LowerMethodName { get; }
        public MethodInfo MethodInfo { get; }
        public AbcCommandAttribute Attribute { get; }

        public DebugCommand(MethodInfo methodInfo, AbcCommandAttribute attribute)
        {
            LowerMethodName = methodInfo.Name.ToLowerInvariant();
            MethodInfo = methodInfo;
            Attribute = attribute;
        }

        public string CreateSummaryText()
        {
            var parameters = MethodInfo.GetParameters().Select(x => $"({x.ParameterType.ToString().Split('.').Last()}){x.Name}");

            var summary = "";
            if (!string.IsNullOrWhiteSpace(Attribute.Summary)) summary = $" - {Attribute.Summary}";
            return $"{MethodInfo.Name} {string.Join(" ", parameters)}{summary}";
        }
    }
}