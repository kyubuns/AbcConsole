using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AnKuchen.Map;
using UnityEngine;

namespace AbcConsole.Internal
{
    internal class Root : MonoBehaviour
    {
        internal static Root CurrentInstance => _currentInstance;
        private static Root _currentInstance;

        internal IReadOnlyList<Log> Logs => _logs;
        internal int LogCount { get; private set; }
        internal ConsoleState State { get; private set; } = ConsoleState.None;
        internal Executor Executor { get; private set; }
        internal bool AutoOpenWhenError { get; set; }
        internal Action<IReadOnlyList<Log>> ErrorCallback { get; set; }

        internal const int MaxLogSize = 1000;
        internal const int MaxAutocompleteSize = 10;

        private AbcConsoleUiElements _ui;
        private readonly List<Log> _logs = new List<Log>(MaxLogSize);
        private static int _mainThreadId;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            _mainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }

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
            var item = new Log(LogCount, condition, stacktrace, type, DateTime.Now);
            _logs.Add(item);
            LogCount++;
            while (_logs.Count > MaxLogSize) _logs.RemoveAt(0);

            if (type == LogType.Error || type == LogType.Assert || type == LogType.Exception)
            {
                if (AutoOpenWhenError && System.Threading.Thread.CurrentThread.ManagedThreadId == _mainThreadId)
                {
                    ShowFullMode();
                    _ui.Console.OpenDetailWindow(item);
                }

                ErrorCallback?.Invoke(_logs);
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

        public bool FpcCounterEnabled
        {
            get => _ui.FpsCounterRoot.activeSelf;
            set => _ui.FpsCounterRoot.SetActive(value);
        }
    }

    internal class DebugCommand
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