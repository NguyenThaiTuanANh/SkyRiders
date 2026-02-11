using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fallback config khi không dùng SO_LevelData: kích hoạt enemy pre-placed trong scene theo level.
/// </summary>
[CreateAssetMenu(fileName = "LevelEnemyConfig", menuName = "Configs/Level Enemy Config")]
public class LevelEnemyConfig : ScriptableObject
{
    [Tooltip("Logical level index that this config represents.")]
    public int levelIndex = 1;

    [Tooltip("Assign pre-placed enemy objects in the scene to activate for this level.")]
    public List<EnemyEntry> enemies = new List<EnemyEntry>();
}

[System.Serializable]
public class EnemyEntry
{
    public EnemyType type;
    public GameObject enemyObject;
}
