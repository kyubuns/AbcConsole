using UnityEngine;

namespace AbcConsole.Internal
{
    public class TriggerKey : MonoBehaviour
    {
#if NEW_INPUT_SYSTEM_SUPPORT
        [SerializeField] private string keyCode = "Backquote";
#else
        [SerializeField] private KeyCode keyCode = KeyCode.BackQuote;
#endif
        private Root _root;

        public void Start()
        {
            _root = GetComponentInParent<Root>();
        }

        public void Update()
        {
            if (KeyInput.GetKeyDown(keyCode))
            {
                _root.OnClickTriggerButton();
            }

            if (Root.CurrentInstance.State != ConsoleState.None && KeyInput.GetEscapeKeyDown())
            {
                _root.OnClickTriggerButton();
            }
        }
    }
}