using UnityEngine;
using TMPro;
using System.Collections;

public class TapToPlayCoroutineEffect : MonoBehaviour
{
    [Header("Scale")]
    public float minScale = 0.9f;
    public float maxScale = 1.1f;
    public float scaleDuration = 0.8f;

    [Header("Fade")]
    public float minAlpha = 0.5f;
    public float maxAlpha = 1f;
    public float fadeDuration = 0.8f;

    private TextMeshProUGUI tmp;
    private Vector3 baseScale;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        baseScale = transform.localScale;
    }

    void OnEnable()
    {
        StartCoroutine(LoopEffect());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator LoopEffect()
    {
        while (true)
        {
            yield return StartCoroutine(Animate(0f, 1f));
            yield return StartCoroutine(Animate(1f, 0f));
        }
    }

    IEnumerator Animate(float from, float to)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / scaleDuration;

            float scale = Mathf.Lerp(minScale, maxScale, Mathf.Lerp(from, to, t));
            transform.localScale = baseScale * scale;

            Color c = tmp.color;
            c.a = Mathf.Lerp(minAlpha, maxAlpha, Mathf.Lerp(from, to, t));
            tmp.color = c;

            yield return null;
        }
    }
}
