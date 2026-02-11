using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Database tất cả level. Level number 1-based (level 1 = index 0).
/// </summary>
[CreateAssetMenu(fileName = "SO_LevelDatabase", menuName = "Game/SO_LevelDatabase")]
public class SO_LevelDatabase : ScriptableObject
{
    public List<SO_LevelData> levels = new List<SO_LevelData>();

    /// <summary>
    /// Lấy SO_LevelData theo level number (1-based). Trả về null nếu không hợp lệ.
    /// </summary>
    public SO_LevelData GetLevel(int levelNumber)
    {
        if (levels == null || levelNumber < 1 || levelNumber > levels.Count)
            return null;
        return levels[levelNumber - 1];
    }

    /// <summary>
    /// Lấy level theo tên scene. Trả về null nếu không tìm thấy.
    /// </summary>
    public SO_LevelData GetLevelBySceneName(string sceneName)
    {
        if (levels == null || string.IsNullOrEmpty(sceneName)) return null;
        foreach (var level in levels)
        {
            if (level != null && level.sceneName == sceneName)
                return level;
        }
        return null;
    }
}
