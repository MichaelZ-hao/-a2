using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class BPutCapsuleOnObjects
{
    private const string ScenePath = "Assets/_Project/Scenes/B_CityRoute.unity";

    public static void Apply()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Configure("Tree", "TreeCollider", 0.30f, 1.0f, 2, new Vector3(0f, 0.2f, 0f));
        Configure("TrafficLight", "TrafficLightCollider", 0.63f, 1.06f, 0, Vector3.zero);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("BPutCapsuleOnObjects completed");
    }

    private static void Configure(string objectName, string oldChildName, float radius, float height, int direction, Vector3 center)
    {
        var go = GameObject.Find(objectName);
        if (go == null) return;
        var oldChild = go.transform.parent != null ? go.transform.parent.Find(oldChildName) : null;
        if (oldChild != null) Object.DestroyImmediate(oldChild.gameObject);
        var oldBox = go.GetComponent<BoxCollider>();
        if (oldBox != null) oldBox.enabled = false;
        var capsule = go.GetComponent<CapsuleCollider>();
        if (capsule == null) capsule = go.AddComponent<CapsuleCollider>();
        capsule.enabled = true;
        capsule.isTrigger = false;
        capsule.direction = direction;
        capsule.radius = radius;
        capsule.height = height;
        capsule.center = center;
    }
}
