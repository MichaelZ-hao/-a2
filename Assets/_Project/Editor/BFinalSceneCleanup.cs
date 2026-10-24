using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public static class BFinalSceneCleanup
{
    [MenuItem("Tools/B/Final Scene Cleanup")]
    public static void Apply()
    {
        const string path = "Assets/_Project/Scenes/B_CityRoute.unity";
        EditorSceneManager.OpenScene(path, OpenSceneMode.Single);

        ReplaceWithControlledCollider("B_FallenTreeCollider", new Vector3(0f, 0f, 0f), Vector3.one);
        ReplaceWithControlledCollider("B_FallenTrafficLightCollider", new Vector3(0f, 0f, 0f), Vector3.one);

        AddSideBuildings();
        RenameSceneObjects();

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        Debug.Log("Final scene cleanup complete.");
    }

    private static void ReplaceWithControlledCollider(string objectName, Vector3 center, Vector3 size)
    {
        GameObject root = FindSceneObject(objectName);
        if (root == null) return;

        foreach (MeshCollider c in root.GetComponentsInChildren<MeshCollider>(true))
            Object.DestroyImmediate(c);
        foreach (BoxCollider c in root.GetComponentsInChildren<BoxCollider>(true))
            Object.DestroyImmediate(c);
        foreach (CapsuleCollider c in root.GetComponentsInChildren<CapsuleCollider>(true))
            Object.DestroyImmediate(c);
        foreach (SphereCollider c in root.GetComponentsInChildren<SphereCollider>(true))
            Object.DestroyImmediate(c);

        BoxCollider box = root.AddComponent<BoxCollider>();
        box.center = center;
        box.size = size;
        box.isTrigger = false;
        box.enabled = true;
    }

    private static void AddSideBuildings()
    {
        GameObject background = FindSceneObject("B_CityBackground_NoCollision");
        if (background == null) return;

        List<GameObject> sources = new List<GameObject>();
        foreach (Transform child in background.GetComponentsInChildren<Transform>(true))
        {
            if (child == background.transform) continue;
            if (child.name.Contains("Building") && child.GetComponent<MeshRenderer>() != null)
                sources.Add(child.gameObject);
        }
        if (sources.Count == 0) return;

        Vector3[] positions =
        {
            new Vector3(-10f, 0f, 8f), new Vector3(10f, 0f, 16f),
            new Vector3(-11f, 0f, 27f), new Vector3(11f, 0f, 35f),
            new Vector3(-10f, 0f, 47f), new Vector3(10f, 0f, 55f),
            new Vector3(-11f, 0f, 66f), new Vector3(11f, 0f, 72f)
        };

        for (int i = 0; i < positions.Length; i++)
        {
            GameObject source = sources[i % sources.Count];
            GameObject copy = Object.Instantiate(source, background.transform);
            copy.name = "Building_Decoration_" + (i + 1).ToString("00");
            copy.transform.position = positions[i];
            copy.transform.rotation = Quaternion.Euler(0f, (i % 2 == 0) ? 180f : 0f, 0f);
            copy.transform.localScale = source.transform.localScale;
            foreach (Collider c in copy.GetComponentsInChildren<Collider>(true))
                c.enabled = false;
        }
    }

    private static void RenameSceneObjects()
    {
        foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
            RenameChildren(root.transform);
    }

    private static void RenameChildren(Transform parent)
    {
        if (parent.name.StartsWith("B_"))
            parent.name = parent.name.Substring(2);
        foreach (Transform child in parent)
            RenameChildren(child);
    }

    private static GameObject FindSceneObject(string objectName)
    {
        foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            GameObject result = FindChild(root.transform, objectName);
            if (result != null) return result;
        }
        return null;
    }

    private static GameObject FindChild(Transform parent, string objectName)
    {
        if (parent.name == objectName) return parent.gameObject;
        foreach (Transform child in parent)
        {
            GameObject result = FindChild(child, objectName);
            if (result != null) return result;
        }
        return null;
    }
}
