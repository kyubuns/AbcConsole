using System.IO;
using UnityEditor;
using UnityEngine;

namespace Development
{
    public static class BeforeAssetStoreUpload
    {
        private const string Target = "Assets/AbcConsole";

        [MenuItem("Asset Store Tools/**Before Upload**")]
        public static void Run()
        {
            Debug.Log("Start");
            AssetDatabase.DeleteAsset("Assets/AbcConsole/LICENSE.md");
            AssetDatabase.DeleteAsset("Assets/AbcConsole/package.json");

            var basePath = Path.Combine(Application.dataPath, "AbcConsole");
            Directory.Move(Path.Combine(basePath, "Samples~/Demo"), Path.Combine(basePath, "Demo"));
            File.Move(Path.Combine(basePath, "Samples~/Demo.meta"), Path.Combine(basePath, "Demo.meta"));

            AssetDatabase.Refresh(ImportAssetOptions.Default);
            Debug.Log("Finish");
        }
    }
}