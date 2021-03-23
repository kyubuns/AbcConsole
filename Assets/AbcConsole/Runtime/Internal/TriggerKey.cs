using UnityEngine;

namespace AbcConsole.Internal
{
    public class TriggerKey : MonoBehaviour
    {
        [SerializeField] private KeyCode keyCode = KeyCode.BackQuote;
        private Root _root;

        public void Start()
        {
            _root = GetComponentInParent<Root>();
        }

        public void Update()
        {
            if (Input.GetKeyDown(keyCode))
            {
                _root.OnClickTriggerButton();
            }

            if (Root.CurrentInstance.State != ConsoleState.None && Input.GetKeyDown(KeyCode.Escape))
            {
                _root.OnClickTriggerButton();
            }
        }
    }
}