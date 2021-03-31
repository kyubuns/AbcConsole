#if NEW_INPUT_SYSTEM_SUPPORT
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#else
using UnityEngine;
#endif

namespace AbcConsole.Internal
{
    public static class KeyInput
    {
#if NEW_INPUT_SYSTEM_SUPPORT
        public static bool GetKeyDown(string keyCode)
        {
            return ((KeyControl) Keyboard.current[keyCode]).wasPressedThisFrame;
        }
#else
        public static bool GetKeyDown(KeyCode keyCode)
        {
            return Input.GetKeyDown(keyCode);
        }
#endif

        public static bool GetEscapeKeyDown()
        {
#if NEW_INPUT_SYSTEM_SUPPORT
            return Keyboard.current.escapeKey.wasPressedThisFrame;
#else
            return Input.GetKeyDown(KeyCode.Escape);
#endif
        }

        public static bool GetReturnKeyDown()
        {
#if NEW_INPUT_SYSTEM_SUPPORT
            return Keyboard.current.enterKey.wasPressedThisFrame;
#else
            return Input.GetKeyDown(KeyCode.Return);
#endif
        }

        public static bool GetTabKeyDown()
        {
#if NEW_INPUT_SYSTEM_SUPPORT
            return Keyboard.current.tabKey.wasPressedThisFrame;
#else
            return Input.GetKeyDown(KeyCode.Tab);
#endif
        }

        public static bool GetUpArrowKeyDown()
        {
#if NEW_INPUT_SYSTEM_SUPPORT
            return Keyboard.current.upArrowKey.wasPressedThisFrame;
#else
            return Input.GetKeyDown(KeyCode.UpArrow);
#endif
        }

        public static bool GetDownArrowKeyDown()
        {
#if NEW_INPUT_SYSTEM_SUPPORT
            return Keyboard.current.downArrowKey.wasPressedThisFrame;
#else
            return Input.GetKeyDown(KeyCode.DownArrow);
#endif
        }
    }
}