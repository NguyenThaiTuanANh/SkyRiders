using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple player health for receiving enemy damage and collisions.
/// Includes Low HP warning effect when HP <= 30%.
/// — Adds flashing health bar effect when HP < 30%.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;

    [Header("Effects")]
    public GameObject explosionFX;
    public GameObject hpLowFX;

    [Header("UI")]
    public Slider healthBar;

    [Header("Low HP Flash Settings")]
    public float lowHpThreshold = 0.3f;
    public float flashSpeed = 5f;

    private float currentHealth;
    private bool dead;
    private bool isLowHP;

    private GameObject lowHpInstance;

    // Flash variables
    private Image fillImage;
    private Color originalColor;
    private Coroutine flashCoroutine;

    void Awake()
    {
        ResetHealth();

        // Cache fill image của Slider
        if (healthBar != null)
        {
            fillImage = healthBar.fillRect.GetComponent<Image>();
            originalColor = fillImage.color;
        }

        if (hpLowFX != null)
        {
            lowHpInstance = Instantiate(hpLowFX, transform);
            lowHpInstance.SetActive(false);
        }
    }

    public void ResetHealth()
    {
        dead = false;
        isLowHP = false;

        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = maxHealth;
            healthBar.enabled = true;
        }

        if (lowHpInstance != null)
            lowHpInstance.SetActive(false);

        StopFlash();
    }

    public void TakeDamage(float amount)
    {
        if (dead || amount <= 0f) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        UpdateHealthUI();
        CheckLowHP();

        if (currentHealth <= 0f)
            Die();
    }

    private void UpdateHealthUI()
    {
        if (healthBar != null)
            healthBar.value = currentHealth;
    }

    private void CheckLowHP()
    {
        float percent = currentHealth / maxHealth;

        if (percent <= lowHpThreshold && !isLowHP)
        {
            isLowHP = true;

            if (lowHpInstance != null)
                lowHpInstance.SetActive(true);

            StartFlash();
        }
        else if (percent > lowHpThreshold && isLowHP)
        {
            isLowHP = false;

            if (lowHpInstance != null)
                lowHpInstance.SetActive(false);

            StopFlash();
        }
    }

    #region Flash Health Bar

    private void StartFlash()
    {
        if (flashCoroutine == null && fillImage != null)
            flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private void StopFlash()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        if (fillImage != null)
            fillImage.color = originalColor;
    }

    private IEnumerator FlashRoutine()
    {
        while (true)
        {
            float t = Mathf.PingPong(Time.time * flashSpeed, 1f);
            fillImage.color = Color.Lerp(originalColor, Color.red, t);
            yield return null;
        }
    }

    #endregion

    private void Die()
    {
        if (dead) return;
        dead = true;

        StopFlash();

        if (explosionFX != null)
        {
            GameObject vfx = Instantiate(explosionFX, transform.position, Quaternion.identity);
            Destroy(vfx, 2f);
        }

        if (LevelManager.Instance != null)
            LevelManager.Instance.GameOver();

        Destroy(gameObject);
    }

    public void Heal(float amount)
    {
        if (dead || amount <= 0f) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        UpdateHealthUI();
        CheckLowHP();
    }
}