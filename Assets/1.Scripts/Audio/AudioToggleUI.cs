using UnityEngine;
using UnityEngine.UI;

public class AudioToggleUI : MonoBehaviour
{
    public enum AudioType
    {
        BGM,
        SFX
    }

    [Header("Type")]
    public AudioType audioType;

    [Header("UI")]
    [SerializeField] private GameObject toggleOn;
    [SerializeField] private GameObject toggleOff;
    [SerializeField] private Button button;

    private bool isOn = true;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        button.onClick.AddListener(OnToggleClick);
    }

    private void Start()
    {
        // Init theo trạng thái AudioManager
        if (audioType == AudioType.BGM)
            isOn = AudioManager.Instance.BGMEnabled;
        else
            isOn = AudioManager.Instance.SFXEnabled;

        UpdateVisual();
    }

    private void OnToggleClick()
    {
        isOn = !isOn;

        if (audioType == AudioType.BGM)
        {
            AudioManager.Instance.SetBGMEnabled(isOn);
        }
        else
        {
            AudioManager.Instance.SetSFXEnabled(isOn);
        }

        UpdateVisual();
    }

    private void UpdateVisual()
    {
        toggleOn.SetActive(isOn);
        toggleOff.SetActive(!isOn);
    }
}
