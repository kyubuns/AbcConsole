using UnityEngine;

namespace AbcConsole.Internal
{
    public class TriggerButton : MonoBehaviour
    {
        [SerializeField] private GameObject console = default;

        public void OnClick()
        {
            console.SetActive(!console.activeSelf);
        }
    }
}