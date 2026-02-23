using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    private readonly List<EnemyBase> enemies = new List<EnemyBase>();
    private bool suppressCompletion;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // ================= SYSTEM =================

    public void Register(EnemyBase enemy)
    {
        if (!enemies.Contains(enemy))
            enemies.Add(enemy);
    }

    public void Unregister(EnemyBase enemy)
    {
        if (enemies.Contains(enemy))
            enemies.Remove(enemy);

        if (!suppressCompletion && enemies.Count == 0 && LevelManager.Instance != null)
        {
            LevelManager.Instance.LevelComplete();
        }
    }

    public int AliveCount()
    {
        return enemies.Count;
    }

    /// <summary>
    /// Used by spawner to avoid triggering LevelComplete while resetting enemies.
    /// Call EndBatch after enabling new enemies.
    /// </summary>
    public void BeginBatch()
    {
        suppressCompletion = true;
        enemies.Clear();
    }

    public void EndBatch()
    {
        suppressCompletion = false;
        Debug.Log("LEVel" + enemies.Count);
        if (enemies.Count == 0 && LevelManager.Instance != null)
            LevelManager.Instance.LevelComplete(); // Win: hết enemy => hoàn thành level
    }
}
