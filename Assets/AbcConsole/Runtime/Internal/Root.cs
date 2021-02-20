using AnKuchen.KuchenList;
using AnKuchen.Map;
using UnityEngine;

namespace AbcConsole.Internal
{
    public class Root : MonoBehaviour
    {
        private AbcConsoleUiElements _ui;

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);

            _ui = new AbcConsoleUiElements(GetComponentInChildren<UICache>());

            _ui.TriggerButton.onClick.AddListener(OnClickTriggerButton);

            // Test
            using (var editor = _ui.Log.Edit())
            {
                for (var i = 0; i < 500; ++i)
                {
                    var i1 = i;
                    editor.Contents.Add(new UIFactory<AbcConsoleUiElements.LogLineUiElements, AbcConsoleUiElements.LogDetailUiElements>(x =>
                    {
                        x.Text.text = $"Log {i1}";
                        x.Button.onClick.AddListener(() =>
                        {
                            _ui.InputField.text = x.Text.text;
                        });
                    }));
                }
            }

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
        }

        public void OnClickTriggerButton()
        {
            _ui.Console.gameObject.SetActive(!_ui.Console.gameObject.activeSelf);
        }
    }
}