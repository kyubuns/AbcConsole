using System.Linq;
using AnKuchen.Map;
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

            var instance = (GameObject) PrefabUtility.InstantiatePrefab(prefab);
            instance.name = "AbcConsole";
            instance.GetComponentInChildren<UICache>().Get("Console").SetActive(false);

            Selection.objects = new Object[] { instance };
        }
    }
}