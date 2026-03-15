using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

public class DumpBattleSceneInfo
{
    public static void Dump()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/BattleScene.unity");
        Debug.Log("--- ROOT GAMEOBJECTS ---");
        foreach (var obj in scene.GetRootGameObjects())
        {
            Debug.Log("Root: " + obj.name);
            var canvases = obj.GetComponentsInChildren<Canvas>(true);
            foreach (var c in canvases)
            {
                Debug.Log("  Found Canvas on: " + c.gameObject.name);
                var texts = c.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true);
                foreach (var t in texts)
                {
                    Debug.Log("    Text: " + t.name + " -> " + t.text);
                }
            }
        }
        Debug.Log("------------------------");
    }
}
