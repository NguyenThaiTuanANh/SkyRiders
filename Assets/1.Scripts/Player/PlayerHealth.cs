using UnityEngine;

/// <summary>
/// Simple player health for receiving enemy damage and collisions.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public GameObject explosionFX;

    private float currentHealth;
    private bool dead;

    void Awake()
    {
        ResetHealth();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        dead = false;
    }

    public void TakeDamage(float amount)
    {
        if (dead || amount <= 0f) return;
        currentHealth -= amount;
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (dead) return;
        dead = true;

        if (explosionFX != null)
        {
            GameObject vfx = Instantiate(explosionFX, transform.position, Quaternion.identity);
            Destroy(vfx, 2f);
        }

        if (LevelManager.Instance != null)
            LevelManager.Instance.GameOver();

        Destroy(gameObject);
    }
}
