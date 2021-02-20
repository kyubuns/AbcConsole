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
    }
}