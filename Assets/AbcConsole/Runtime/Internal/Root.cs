using AnKuchen.KuchenList;
using AnKuchen.Map;
using UnityEngine;

namespace AbcConsole.Internal
{
    public class Root : MonoBehaviour
    {
        public void Awake()
        {
            DontDestroyOnLoad(gameObject);

            var ui = new AbcConsoleUiElements(GetComponentInChildren<UICache>());

            using (var editor = ui.Log.Edit())
            {
                for (var i = 0; i < 500; ++i)
                {
                    var i1 = i;
                    editor.Contents.Add(new UIFactory<AbcConsoleUiElements.LogLineUiElements, AbcConsoleUiElements.LogDetailUiElements>(x =>
                    {
                        x.Text.text = $"Log {i1}";
                        x.Button.onClick.AddListener(() =>
                        {
                            ui.InputField.text = x.Text.text;
                        });
                    }));
                }
            }

            using (var editor = ui.Autocomplete.Edit())
            {
                for (var i = 0; i < 5; ++i)
                {
                    var a = editor.Create();
                }
            }
        }
    }
}