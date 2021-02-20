using System.Collections;
using AnKuchen.KuchenList;
using AnKuchen.Map;
using UnityEngine;
using UnityEngine.UI;

namespace AbcConsole.Internal
{
    public class Console : MonoBehaviour
    {
        private Root _root;
        private RectTransform _canvasRect;
        private AbcConsoleUiElements _ui;
        private int _logUpdatedCount;
        private int? _selectingLogId;
        private bool _forceUpdate;
        private float _updatedKeyboardHeight;

        private static readonly Color LogColor = new Color32(0, 0, 0, 0);
        private static readonly Color WarningColor = new Color32(255, 255, 0, 32);
        private static readonly Color ErrorColor = new Color32(255, 0, 0, 32);
        private static readonly Color ExceptionColor = new Color32(255, 0, 0, 32);
        private static readonly Color AssertColor = new Color32(255, 0, 0, 32);
        private Coroutine _clearAutocompleteCoroutine;

        public void OnEnable()
        {
            if (_root == null)
            {
                _root = GetComponentInParent<Root>();
            }

            if (_canvasRect == null)
            {
                _canvasRect = GetComponentInParent<CanvasScaler>().GetComponent<RectTransform>();
            }

            if (_ui == null)
            {
                _ui = new AbcConsoleUiElements(GetComponentInParent<UICache>());
                _ui.EnterButton.onClick.AddListener(OnClickEnterButton);
                _ui.PasteButton.onClick.AddListener(OnClickPasteButton);
                _ui.InputField.onEndEdit.AddListener(_ => OnInputFieldEndEdit());
                _ui.InputField.onValueChanged.AddListener(OnInputFieldValueChanged);
                _ui.InputField.onValidateInput += (text, index, addedChar) =>
                {
                    if (addedChar == '`') return '\0';
                    return addedChar;
                };
            }

            _ui.InputField.Focus();
        }

        public void Update()
        {
            UpdateViewArea();
            UpdateLogs();
        }

        private void UpdateViewArea()
        {
            var keyboardHeight = KeyboardRect.GetHeight();
            if (Mathf.Abs(keyboardHeight - _updatedKeyboardHeight) < 0.0001f) return;

            _updatedKeyboardHeight = keyboardHeight;

            var resolutionHeight = _canvasRect.sizeDelta.y;
            var rate = resolutionHeight / Screen.height;
            var margin = keyboardHeight * rate;
            _ui.ViewArea.sizeDelta = new Vector2(0, -margin);
            _ui.Log.NormalizedPosition = 0.0f;
        }

        private void UpdateLogs()
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

        private void OnInputFieldEndEdit()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnClickEnterButton();
                _ui.InputField.Focus();
            }

            if (_ui.InputField.touchScreenKeyboard?.status == TouchScreenKeyboard.Status.Done)
            {
                OnClickEnterButton();
                _ui.InputField.Focus();
            }

            IEnumerator ClearAutocomplete()
            {
                // Buttonのクリック判定を出すために待つ
                yield return new WaitForSecondsRealtime(0.2f);
                using (_ui.Autocomplete.Edit())
                {
                }
                _clearAutocompleteCoroutine = null;
            }

            _clearAutocompleteCoroutine = StartCoroutine(ClearAutocomplete());
        }

        private void OnInputFieldValueChanged(string text)
        {
            var commands = _root.Executor.GetAutocomplete(text);

            using (var editor = _ui.Autocomplete.Edit())
            {
                foreach(var command in commands)
                {
                    var a = editor.Create();
                    a.Text.text = command.CreateSummaryText();
                    a.Button.onClick.AddListener(() =>
                    {
                        if (_clearAutocompleteCoroutine != null) StopCoroutine(_clearAutocompleteCoroutine);

                        _ui.InputField.text = $"{command.MethodInfo.Name} ";
                        _ui.InputField.FocusAndMoveToEnd();
                    });
                }
            }
        }

        public void OnClickEnterButton()
        {
            var text = _ui.InputField.text.Trim();
            if (string.IsNullOrWhiteSpace(text)) return;

            Debug.Log($"> {text}");
            var executed = _root.Executor.ExecuteMethod(text);
            if (executed) _ui.InputField.text = "";
        }

        public void OnClickPasteButton()
        {
            _ui.InputField.text = GUIUtility.systemCopyBuffer;
        }
    }
}