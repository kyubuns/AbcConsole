using System.Collections;
using UnityEngine;

namespace AbcConsole.Demo
{
    public class DemoButtons : MonoBehaviour
    {
        public void OnClickPrintLogButton()
        {
            StartCoroutine(PrintLog());
        }

        private IEnumerator PrintLog()
        {
            for (var i = 0; i < 50; ++i)
            {
                Debug.Log($"PrintLog {i}");
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}