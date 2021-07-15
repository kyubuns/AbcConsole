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
        private bool _freezeAutocomplete;
        private int? _autocompleteSelecting;
        private bool _freezeSelectHistory;
        private int? _selectHistory;
        private string _autocompleteSelectingOriginalInput;
        private DebugCommand[] _autocompleteCache;
        private float _prevFrameKeyboardHeight;
        private float _stableKeyboardHeight;
        private int _onInputFieldEndEditFrame;
        private Rect _cachedSafeArea;

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
                _ui.EnterButton.onClick.AddListener(() =>
                {
                    var isFocused = (_onInputFieldEndEditFrame == Time.frameCount);
                    OnClickEnterButton();
                    if (isFocused) _ui.InputField.FocusAndMoveToEnd();
                });
                _ui.PasteButton.onClick.AddListener(() =>
                {
                    var isFocused = (_onInputFieldEndEditFrame == Time.frameCount);
                    OnClickPasteButton();
                    if (isFocused) _ui.InputField.FocusAndMoveToEnd();
                });
                _ui.InputField.onEndEdit.AddListener(_ => OnInputFieldEndEdit());
                _ui.InputField.onValueChanged.AddListener(OnInputFieldValueChanged);
                _ui.InputField.onValidateInput += (text, index, addedChar) =>
                {
                    if (addedChar == '`') return '\0';
                    return addedChar;
                };
            }

            _ui.InputField.FocusAndMoveToEnd();
        }

        public void Update()
        {
            UpdateClearAutocomplete();
            UpdateKeys();
            UpdateSafeArea();
            UpdateViewArea();
            UpdateLogs();
        }

        private void UpdateClearAutocomplete()
        {
            if (_ui.Autocomplete.Elements.Length > 0 && Input.touchCount > 0)
            {
                ClearAutocomplete();
            }
        }

        private void UpdateKeys()
        {
            if (!_ui.InputField.IsActive()) return;

            if (KeyInput.GetTabKeyDown()) RequestAutocomplete();
            if (KeyInput.GetUpArrowKeyDown()) RequestHistoryUp();
            if (KeyInput.GetDownArrowKeyDown()) RequestHistoryDown();
        }

        private void RequestAutocomplete()
        {
            _freezeAutocomplete = true;
            using (Disposable.Create(() => _freezeAutocomplete = false))
            {
                if (_autocompleteSelecting == null) _autocompleteSelecting = 0;
                else _autocompleteSelecting = _autocompleteSelecting.Value + 1;

                if (_ui.Autocomplete.Elements.Length <= _autocompleteSelecting.Value)
                {
                    _autocompleteSelecting = null;
                    _ui.InputField.text = _autocompleteSelectingOriginalInput;
                }
                else
                {
                    _ui.Autocomplete.Elements[_autocompleteSelecting.Value].Button.onClick.Invoke();
                }
            }
        }

        private void RequestHistoryUp()
        {
            if (_selectHistory == null) _selectHistory = 0;
            else _selectHistory = _selectHistory.Value + 1;
            RequestHistory();
        }

        private void RequestHistoryDown()
        {
            if (_selectHistory == null || _selectHistory == 0) _selectHistory = null;
            else _selectHistory = _selectHistory.Value - 1;
            RequestHistory();
        }

        private void RequestHistory()
        {
            if (_selectHistory == null) return;

            _freezeSelectHistory = true;
            using (Disposable.Create(() => _freezeSelectHistory = false))
            {
                var count = 0;
                for (var i = _root.Logs.Count - 1; i >= 0; --i)
                {
                    if (!_root.Logs[i].Condition.StartsWith("> ")) continue;
                    if (count != _selectHistory)
                    {
                        count++;
                        continue;
                    }

                    _ui.InputField.text = _root.Logs[i].Condition.TrimStart('>').Trim();
                    break;
                }
            }
        }

        private void UpdateSafeArea()
        {
            if (_cachedSafeArea == Screen.safeArea) return;
            _cachedSafeArea = Screen.safeArea;

            var resolutionWidth = _canvasRect.sizeDelta.x;
            var rate = resolutionWidth / Screen.width;
            var leftMargin = _cachedSafeArea.x * rate;
            var rightMargin = (Screen.width - _cachedSafeArea.x - _cachedSafeArea.width) * rate;
            _ui.ViewArea.sizeDelta = new Vector2(-(leftMargin + rightMargin), _ui.ViewArea.sizeDelta.y);
            _ui.ViewArea.anchoredPosition = new Vector2((leftMargin - rightMargin) / 2f, 0f);
        }

        private float GetKeyboardHeight()
        {
            // たまに1フレだけ荒ぶることがあるので2フレ連続しないと値を採用しない
            var keyboardHeight = KeyboardRect.GetHeight();
            if (Mathf.Abs(_prevFrameKeyboardHeight - keyboardHeight) < 0.0001f)
            {
                // 2フレ連続で同じ値ならオッケー
                _stableKeyboardHeight = keyboardHeight;
            }
            else
            {
                _prevFrameKeyboardHeight = keyboardHeight;
            }

            return _stableKeyboardHeight;
        }

        private void UpdateViewArea()
        {
            var keyboardHeight = GetKeyboardHeight();
            if (Mathf.Abs(keyboardHeight - _updatedKeyboardHeight) < 0.0001f) return;
            _updatedKeyboardHeight = keyboardHeight;

            var resolutionHeight = _canvasRect.sizeDelta.y;
            var rate = resolutionHeight / Screen.height;
            var margin = keyboardHeight * rate;
            _ui.ViewArea.sizeDelta = new Vector2(_ui.ViewArea.sizeDelta.x, -margin);
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
                            x.DetailButton.onClick.AddListener(() =>
                            {
                                _ui.LogDetailText.text = $"{log.DateTime}\n{log.Condition}\n---\n{log.StackTrace}";
                                _ui.LogDetail.gameObject.SetActive(true);
                            });
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
            _onInputFieldEndEditFrame = Time.frameCount;

            if (KeyInput.GetReturnKeyDown())
            {
                OnClickEnterButton();
                _ui.InputField.FocusAndMoveToEnd();
                return;
            }

            if (_ui.InputField.touchScreenKeyboard?.status == TouchScreenKeyboard.Status.Done)
            {
                OnClickEnterButton();
                _ui.InputField.FocusAndMoveToEnd();
                return;
            }
        }

        private void ClearAutocomplete()
        {
            using (_ui.Autocomplete.Edit())
            {
            }
        }

        private void OnInputFieldValueChanged(string text)
        {
            if (!_freezeSelectHistory)
            {
                _selectHistory = null;
            }

            if (!_freezeAutocomplete)
            {
                _autocompleteCache = _root.Executor.GetAutocomplete(text);
                _autocompleteSelecting = null;
                _autocompleteSelectingOriginalInput = text;

                using (var editor = _ui.Autocomplete.Edit())
                {
                    foreach (var command in _autocompleteCache)
                    {
                        var a = editor.Create();
                        a.Text.text = command.CreateSummaryText();
                        a.Button.onClick.AddListener(() =>
                        {
                            _ui.InputField.text = $"{command.MethodInfo.Name} ";
                            _ui.InputField.FocusAndMoveToEnd();
                        });
                    }
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