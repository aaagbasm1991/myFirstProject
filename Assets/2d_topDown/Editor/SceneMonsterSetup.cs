using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneMonsterSetup
{
    private const string ScenePath = "Assets/2d_topDown/Scenes/MainScene.unity";
    private const string OrcIdlePath = "Assets/2d_topDown/Art/Tiny RPG Character Asset Pack v1.03 -Free Soldier&Orc/Characters(100x100)/Orc/Orc/Orc-Idle.png";
    private const string OrcWalkPath = "Assets/2d_topDown/Art/Tiny RPG Character Asset Pack v1.03 -Free Soldier&Orc/Characters(100x100)/Orc/Orc/Orc-Walk.png";

    [MenuItem("Tools/2D TopDown/Ensure Orc Monsters")]
    public static void EnsureOrcsInMainScene()
    {
        Texture2D idleSheet = ImportOrcSheet(OrcIdlePath);
        Texture2D walkSheet = ImportOrcSheet(OrcWalkPath);

        if (idleSheet == null || walkSheet == null)
        {
            Debug.LogWarning("Orc monster setup skipped because Orc sprite sheets could not be loaded.");
            return;
        }

        Scene activeScene = SceneManager.GetActiveScene();
        bool openedSetupScene = activeScene.path != ScenePath;
        Scene scene = openedSetupScene
            ? EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single)
            : activeScene;

        CreateOrUpdateOrc("OrcMonster_A", new Vector3(-2.35f, 1.2f, 0f), idleSheet, walkSheet, 1.25f);
        CreateOrUpdateOrc("OrcMonster_B", new Vector3(2.15f, 0.8f, 0f), idleSheet, walkSheet, 1.05f);
        CreateOrUpdateOrc("OrcMonster_C", new Vector3(0.95f, -1.7f, 0f), idleSheet, walkSheet, 1.45f);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        if (openedSetupScene)
        {
            Debug.Log("Orc monsters added to MainScene.");
        }
    }

    private static Texture2D ImportOrcSheet(string path)
    {
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.alphaIsTransparency = true;
            importer.spritePixelsPerUnit = 100f;
            importer.SaveAndReimport();
        }

        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }

    private static void CreateOrUpdateOrc(
        string name,
        Vector3 position,
        Texture2D idleSheet,
        Texture2D walkSheet,
        float patrolRadius
    )
    {
        GameObject orc = GameObject.Find(name);

        if (orc == null)
        {
            orc = new GameObject(name);
        }

        orc.transform.position = position;
        orc.transform.localScale = Vector3.one;

        SpriteRenderer renderer = orc.GetComponent<SpriteRenderer>();

        if (renderer == null)
        {
            renderer = orc.AddComponent<SpriteRenderer>();
        }

        renderer.sortingOrder = 9;

        OrcMonster monster = orc.GetComponent<OrcMonster>();

        if (monster == null)
        {
            monster = orc.AddComponent<OrcMonster>();
        }

        SerializedObject serializedMonster = new SerializedObject(monster);
        serializedMonster.FindProperty("idleSheet").objectReferenceValue = idleSheet;
        serializedMonster.FindProperty("walkSheet").objectReferenceValue = walkSheet;
        serializedMonster.FindProperty("frameWidth").intValue = 100;
        serializedMonster.FindProperty("frameHeight").intValue = 100;
        serializedMonster.FindProperty("animationFrameRate").floatValue = 8f;
        serializedMonster.FindProperty("moveSpeed").floatValue = 1.15f;
        serializedMonster.FindProperty("patrolRadius").floatValue = patrolRadius;
        serializedMonster.FindProperty("waitDuration").floatValue = 1.1f;
        serializedMonster.ApplyModifiedPropertiesWithoutUndo();
    }
}
