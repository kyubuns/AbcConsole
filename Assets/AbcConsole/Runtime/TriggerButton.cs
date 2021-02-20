using UnityEngine;

namespace AbcConsole
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