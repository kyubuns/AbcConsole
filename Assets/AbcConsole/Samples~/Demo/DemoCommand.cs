using UnityEngine;

namespace AbcConsole.Demo
{
    public static class DemoCommand
    {
        [AbcCommand("Show application version")]
        public static void AppVersion()
        {
            Debug.Log($"AppVersion is {Application.version}");
        }

        [AbcCommand]
        public static void SetBackgroundColor(int r, int g, int b)
        {
            GameObject.Find("Main Camera").GetComponent<Camera>().backgroundColor = new Color32((byte) r, (byte) g, (byte) b, 255);
        }

        [AbcCommand]
        public static void SpawnCube()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "Cube";

            Debug.Log("Spawn Cube Success!");
        }

        [AbcCommand]
        public static void SpawnCube(string name)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;

            Debug.Log("Spawn Cube Success!");
        }

        [AbcCommand]
        public static void SetCubeColor(int r, int g, int b)
        {
            var cube = GameObject.Find("Cube");
            cube.GetComponent<MeshRenderer>().material.color = new Color32((byte) r, (byte) g, (byte) b, 255);

            Debug.Log($"Set Cube Color ({r}, {g}, {b})");
        }

        [AbcCommand]
        public static void SetCubePosition(float x, float y, float z)
        {
            var cube = GameObject.Find("Cube");
            cube.transform.position = new Vector3(x, y, z);

            Debug.Log($"Set Cube Position ({x}, {y}, {z})");
        }
    }
}