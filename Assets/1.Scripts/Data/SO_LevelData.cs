using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data for one enemy type in a level: type, prefab, and count to spawn.
/// </summary>
[System.Serializable]
public class LevelEnemyEntry
{
    [Tooltip("Enemy type (for UI/validation). Must match prefab's EnemyBase.enemyType.")]
    public EnemyType enemyType = EnemyType.Soldier;

    [Tooltip("Prefab to spawn (must have EnemyBase component).")]
    public GameObject enemyPrefab;

    [Tooltip("Number of this enemy to spawn in the level.")]
    [Min(0)]
    public int amount = 1;
}

/// <summary>
/// ScriptableObject chứa thông tin một level: scene, reward, danh sách enemy spawn.
/// </summary>
[CreateAssetMenu(fileName = "SO_LevelData", menuName = "Game/SO_LevelData")]
public class SO_LevelData : ScriptableObject
{
    [Header("Level Info")]
    [Tooltip("Level number (1-based). Used to match LevelManager.levelIndex.")]
    public int levelIndex = 1;

    [Tooltip("Tên scene gameplay tương ứng (vd: GamePlayScene, Demo_Desert).")]
    public string sceneName = "GamePlayScene";

    [Header("Reward")]
    [Tooltip("Số coin thưởng khi hoàn thành level.")]
    [Min(0)]
    public int rewardCoin = 100;

    [Header("Enemies")]
    [Tooltip("Danh sách loại enemy và số lượng spawn cho level này.")]
    public List<LevelEnemyEntry> enemyData = new List<LevelEnemyEntry>();

    /// <summary>
    /// Tổng số enemy sẽ spawn trong level.
    /// </summary>
    public int TotalEnemyCount
    {
        get
        {
            int total = 0;
            if (enemyData == null) return 0;
            foreach (var entry in enemyData)
                total += entry != null ? entry.amount : 0;
            return total;
        }
    }
}
