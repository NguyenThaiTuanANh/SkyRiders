using System.Collections;
using UnityEngine;
using TMPro;

public class UIMainMenuTitleEffect_Enhanced : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private RectTransform rect;
    [SerializeField] private TextMeshProUGUI text;

    [Header("Intro Scale")]
    [SerializeField] private float startScale = 2.6f;
    [SerializeField] private float endScale = 1f;
    [SerializeField] private float introDuration = 0.6f;

    [Header("Intro Curve (Casual Bounce)")]
    [SerializeField]
    private AnimationCurve introCurve =
        new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.65f, 1.15f),
            new Keyframe(1f, 1f)
        );

    [Header("Intro Rotation")]
    [SerializeField] private float introRotateStrength = 6f;

    [Header("Idle Breathing")]
    [SerializeField] private float idleScale = 1.06f;
    [SerializeField] private float idleSpeed = 1.4f;

    [Header("Idle Rotation")]
    [SerializeField] private float idleRotateStrength = 1.8f;

    [Header("Glow Color")]
    [SerializeField] private Color glowColor = new Color(1f, 0.95f, 0.8f);
    [SerializeField] private float glowSpeed = 1.2f;

    private Color baseColor;

    private void Start()
    {
        baseColor = text.color;
        //PlayIntro();
    }

    public void PlayIntro()
    {
        StopAllCoroutines();
        StartCoroutine(IntroRoutine());
    }

    IEnumerator IntroRoutine()
    {
        rect.localScale = Vector3.one * startScale;
        rect.localRotation = Quaternion.identity;
        text.alpha = 0f;

        float time = 0f;

        while (time < introDuration)
        {
            time += Time.deltaTime;
            float t = time / introDuration;

            float curve = introCurve.Evaluate(t);
            float scale = Mathf.Lerp(startScale, endScale, curve);
            rect.localScale = Vector3.one * scale;

            float rotZ = Mathf.Sin(t * Mathf.PI) * introRotateStrength;
            rect.localRotation = Quaternion.Euler(0, 0, rotZ);

            text.alpha = Mathf.Clamp01(t * 1.2f);
            yield return null;
        }

        rect.localScale = Vector3.one * endScale;
        rect.localRotation = Quaternion.identity;
        text.alpha = 1f;

        // Flash sáng
        yield return StartCoroutine(FlashRoutine());

        StartCoroutine(IdleRoutine());
    }

    IEnumerator FlashRoutine()
    {
        text.color = glowColor;
        yield return new WaitForSeconds(0.06f);
        text.color = baseColor;
    }

    IEnumerator IdleRoutine()
    {
        while (true)
        {
            float sin = Mathf.Sin(Time.time * idleSpeed);
            float scale = Mathf.Lerp(1f, idleScale, (sin + 1f) * 1.5f);
            rect.localScale = Vector3.one * scale;

            float rot = Mathf.Sin(Time.time * 0.8f) * idleRotateStrength;
            rect.localRotation = Quaternion.Euler(0, 0, rot);

            Color c = Color.Lerp(baseColor, glowColor,
                (Mathf.Sin(Time.time * glowSpeed) + 1f) * 0.5f);
            text.color = c;

            yield return null;
        }
    }
}
