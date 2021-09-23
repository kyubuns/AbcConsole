using UnityEngine;
using UnityEngine.UI;

namespace AbcConsole.Internal
{
    public class FpsCounter : MonoBehaviour
    {
        [SerializeField] private Text text = default;

        public void Update()
        {
            text.text = $"{Time.time:0.00}";
        }
    }
}