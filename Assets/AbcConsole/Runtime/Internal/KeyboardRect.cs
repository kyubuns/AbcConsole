using UnityEngine;

namespace AbcConsole.Internal
{
    public static class KeyboardRect
    {
        public static float GetHeight()
        {
            if (Application.isEditor)
            {
                return 0f;
            }

            if (Application.platform == RuntimePlatform.Android)
            {
                return 0f;
            }

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                var area = TouchScreenKeyboard.area;
                return area.height;
            }

            return 0f;
        }
    }
}
