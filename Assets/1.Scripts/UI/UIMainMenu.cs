using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private GameObject UiLevelSelect;
    [SerializeField] private GameObject SettingPopup;
    [SerializeField] private GameObject ShopPopup;

    public static UIMainMenu Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        //PlayerData.Instance.LoadData();
        UpdateCoinUI();
        //SaveLevelComplete(3);
        UpdateStatusUi();
    }

    public void UpdateCoinUI()
    {
        if (coinText == null) return;
        int coin = SaveLoadManager.Instance != null ? SaveLoadManager.Instance.Coin : 0;
        coinText.text = coin.ToString();
    }

    private void UpdateStatusUi()
    {
        UiLevelSelect.SetActive(false);
        SettingPopup.SetActive(false);
        ShopPopup.SetActive(false);
    }

    public void OnClickPlay()
    {
        UpdateCoinUI();
        UiLevelSelect.SetActive(true);
    }

    public void OnClickShop()
    {
        UpdateCoinUI();
        ShopPopup.SetActive(true);
    }

    public void OnClickSetting()
    {
        SettingPopup.SetActive(true);
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }

}
