using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AbcConsole.Internal
{
    public static class InputFieldExtensions
    {
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

            if (
                Application.platform == RuntimePlatform.OSXEditor
                || Application.platform == RuntimePlatform.OSXPlayer
                || Application.platform == RuntimePlatform.WindowsPlayer
                || Application.platform == RuntimePlatform.WindowsEditor
                || Application.platform == RuntimePlatform.IPhonePlayer
                || Application.platform == RuntimePlatform.Android
                || Application.platform == RuntimePlatform.LinuxPlayer
                || Application.platform == RuntimePlatform.LinuxEditor
                || Application.platform == RuntimePlatform.WebGLPlayer
            )
            {
                Utils.StartCoroutine(Internal());
            }
        }
    }

    public class CoroutineRunner : MonoBehaviour
    {
    }
}