using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace AbcConsole.Internal
{
    public static class InputFieldExtensions
    {
        public static void Focus(this InputField inputField, MonoBehaviour coroutineRunner)
        {
            IEnumerator ActivateInputFieldInternal()
            {
                yield return new WaitForEndOfFrame();
                inputField.ActivateInputField();
            }

            coroutineRunner.StartCoroutine(ActivateInputFieldInternal());
        }
    }
}