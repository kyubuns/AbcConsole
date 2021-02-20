using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AbcConsole.Internal
{
    public static class InputFieldExtensions
    {
        public static void Focus(this InputField inputField)
        {
            var go = new GameObject("AbcConsoleCoroutineRunner");

            IEnumerator Internal()
            {
                yield return new WaitForEndOfFrame();
                if (TouchScreenKeyboard.isSupported)
                {
                    TouchScreenKeyboard.Open(string.Empty, TouchScreenKeyboardType.ASCIICapable);
                }
                EventSystem.current.SetSelectedGameObject(inputField.gameObject);
                inputField.ActivateInputField();
                Object.Destroy(go);
            }

            var coroutineRunner = go.AddComponent<CoroutineRunner>();
            coroutineRunner.StartCoroutine(Internal());
        }
    }

    public class CoroutineRunner : MonoBehaviour
    {
    }
}