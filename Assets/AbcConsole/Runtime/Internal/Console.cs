using AnKuchen.KuchenList;
using AnKuchen.Map;
using UnityEngine;

namespace AbcConsole.Internal
{
    public class Console : MonoBehaviour
    {
        private Root _root;
        private AbcConsoleUiElements _ui;
        private int _logUpdatedCount;
        private int? _selectingLogId;
        private bool _forceUpdate;

        private static readonly Color LogColor = new Color32(0, 0, 0, 0);
        private static readonly Color WarningColor = new Color32(255, 255, 0, 32);
        private static readonly Color ErrorColor = new Color32(255, 0, 0, 32);
        private static readonly Color ExceptionColor = new Color32(255, 0, 0, 32);
        private static readonly Color AssertColor = new Color32(255, 0, 0, 32);

        public void OnEnable()
        {
            Debug.Log("Console.OnEnable");

            if (_root == null)
            {
                _root = GetComponentInParent<Root>();
            }

            if (_ui == null)
            {
                _ui = new AbcConsoleUiElements(GetComponentInParent<UICache>());
                _ui.EnterButton.onClick.AddListener(() => OnClickEnterButton());
                _ui.PasteButton.onClick.AddListener(() => OnClickPasteButton());
            }

            /*
            using (var editor = _ui.Autocomplete.Edit())
            {
                for (var i = 0; i < 5; ++i)
                {
                    var a = editor.Create();
                    a.Text.text = $"Autocomplete {i}";
                    a.Button.onClick.AddListener(() =>
                    {
                        _ui.InputField.text = a.Text.text;
                    });
                }
            }
            */
        }

        public void Update()
        {
            if (!_ui.LogRoot.activeSelf) return;
            if (!_forceUpdate && _logUpdatedCount == _root.LogCount) return;
            _logUpdatedCount = _root.LogCount;
            _forceUpdate = false;

            using (var editor = _ui.Log.Edit())
            {
                foreach (var log in _root.Logs)
                {
                    editor.Contents.Add(new UIFactory<AbcConsoleUiElements.LogLineUiElements, AbcConsoleUiElements.LogDetailUiElements>(x =>
                    {
                        var imageColor = Color.clear;
                        if (log.Type == LogType.Log) imageColor = LogColor;
                        else if (log.Type == LogType.Warning) imageColor = WarningColor;
                        else if (log.Type == LogType.Error) imageColor = ErrorColor;
                        else if (log.Type == LogType.Exception) imageColor = ExceptionColor;
                        else if (log.Type == LogType.Assert) imageColor = AssertColor;

                        x.Text.text = log.Condition;
                        x.Image.color = imageColor;

                        if (log.Condition.StartsWith("> "))
                        {
                            x.Button.onClick.AddListener(() =>
                            {
                                _ui.InputField.text = log.Condition.TrimStart('>').Trim();
                            });
                        }
                        else if (_selectingLogId == log.Id)
                        {
                            x.Button.onClick.AddListener(() =>
                            {
                                _selectingLogId = null;
                                _forceUpdate = true;
                            });
                        }
                        else
                        {
                            x.Button.onClick.AddListener(() =>
                            {
                                _selectingLogId = log.Id;
                                _forceUpdate = true;
                            });
                        }
                    }));

                    if (log.Id == _selectingLogId)
                    {
                        editor.Contents.Add(new UIFactory<AbcConsoleUiElements.LogLineUiElements, AbcConsoleUiElements.LogDetailUiElements>(x =>
                        {
                            x.CopyButton.onClick.AddListener(() =>
                            {
                                GUIUtility.systemCopyBuffer = $"{log.Condition}\n---\n{log.StackTrace}";
                                Debug.Log("DebugLog Copied!");
                                _selectingLogId = null;
                            });
                        }));
                    }
                }
            }
        }

        public void OnClickEnterButton()
        {
            var text = _ui.InputField.text.Trim();
            _ui.InputField.text = "";
            Debug.Log($"> {text}");
        }

        public void OnClickPasteButton()
        {
            _ui.InputField.text = GUIUtility.systemCopyBuffer;
        }
    }
}