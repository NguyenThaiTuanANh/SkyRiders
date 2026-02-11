using System;
using UnityEngine;

/// <summary>
/// Reusable health component used by all enemies (and optionally the player).
/// Keeps simple events for UI bars and death callbacks.
/// </summary>
public class HealthSystem : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    //public EnemyHealthBar enemyHealthBar;

    public Action<float> OnHealthChanged; // current
    public Action OnDeath;

    private float currentHealth;
    private bool isInitialized;

    void Awake()
    {
        //ResetHealth();
    }

    /// <summary>
    /// Resets health to max and notifies listeners.
    /// </summary>
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isInitialized = true;
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void SetMaxHealth(float newMax, bool fillCurrent = true)
    {
        maxHealth = Mathf.Max(1f, newMax);
        if (fillCurrent)
            currentHealth = maxHealth;

        OnHealthChanged?.Invoke(currentHealth);
    }

    public void TakeDamage(float amount)
    {
        if (!isInitialized || amount <= 0f) return;
        if (currentHealth <= 0f) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(0f, currentHealth);
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0f)
        {
            OnDeath?.Invoke();
        }
    }

    public float CurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0f;
}
