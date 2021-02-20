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

        private const int MaxLogSize = 1000;

        private AbcConsoleUiElements _ui;
        private readonly List<Log> _logs = new List<Log>(MaxLogSize);

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);

            _currentInstance = this;
            Application.logMessageReceivedThreaded += ReceiveLogMessage;

            _ui = new AbcConsoleUiElements(GetComponentInChildren<UICache>());
            _ui.TriggerButton.onClick.AddListener(OnClickTriggerButton);

            Executor = new Executor();
        }

        public void OnDestroy()
        {
            if (_currentInstance == this) _currentInstance = null;
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
        IReadOnlyList<Log> Logs { get; }
        void ClearLogs();
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

    public class DebugCommand
    {
        public MethodInfo MethodInfo { get; }
        public AbcCommandAttribute Attribute { get; }

        public DebugCommand(MethodInfo methodInfo, AbcCommandAttribute attribute)
        {
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