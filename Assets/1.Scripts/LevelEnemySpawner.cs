using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawn point theo từng loại enemy: setup trên scene, mỗi loại có danh sách Transform.
/// </summary>
[System.Serializable]
public class SpawnPointsByEnemyType
{
    public EnemyType enemyType;
    [Tooltip("Các điểm spawn setup sẵn trên scene cho loại enemy này. Spawn lần lượt theo thứ tự.")]
    public List<Transform> points = new List<Transform>();
}

/// <summary>
/// Đọc SO_LevelData theo level hiện tại và spawn enemy từ prefab tại spawn point theo từng dạng enemy.
/// Spawn points được setup theo từng EnemyType trên scene.
/// </summary>
public class LevelEnemySpawner : MonoBehaviour
{
    [Header("Level Data")]
    [Tooltip("Database level. Spawn theo SO_LevelData nếu có.")]
    [SerializeField] private SO_LevelDatabase levelDatabase;

    [Header("Spawn Points Theo Từng Loại Enemy")]
    [Tooltip("Setup sẵn trên scene: mỗi loại enemy có danh sách point riêng. Spawn đúng loại tại đúng point.")]
    [SerializeField] private List<SpawnPointsByEnemyType> spawnPointsByType = new List<SpawnPointsByEnemyType>();

    [Header("Fallback Spawn (khi loại enemy không có point)")]
    [Tooltip("Điểm spawn chung khi loại enemy không có trong spawnPointsByType.")]
    [SerializeField] private List<Transform> fallbackSpawnPoints = new List<Transform>();

    [Tooltip("Khi không có point nào: spawn quanh vị trí này (random trong spawnRadius).")]
    [SerializeField] private Transform spawnAreaCenter;

    [Tooltip("Bán kính random quanh spawnAreaCenter.")]
    [SerializeField] private float spawnRadius = 5f;

    [Header("Fallback - Pre-placed Enemies (cũ)")]
    [Tooltip("Tất cả enemy pre-placed trong scene (mặc định tắt). Dùng khi không dùng SO_LevelData spawn.")]
    [SerializeField] private List<GameObject> allSceneEnemies = new List<GameObject>();

    [Tooltip("Config theo level cho pre-placed. Chỉ dùng khi không có levelData.enemyData.")]
    [SerializeField] private List<LevelEnemyConfig> levelConfigs = new List<LevelEnemyConfig>();

    [Header("Debug")]
    [Tooltip("Override level (0 = dùng LevelManager.levelIndex).")]
    [SerializeField] private int overrideLevelIndex = 0;

    private readonly List<EnemyBase> spawnedEnemies = new List<EnemyBase>();

    private void Start()
    {
        int levelIndex = overrideLevelIndex > 0
            ? overrideLevelIndex
            : (LevelManager.Instance != null ? LevelManager.Instance.levelIndex : 1);

        LoadLevel(levelIndex);
    }

    public void LoadLevel(int levelIndex)
    {
        ClearSpawnedEnemies();

        SO_LevelData levelData = levelDatabase != null ? levelDatabase.GetLevel(levelIndex) : null;

        if (levelData != null && levelData.enemyData != null && levelData.enemyData.Count > 0)
        {
            SpawnFromLevelData(levelData);
            return;
        }

        // Fallback: pre-placed config
        LevelEnemyConfig config = levelConfigs.Find(c => c != null && c.levelIndex == levelIndex);
        if (config != null)
        {
            ApplyPrePlacedConfig(config);
            return;
        }

        DisableAllPrePlaced();
    }

    private void SpawnFromLevelData(SO_LevelData levelData)
    {
        if (EnemyManager.Instance != null)
            EnemyManager.Instance.BeginBatch();

        DisableAllPrePlaced();

        foreach (LevelEnemyEntry entry in levelData.enemyData)
        {
            if (entry == null || entry.enemyPrefab == null || entry.amount <= 0)
                continue;

            for (int i = 0; i < entry.amount; i++)
            {
                Vector3 pos = GetSpawnPositionForType(entry.enemyType, i);
                Quaternion rot = Quaternion.identity;
                GameObject go = Instantiate(entry.enemyPrefab, pos, rot);
                var enemy = go.GetComponent<EnemyBase>();
                if (enemy != null)
                    spawnedEnemies.Add(enemy);
            }
        }

        if (EnemyManager.Instance != null)
            EnemyManager.Instance.EndBatch();
    }

    /// <summary>
    /// Lấy vị trí spawn cho loại enemy tại index thứ i (cùng loại). Ưu tiên spawnPointsByType, rồi fallback, rồi random quanh spawnAreaCenter.
    /// </summary>
    private Vector3 GetSpawnPositionForType(EnemyType enemyType, int index)
    {
        if (spawnPointsByType != null)
        {
            foreach (var group in spawnPointsByType)
            {
                if (group == null || group.enemyType != enemyType || group.points == null || group.points.Count == 0)
                    continue;

                Transform t = group.points[index % group.points.Count];
                if (t != null)
                    return t.position;
                break;
            }
        }

        if (fallbackSpawnPoints != null && fallbackSpawnPoints.Count > 0)
        {
            Transform t = fallbackSpawnPoints[index % fallbackSpawnPoints.Count];
            if (t != null)
                return t.position;
        }

        return GetDefaultSpawnPosition();
    }

    private Vector3 GetDefaultSpawnPosition()
    {
        Vector3 center = spawnAreaCenter != null ? spawnAreaCenter.position : transform.position;
        Vector2 r = Random.insideUnitCircle * spawnRadius;
        return center + new Vector3(r.x, 0f, r.y);
    }

    private void ClearSpawnedEnemies()
    {
        foreach (var e in spawnedEnemies)
        {
            if (e != null && e.gameObject != null)
                Destroy(e.gameObject);
        }
        spawnedEnemies.Clear();
    }

    private void ApplyPrePlacedConfig(LevelEnemyConfig config)
    {
        if (EnemyManager.Instance != null)
            EnemyManager.Instance.BeginBatch();

        foreach (var enemyObj in allSceneEnemies)
        {
            if (enemyObj != null)
                enemyObj.SetActive(false);
        }

        foreach (var entry in config.enemies)
        {
            if (entry == null || entry.enemyObject == null) continue;
            entry.enemyObject.SetActive(true);
        }

        if (EnemyManager.Instance != null)
            EnemyManager.Instance.EndBatch();
    }

    private void DisableAllPrePlaced()
    {
        foreach (var enemyObj in allSceneEnemies)
        {
            if (enemyObj != null)
                enemyObj.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        ClearSpawnedEnemies();
    }
}
