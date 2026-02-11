using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Simple world-space health bar. Set as child of enemy, assign fill transform.
/// </summary>
public class EnemyHealthBar : MonoBehaviour
{
    //[Tooltip("Transform whose localScale.x represents fill (0-1).")]
    //public Transform fill;

    ////[Tooltip("Optional: camera to face. If empty, will grab main camera.")]
    ////public Camera faceCamera;

    //void Awake()
    //{
    //    //if (faceCamera == null && Camera.main != null)
    //    //    faceCamera = Camera.main;
    //}

    //public void SetFill(float current, float max)
    //{
    //    if (fill == null || max <= 0f) return;
    //    float t = Mathf.Clamp01(current / max);
    //    Vector3 scale = fill.localScale;
    //    scale.x = t;
    //    fill.localScale = scale;
    //}

    //void LateUpdate()
    //{
    //    //if (faceCamera == null) return;
    //    //transform.forward = faceCamera.transform.forward;
    //}


    public Slider healthBar;
    public Slider fakeBar;
    private float currentHealth;

    public void SetHealth(float health)
    {
        currentHealth = health;
        healthBar.maxValue = health;
        healthBar.value = health;
        healthBar.enabled = true;
        fakeBar.maxValue = health;
        fakeBar.value = health;
        fakeBar.enabled = true;
    }

    public void SetFill(float Health)
    {
        currentHealth = Health;

        healthBar.value = currentHealth;
        if (healthBar.value <= 0) healthBar.value = 0;
        StartCoroutine(FakeBarWaitCO());

    }


    IEnumerator FakeBarWaitCO()
    {
        yield return new WaitForSeconds(0.3f);
        fakeBar.value = currentHealth;
    }
}
