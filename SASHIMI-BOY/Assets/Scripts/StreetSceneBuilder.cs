using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
[DisallowMultipleComponent]
public class StreetSceneBuilder : MonoBehaviour
{
    private const string GeneratedRootName = "GeneratedStreetBlock";

    [SerializeField] private bool autoRebuild = true;

    private readonly Dictionary<Color32, Material> materialCache = new Dictionary<Color32, Material>();

#if UNITY_EDITOR
    private bool rebuildQueued;
#endif

    private void OnEnable()
    {
        if (autoRebuild)
        {
            QueueRebuild();
        }
    }

    private void OnValidate()
    {
        if (autoRebuild)
        {
            QueueRebuild();
        }
    }

    [ContextMenu("Rebuild Street Block")]
    public void RebuildStreetBlock()
    {
        RebuildImmediate();
    }

    private void QueueRebuild()
    {
#if UNITY_EDITOR
        if (Application.isPlaying || rebuildQueued)
        {
            return;
        }

        rebuildQueued = true;
        EditorApplication.delayCall += DelayedRebuild;
#else
        RebuildImmediate();
#endif
    }

#if UNITY_EDITOR
    private void DelayedRebuild()
    {
        rebuildQueued = false;

        if (this == null || !isActiveAndEnabled || Application.isPlaying)
        {
            return;
        }

        RebuildImmediate();
    }
#endif

    private void RebuildImmediate()
    {
        Transform existingRoot = transform.Find(GeneratedRootName);
        if (existingRoot != null)
        {
            DestroySmart(existingRoot.gameObject);
        }

        ClearMaterialCache();

        GameObject generatedRoot = new GameObject(GeneratedRootName);
        generatedRoot.transform.SetParent(transform, false);

        BuildStreet(generatedRoot.transform);
    }

    private void BuildStreet(Transform root)
    {
        CreateBlockBase(root);
        CreateStoreRow(root);
        CreateStreetProps(root);
    }

    private void CreateBlockBase(Transform root)
    {
        CreatePrimitive(
            PrimitiveType.Cube,
            "Ground",
            root,
            new Vector3(0f, -0.5f, 0f),
            Vector3.zero,
            new Vector3(48f, 1f, 68f),
            new Color(0.18f, 0.20f, 0.18f));

        CreatePrimitive(
            PrimitiveType.Cube,
            "Road",
            root,
            new Vector3(0f, 0.02f, 0f),
            Vector3.zero,
            new Vector3(10f, 0.12f, 56f),
            new Color(0.17f, 0.18f, 0.20f));

        CreatePrimitive(
            PrimitiveType.Cube,
            "LeftSidewalk",
            root,
            new Vector3(-7.2f, 0.08f, 0f),
            Vector3.zero,
            new Vector3(4f, 0.2f, 56f),
            new Color(0.53f, 0.50f, 0.47f));

        CreatePrimitive(
            PrimitiveType.Cube,
            "RightSidewalk",
            root,
            new Vector3(7.2f, 0.08f, 0f),
            Vector3.zero,
            new Vector3(4f, 0.2f, 56f),
            new Color(0.53f, 0.50f, 0.47f));

        for (int i = -5; i <= 5; i++)
        {
            CreatePrimitive(
                PrimitiveType.Cube,
                $"LaneStripe_{i + 5}",
                root,
                new Vector3(0f, 0.1f, i * 4.8f),
                Vector3.zero,
                new Vector3(0.3f, 0.02f, 2.2f),
                new Color(0.95f, 0.90f, 0.72f));
        }

        for (int i = -3; i <= 3; i++)
        {
            CreatePrimitive(
                PrimitiveType.Cube,
                $"CrosswalkLeft_{i + 3}",
                root,
                new Vector3(-1.7f, 0.11f, 0.7f + (i * 0.9f)),
                Vector3.zero,
                new Vector3(2.2f, 0.02f, 0.45f),
                new Color(0.93f, 0.93f, 0.91f));

            CreatePrimitive(
                PrimitiveType.Cube,
                $"CrosswalkRight_{i + 3}",
                root,
                new Vector3(1.7f, 0.11f, -0.7f + (i * 0.9f)),
                Vector3.zero,
                new Vector3(2.2f, 0.02f, 0.45f),
                new Color(0.93f, 0.93f, 0.91f));
        }
    }

    private void CreateStoreRow(Transform root)
    {
        CreateStorefront(
            root,
            "FishShop",
            new Vector3(-12.5f, 0f, -13.5f),
            90f,
            new Color(0.22f, 0.47f, 0.50f),
            new Color(0.96f, 0.56f, 0.40f),
            CreateFishSign);

        CreateStorefront(
            root,
            "ClothingStore",
            new Vector3(-12.5f, 0f, 13.5f),
            90f,
            new Color(0.74f, 0.60f, 0.45f),
            new Color(0.92f, 0.80f, 0.67f),
            CreateClothingSign);

        CreateStorefront(
            root,
            "MusicGearStore",
            new Vector3(12.5f, 0f, -13.5f),
            -90f,
            new Color(0.24f, 0.29f, 0.39f),
            new Color(0.25f, 0.78f, 0.77f),
            CreateSpeakerSign);

        CreateStorefront(
            root,
            "Club",
            new Vector3(12.5f, 0f, 13.5f),
            -90f,
            new Color(0.14f, 0.12f, 0.20f),
            new Color(0.95f, 0.18f, 0.58f),
            CreateClubSign);
    }

    private void CreateStorefront(
        Transform parent,
        string storeName,
        Vector3 position,
        float yRotation,
        Color wallColor,
        Color accentColor,
        System.Action<Transform> signBuilder)
    {
        GameObject storeRoot = new GameObject(storeName);
        storeRoot.transform.SetParent(parent, false);
        storeRoot.transform.localPosition = position;
        storeRoot.transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);

        CreatePrimitive(
            PrimitiveType.Cube,
            "BuildingBody",
            storeRoot.transform,
            new Vector3(0f, 3.2f, 0f),
            Vector3.zero,
            new Vector3(7.4f, 6.4f, 8.8f),
            wallColor);

        CreatePrimitive(
            PrimitiveType.Cube,
            "RoofCap",
            storeRoot.transform,
            new Vector3(0f, 6.75f, -0.2f),
            Vector3.zero,
            new Vector3(8f, 0.45f, 9.3f),
            new Color(
                Mathf.Clamp01(wallColor.r * 0.7f),
                Mathf.Clamp01(wallColor.g * 0.7f),
                Mathf.Clamp01(wallColor.b * 0.7f)));

        CreatePrimitive(
            PrimitiveType.Cube,
            "StorefrontFrame",
            storeRoot.transform,
            new Vector3(0f, 2.55f, 4.2f),
            Vector3.zero,
            new Vector3(7.0f, 3.8f, 0.3f),
            accentColor);

        CreatePrimitive(
            PrimitiveType.Cube,
            "LeftWindow",
            storeRoot.transform,
            new Vector3(-2.05f, 2.45f, 4.38f),
            Vector3.zero,
            new Vector3(1.8f, 2.2f, 0.08f),
            new Color(0.66f, 0.87f, 0.94f));

        CreatePrimitive(
            PrimitiveType.Cube,
            "RightWindow",
            storeRoot.transform,
            new Vector3(2.05f, 2.45f, 4.38f),
            Vector3.zero,
            new Vector3(1.8f, 2.2f, 0.08f),
            new Color(0.66f, 0.87f, 0.94f));

        CreatePrimitive(
            PrimitiveType.Cube,
            "Door",
            storeRoot.transform,
            new Vector3(0f, 1.35f, 4.42f),
            Vector3.zero,
            new Vector3(1.1f, 2.7f, 0.12f),
            new Color(0.13f, 0.11f, 0.10f));

        CreatePrimitive(
            PrimitiveType.Cube,
            "Awning",
            storeRoot.transform,
            new Vector3(0f, 4.55f, 4.48f),
            new Vector3(18f, 0f, 0f),
            new Vector3(6.8f, 0.2f, 1.4f),
            accentColor);

        CreatePrimitive(
            PrimitiveType.Cube,
            "SignPanel",
            storeRoot.transform,
            new Vector3(0f, 5.6f, 4.42f),
            Vector3.zero,
            new Vector3(3.8f, 1.9f, 0.2f),
            new Color(0.96f, 0.93f, 0.88f));

        Transform signRoot = new GameObject("IconSign").transform;
        signRoot.SetParent(storeRoot.transform, false);
        signRoot.localPosition = new Vector3(0f, 5.6f, 4.58f);
        signRoot.localRotation = Quaternion.identity;
        signBuilder(signRoot);

        CreatePrimitive(
            PrimitiveType.Cube,
            "BackAccent",
            storeRoot.transform,
            new Vector3(0f, 3f, -4.25f),
            Vector3.zero,
            new Vector3(7.1f, 5.6f, 0.2f),
            new Color(0.12f, 0.13f, 0.15f));
    }

    private void CreateStreetProps(Transform root)
    {
        CreateLampPair(root, -5.2f);
        CreateLampPair(root, 0f);
        CreateLampPair(root, 5.2f);

        CreatePlanter(root, new Vector3(-9.4f, 0.4f, 0f));
        CreatePlanter(root, new Vector3(9.4f, 0.4f, 0f));
        CreatePlanter(root, new Vector3(-9.4f, 0.4f, -24f));
        CreatePlanter(root, new Vector3(9.4f, 0.4f, 24f));

        CreatePrimitive(
            PrimitiveType.Cube,
            "ClubNeonStrip",
            root,
            new Vector3(10.75f, 5.6f, 13.5f),
            Vector3.zero,
            new Vector3(0.18f, 2.6f, 7.5f),
            new Color(0.12f, 0.87f, 0.88f));

        CreatePrimitive(
            PrimitiveType.Cube,
            "FishShopCrate",
            root,
            new Vector3(-9.6f, 0.6f, -16.6f),
            Vector3.zero,
            new Vector3(1.2f, 1.2f, 1.2f),
            new Color(0.50f, 0.34f, 0.18f));

        CreatePrimitive(
            PrimitiveType.Cube,
            "MusicStorePedestal",
            root,
            new Vector3(9.8f, 0.5f, -16.1f),
            Vector3.zero,
            new Vector3(1.5f, 1f, 1.5f),
            new Color(0.17f, 0.20f, 0.25f));
    }

    private void CreateLampPair(Transform root, float zPosition)
    {
        CreateLamp(root, new Vector3(-6.4f, 0f, zPosition));
        CreateLamp(root, new Vector3(6.4f, 0f, zPosition));
    }

    private void CreateLamp(Transform parent, Vector3 position)
    {
        GameObject lampRoot = new GameObject($"Lamp_{position.x}_{position.z}");
        lampRoot.transform.SetParent(parent, false);
        lampRoot.transform.localPosition = position;

        CreatePrimitive(
            PrimitiveType.Cylinder,
            "Pole",
            lampRoot.transform,
            new Vector3(0f, 2.4f, 0f),
            Vector3.zero,
            new Vector3(0.12f, 2.4f, 0.12f),
            new Color(0.16f, 0.17f, 0.18f));

        CreatePrimitive(
            PrimitiveType.Sphere,
            "Light",
            lampRoot.transform,
            new Vector3(0f, 4.95f, 0f),
            Vector3.zero,
            new Vector3(0.55f, 0.55f, 0.55f),
            new Color(1.0f, 0.88f, 0.66f));
    }

    private void CreatePlanter(Transform parent, Vector3 position)
    {
        GameObject planterRoot = new GameObject($"Planter_{position.x}_{position.z}");
        planterRoot.transform.SetParent(parent, false);
        planterRoot.transform.localPosition = position;

        CreatePrimitive(
            PrimitiveType.Cube,
            "Base",
            planterRoot.transform,
            Vector3.zero,
            Vector3.zero,
            new Vector3(1.8f, 0.8f, 1.8f),
            new Color(0.34f, 0.28f, 0.22f));

        CreatePrimitive(
            PrimitiveType.Cylinder,
            "Bush",
            planterRoot.transform,
            new Vector3(0f, 0.9f, 0f),
            Vector3.zero,
            new Vector3(0.6f, 0.7f, 0.6f),
            new Color(0.23f, 0.49f, 0.26f));
    }

    private void CreateFishSign(Transform parent)
    {
        CreatePrimitive(
            PrimitiveType.Sphere,
            "Body",
            parent,
            new Vector3(0f, 0f, 0f),
            Vector3.zero,
            new Vector3(1.25f, 0.68f, 0.34f),
            new Color(0.95f, 0.58f, 0.42f));

        CreatePrimitive(
            PrimitiveType.Cube,
            "Tail",
            parent,
            new Vector3(-0.78f, 0f, 0f),
            new Vector3(0f, 0f, 45f),
            new Vector3(0.34f, 0.52f, 0.14f),
            new Color(0.91f, 0.50f, 0.34f));

        CreatePrimitive(
            PrimitiveType.Cube,
            "Fin",
            parent,
            new Vector3(0.05f, -0.24f, 0f),
            new Vector3(0f, 0f, 25f),
            new Vector3(0.44f, 0.14f, 0.08f),
            new Color(0.89f, 0.45f, 0.31f));

        CreatePrimitive(
            PrimitiveType.Sphere,
            "Eye",
            parent,
            new Vector3(0.4f, 0.1f, 0.16f),
            Vector3.zero,
            new Vector3(0.09f, 0.09f, 0.09f),
            new Color(0.08f, 0.08f, 0.09f));
    }

    private void CreateClothingSign(Transform parent)
    {
        CreatePrimitive(
            PrimitiveType.Cube,
            "Torso",
            parent,
            new Vector3(0f, -0.05f, 0f),
            Vector3.zero,
            new Vector3(0.82f, 0.86f, 0.18f),
            new Color(0.25f, 0.71f, 0.87f));

        CreatePrimitive(
            PrimitiveType.Cube,
            "LeftSleeve",
            parent,
            new Vector3(-0.58f, 0.18f, 0f),
            new Vector3(0f, 0f, 28f),
            new Vector3(0.48f, 0.30f, 0.18f),
            new Color(0.25f, 0.71f, 0.87f));

        CreatePrimitive(
            PrimitiveType.Cube,
            "RightSleeve",
            parent,
            new Vector3(0.58f, 0.18f, 0f),
            new Vector3(0f, 0f, -28f),
            new Vector3(0.48f, 0.30f, 0.18f),
            new Color(0.25f, 0.71f, 0.87f));

        CreatePrimitive(
            PrimitiveType.Cube,
            "Collar",
            parent,
            new Vector3(0f, 0.4f, 0f),
            Vector3.zero,
            new Vector3(0.26f, 0.12f, 0.2f),
            new Color(0.94f, 0.94f, 0.96f));
    }

    private void CreateSpeakerSign(Transform parent)
    {
        CreatePrimitive(
            PrimitiveType.Cube,
            "Cabinet",
            parent,
            new Vector3(0f, 0f, 0f),
            Vector3.zero,
            new Vector3(0.82f, 1.32f, 0.18f),
            new Color(0.14f, 0.15f, 0.18f));

        CreatePrimitive(
            PrimitiveType.Cylinder,
            "Woofer",
            parent,
            new Vector3(0f, -0.26f, 0.12f),
            new Vector3(90f, 0f, 0f),
            new Vector3(0.28f, 0.04f, 0.28f),
            new Color(0.27f, 0.86f, 0.83f));

        CreatePrimitive(
            PrimitiveType.Cylinder,
            "Tweeter",
            parent,
            new Vector3(0f, 0.34f, 0.12f),
            new Vector3(90f, 0f, 0f),
            new Vector3(0.17f, 0.04f, 0.17f),
            new Color(0.85f, 0.95f, 0.99f));
    }

    private void CreateClubSign(Transform parent)
    {
        CreatePrimitive(
            PrimitiveType.Cylinder,
            "Record",
            parent,
            new Vector3(0f, 0f, 0f),
            new Vector3(90f, 0f, 0f),
            new Vector3(0.58f, 0.05f, 0.58f),
            new Color(0.08f, 0.08f, 0.12f));

        CreatePrimitive(
            PrimitiveType.Cylinder,
            "RecordRing",
            parent,
            new Vector3(0f, 0f, 0.02f),
            new Vector3(90f, 0f, 0f),
            new Vector3(0.42f, 0.02f, 0.42f),
            new Color(0.97f, 0.19f, 0.58f));

        CreatePrimitive(
            PrimitiveType.Cylinder,
            "Center",
            parent,
            new Vector3(0f, 0f, 0.05f),
            new Vector3(90f, 0f, 0f),
            new Vector3(0.12f, 0.03f, 0.12f),
            new Color(0.16f, 0.90f, 0.87f));

        CreatePrimitive(
            PrimitiveType.Cube,
            "Needle",
            parent,
            new Vector3(0.52f, 0.42f, 0f),
            new Vector3(0f, 0f, -28f),
            new Vector3(0.62f, 0.07f, 0.07f),
            new Color(0.93f, 0.93f, 0.95f));
    }

    private GameObject CreatePrimitive(
        PrimitiveType primitiveType,
        string objectName,
        Transform parent,
        Vector3 localPosition,
        Vector3 localEulerAngles,
        Vector3 localScale,
        Color color)
    {
        GameObject primitive = GameObject.CreatePrimitive(primitiveType);
        primitive.name = objectName;
        primitive.transform.SetParent(parent, false);
        primitive.transform.localPosition = localPosition;
        primitive.transform.localEulerAngles = localEulerAngles;
        primitive.transform.localScale = localScale;

        Renderer renderer = primitive.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            renderer.receiveShadows = true;
            renderer.sharedMaterial = CreateMaterial(color);
        }

        return primitive;
    }

    private Material CreateMaterial(Color color)
    {
        Color32 colorKey = color;

        if (materialCache.TryGetValue(colorKey, out Material cachedMaterial))
        {
            return cachedMaterial;
        }

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");

        if (shader == null)
        {
            shader = Shader.Find("Standard");
        }

        Material material = new Material(shader);
        material.hideFlags = HideFlags.HideAndDontSave;

        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", color);
        }

        if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", color);
        }

        materialCache[colorKey] = material;
        return material;
    }

    private void ClearMaterialCache()
    {
        foreach (Material material in materialCache.Values)
        {
            DestroySmart(material);
        }

        materialCache.Clear();
    }

    private void DestroySmart(Object target)
    {
        if (target == null)
        {
            return;
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            DestroyImmediate(target);
            return;
        }
#endif

        Destroy(target);
    }
}
