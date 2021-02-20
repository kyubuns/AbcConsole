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
                for (var i = 0; i < 5; ++i)
                {
                    var i1 = i;
                    editor.Contents.Add(new UIFactory<AbcConsoleUiElements.LogLineUiElements>(x =>
                    {
                        x.Text.text = $"Log {i1}";
                    }));
                }
            }
        }
    }
}