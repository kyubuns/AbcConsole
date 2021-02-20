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
                using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    var currentActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
                    var unityPlayer = currentActivity.Get<AndroidJavaObject>("mUnityPlayer");
                    var view = unityPlayer.Call<AndroidJavaObject>("getView");

                    if (view == null) return 0;

                    int result;

                    using (var rect = new AndroidJavaObject("android.graphics.Rect"))
                    {
                        view.Call("getWindowVisibleDisplayFrame", rect);
                        result = Screen.height - rect.Call<int>("height");
                    }

                    if (TouchScreenKeyboard.hideInput) return result;

                    var softInputDialog = unityPlayer.Get<AndroidJavaObject>("mSoftInputDialog");
                    var window = softInputDialog?.Call<AndroidJavaObject>("getWindow");
                    var decorView = window?.Call<AndroidJavaObject>("getDecorView");

                    if (decorView == null) return result;

                    var decorHeight = decorView.Call<int>("getHeight");
                    result += decorHeight;

                    return result;
                }
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
