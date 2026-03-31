using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class StreetSceneEditorBootstrap
{
    private const string SampleScenePath = "Assets/Scenes/SampleScene.unity";

    static StreetSceneEditorBootstrap()
    {
        EditorSceneManager.sceneOpened += HandleSceneOpened;
        EditorApplication.delayCall += HandleInitialScene;
    }

    private static void HandleInitialScene()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        EnsureStreetScene(SceneManager.GetActiveScene());
    }

    private static void HandleSceneOpened(Scene scene, OpenSceneMode mode)
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        EnsureStreetScene(scene);
    }

    private static void EnsureStreetScene(Scene scene)
    {
        if (!scene.IsValid() || scene.path != SampleScenePath)
        {
            return;
        }

        StreetSceneBuilder builder = Object.FindFirstObjectByType<StreetSceneBuilder>();
        if (builder == null)
        {
            GameObject root = new GameObject("StreetScene");
            builder = root.AddComponent<StreetSceneBuilder>();
            EditorSceneManager.MarkSceneDirty(scene);
        }

        AlignMainCamera();
        builder.RebuildStreetBlock();
        EditorSceneManager.MarkSceneDirty(scene);
    }

    private static void AlignMainCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            return;
        }

        Transform cameraTransform = mainCamera.transform;
        cameraTransform.position = new Vector3(0f, 14f, -31f);
        cameraTransform.rotation = Quaternion.Euler(18f, 0f, 0f);
        mainCamera.fieldOfView = 54f;
        mainCamera.backgroundColor = new Color(0.12941177f, 0.14901961f, 0.23921569f);

        EditorUtility.SetDirty(mainCamera);
        EditorUtility.SetDirty(cameraTransform);
    }
}
