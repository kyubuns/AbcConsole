using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AbcConsole.Editor
{
    public static class CreateMenu
    {
        [MenuItem("Assets/Create/AbcConsole")]
        public static void Create()
        {
            var prefabGuid = AssetDatabase.FindAssets("AbcConsole t:Prefab").Single();
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(prefabGuid));

            var instance = PrefabUtility.InstantiatePrefab(prefab);
            instance.name = "AbcConsole";

            Selection.objects = new[] { instance };
        }
    }
}