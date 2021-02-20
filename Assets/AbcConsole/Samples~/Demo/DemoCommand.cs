using UnityEngine;

namespace AbcConsole.Demo
{
    public static class DemoCommand
    {
        [AbcCommand("message to debug log")]
        public static void Echo(string message)
        {
            Debug.Log(message);
        }

        [AbcCommand("warning")]
        public static void EchoWarning(string message)
        {
            Debug.LogWarning(message);
        }

        [AbcCommand("error")]
        public static void EchoError(string message)
        {
            Debug.LogError(message);
        }
    }
}