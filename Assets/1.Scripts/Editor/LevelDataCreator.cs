using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor utility: tạo sẵn SO_LevelData cho từng level (sceneName, rewardCoin, enemyData placeholder).
/// Chạy menu: Game > Create Default Level Data.
/// Sau khi tạo, gán SO_LevelDatabase vào LevelManager/LevelEnemySpawner và gán prefab enemy vào từng SO_LevelData.enemyData.
/// </summary>
public static class LevelDataCreator
{
    private const string DataFolder = "Assets/1.Scripts/Data";
    private const string LevelDataPrefix = "SO_LevelData_Level";

    [MenuItem("Game/Create Default Level Data")]
    public static void CreateDefaultLevelData()
    {
        // Cấu hình level: sceneName, rewardCoin, enemy type + amount (prefab gán sau trong Inspector)
        var configs = new[]
        {
            new { levelIndex = 1, sceneName = "GamePlay_Winter",  rewardCoin = 100, soldiers = 2, tanks = 0, containers = 0 },
            new { levelIndex = 2, sceneName = "GamePlay_Woodland", rewardCoin = 150, soldiers = 3, tanks = 1, containers = 0 },
            new { levelIndex = 3, sceneName = "GamePlay_Desert",   rewardCoin = 200, soldiers = 4, tanks = 1, containers = 1 },
        };

        if (!AssetDatabase.IsValidFolder("Assets/1.Scripts"))
        {
            Debug.LogWarning("Folder Assets/1.Scripts not found. Create level data manually.");
            return;
        }
        if (!AssetDatabase.IsValidFolder("Assets/1.Scripts/Editor"))
        {
            AssetDatabase.CreateFolder("Assets/1.Scripts", "Editor");
        }

        GameObject soldierPrefab = FindPrefabByEnemyType(EnemyType.Soldier);
        GameObject tankPrefab = FindPrefabByEnemyType(EnemyType.Tank);
        GameObject containerPrefab = FindPrefabByEnemyType(EnemyType.Container);

        var createdLevels = new System.Collections.Generic.List<SO_LevelData>();

        foreach (var c in configs)
        {
            string assetPath = $"{DataFolder}/{LevelDataPrefix}{c.levelIndex}.asset";
            SO_LevelData existing = AssetDatabase.LoadAssetAtPath<SO_LevelData>(assetPath);
            SO_LevelData levelData = existing != null ? existing : ScriptableObject.CreateInstance<SO_LevelData>();

            levelData.levelIndex = c.levelIndex;
            levelData.sceneName = c.sceneName;
            levelData.rewardCoin = c.rewardCoin;
            levelData.enemyData.Clear();

            if (c.soldiers > 0 && soldierPrefab != null)
                levelData.enemyData.Add(new LevelEnemyEntry { enemyType = EnemyType.Soldier, enemyPrefab = soldierPrefab, amount = c.soldiers });
            else if (c.soldiers > 0)
                levelData.enemyData.Add(new LevelEnemyEntry { enemyType = EnemyType.Soldier, enemyPrefab = null, amount = c.soldiers });

            if (c.tanks > 0 && tankPrefab != null)
                levelData.enemyData.Add(new LevelEnemyEntry { enemyType = EnemyType.Tank, enemyPrefab = tankPrefab, amount = c.tanks });
            else if (c.tanks > 0)
                levelData.enemyData.Add(new LevelEnemyEntry { enemyType = EnemyType.Tank, enemyPrefab = null, amount = c.tanks });

            if (c.containers > 0 && containerPrefab != null)
                levelData.enemyData.Add(new LevelEnemyEntry { enemyType = EnemyType.Container, enemyPrefab = containerPrefab, amount = c.containers });
            else if (c.containers > 0)
                levelData.enemyData.Add(new LevelEnemyEntry { enemyType = EnemyType.Container, enemyPrefab = null, amount = c.containers });

            if (existing == null)
            {
                AssetDatabase.CreateAsset(levelData, assetPath);
                createdLevels.Add(levelData);
            }
            else
            {
                EditorUtility.SetDirty(levelData);
            }
        }

        SO_LevelDatabase database = FindOrCreateLevelDatabase();
        if (database != null)
        {
            database.levels.Clear();
            for (int i = 0; i < configs.Length; i++)
            {
                string path = $"{DataFolder}/{LevelDataPrefix}{configs[i].levelIndex}.asset";
                var ld = AssetDatabase.LoadAssetAtPath<SO_LevelData>(path);
                if (ld != null) database.levels.Add(ld);
            }
            EditorUtility.SetDirty(database);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Level Data: Created/updated {configs.Length} levels. Assign enemy prefabs in SO_LevelData if needed.");
    }

    private static GameObject FindPrefabByEnemyType(EnemyType enemyType)
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;
            var eb = prefab.GetComponent<EnemyBase>();
            if (eb != null && eb.enemyType == enemyType)
                return prefab;
        }
        return null;
    }

    private static SO_LevelDatabase FindOrCreateLevelDatabase()
    {
        string[] guids = AssetDatabase.FindAssets("t:SO_LevelDatabase");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<SO_LevelDatabase>(path);
        }
        var db = ScriptableObject.CreateInstance<SO_LevelDatabase>();
        AssetDatabase.CreateAsset(db, $"{DataFolder}/SO_LevelDatabase.asset");
        return db;
    }
}
