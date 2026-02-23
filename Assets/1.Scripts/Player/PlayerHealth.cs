using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple player health for receiving enemy damage and collisions.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public GameObject explosionFX;

    private float currentHealth;
    private bool dead;

    public Slider healthBar;
    //public Slider fakeBar;

    void Awake()
    {
        ResetHealth();
    }

    public void ResetHealth()
    {
        //currentHealth = maxHealth;
        dead = false;
        SetHealth(maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (dead || amount <= 0f) return;
        currentHealth -= amount;
        SetFill(currentHealth);
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

    public void SetHealth(float health)
    {
        currentHealth = health;
        healthBar.maxValue = health;
        healthBar.value = health;
        healthBar.enabled = true;
        //fakeBar.maxValue = health;
        //fakeBar.value = health;
        //fakeBar.enabled = true;
    }

    public void SetFill(float Health)
    {
        currentHealth = Health;

        healthBar.value = currentHealth;
        if (healthBar.value <= 0) healthBar.value = 0;
        //StartCoroutine(FakeBarWaitCO());

    }


    //IEnumerator FakeBarWaitCO()
    //{
    //    yield return new WaitForSeconds(0.3f);
    //    fakeBar.value = currentHealth;
    //}
}
