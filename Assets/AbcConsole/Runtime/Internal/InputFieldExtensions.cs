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
            IEnumerator Internal()
            {
                yield return new WaitForEndOfFrame();
                if (TouchScreenKeyboard.isSupported)
                {
                    TouchScreenKeyboard.Open(inputField.text, TouchScreenKeyboardType.ASCIICapable);
                }
                EventSystem.current.SetSelectedGameObject(inputField.gameObject);
                inputField.ActivateInputField();
            }

            Utils.StartCoroutine(Internal());
        }

        public static void FocusAndMoveToEnd(this InputField inputField)
        {
            IEnumerator Internal()
            {
                yield return new WaitForEndOfFrame();
                if (TouchScreenKeyboard.isSupported)
                {
                    TouchScreenKeyboard.Open(inputField.text, TouchScreenKeyboardType.ASCIICapable);
                }
                EventSystem.current.SetSelectedGameObject(inputField.gameObject);
                inputField.ActivateInputField();

                yield return new WaitForEndOfFrame();

                inputField.MoveTextEnd(false);
            }

            Utils.StartCoroutine(Internal());
        }
    }

    public class CoroutineRunner : MonoBehaviour
    {
    }
}