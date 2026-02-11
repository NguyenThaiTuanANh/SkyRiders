using UnityEngine;
using DamageNumbersPro;

public class DamageShow : MonoBehaviour
{
    [Header("Damage Numbers Pro")]
    [SerializeField] private DamageNumber damageNumberPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0);

    /// <summary>
    /// Gọi hàm này khi object bị trúng đạn
    /// </summary>
    public void ShowDamage(int damage)
    {
        Vector3 spawnPos = transform.position + offset;
        damageNumberPrefab.Spawn(spawnPos, damage);
    }

    /// <summary>
    /// Gọi khi có hit position (raycast, bullet)
    /// </summary>
    public void ShowDamage(int damage, Vector3 hitPosition)
    {
        damageNumberPrefab.Spawn(hitPosition, damage);
    }
}
