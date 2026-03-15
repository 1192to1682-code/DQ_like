using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

public class FixBattleScene
{
    public static void Fix()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/BattleScene.unity");
        GameObject canvasToRemove = null;
        foreach (var obj in scene.GetRootGameObjects())
        {
            if (obj.name == "Canvas")
            {
                // Verify it has no StatusUI component to be safe
                if (obj.GetComponent("StatusUI") == null)
                {
                    canvasToRemove = obj;
                    break;
                }
            }
        }

        if (canvasToRemove != null)
        {
            GameObject.DestroyImmediate(canvasToRemove);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("Removed rogue Canvas from BattleScene");
        }
        else
        {
            Debug.Log("Canvas not found");
        }
    }
}
