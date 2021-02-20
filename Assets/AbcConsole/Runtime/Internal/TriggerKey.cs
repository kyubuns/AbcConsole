using UnityEngine;

namespace AbcConsole.Internal
{
    public class TriggerKey : MonoBehaviour
    {
        [SerializeField] private KeyCode keyCode = KeyCode.BackQuote;

        public void Update()
        {
            if (!Input.GetKeyDown(keyCode)) return;
            GetComponentInParent<Root>().OnClickTriggerButton();
        }
    }
}