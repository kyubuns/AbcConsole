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

        public void OnClickPrintErrorLogButton()
        {
            Debug.LogError("Test Error Log!");
        }

        private static IEnumerator PrintLog()
        {
            for (var i = 0; i < 30; ++i)
            {
                Debug.Log($"PrintLog {i}");
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}