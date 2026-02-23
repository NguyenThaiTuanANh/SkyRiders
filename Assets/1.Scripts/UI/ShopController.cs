using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Điều khiển Shop: hiển thị danh sách skin, mua (trừ coin + mở khóa), chọn skin dùng.
/// Gắn vào popup Shop, gán SO_PlaneSkinDatabase và danh sách ShopSkinSlot (5 ô).
/// </summary>
public class ShopController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private SO_PlaneSkinDatabase skinDatabase;

    [Header("UI Slots")]
    [Tooltip("5 ô skin (theo thứ tự hoặc gán tay).")]
    [SerializeField] private List<ShopSkinSlot> skinSlots = new List<ShopSkinSlot>();

    [Header("Feedback")]
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private float messageDuration = 2f;

    [Header("Coin (optional)")]
    [SerializeField] private TMP_Text coinText;

    private float messageHideTime;

    void Start()
    {
        RefreshAllSlots();
        RefreshCoin();
    }

    void OnEnable()
    {
        RefreshAllSlots();
        RefreshCoin();
    }

    void Update()
    {
        if (messageText != null && messageText.gameObject.activeSelf && Time.unscaledTime >= messageHideTime)
            messageText.gameObject.SetActive(false);
    }

    public void RefreshAllSlots()
    {
        if (skinDatabase == null || skinDatabase.skins == null) return;

        for (int i = 0; i < skinDatabase.skins.Count && i < skinSlots.Count; i++)
        {
            ShopSkinSlot slot = skinSlots[i];
            PlaneSkinData skin = skinDatabase.skins[i];
            if (slot == null || skin == null) continue;

            bool unlocked = SaveLoadManager.Instance != null && SaveLoadManager.Instance.IsSkinUnlocked(skin.skinId);
            bool isCurrent = SaveLoadManager.Instance != null && SaveLoadManager.Instance.CurrentSkinId == skin.skinId;
            slot.SetData(skin, unlocked, isCurrent);
        }
    }

    public void RefreshCoin()
    {
        if (coinText != null && SaveLoadManager.Instance != null)
            coinText.text = SaveLoadManager.Instance.Coin.ToString();
    }

    /// <summary>
    /// Mua skin (trừ coin và mở khóa). Gọi từ ShopSkinSlot.
    /// </summary>
    public void TryBuySkin(int skinId)
    {
        if (SaveLoadManager.Instance == null || skinDatabase == null)
        {
            ShowMessage("Lỗi dữ liệu.");
            return;
        }

        if (SaveLoadManager.Instance.IsSkinUnlocked(skinId))
        {
            ShowMessage("Skin đã mở khóa.");
            return;
        }

        PlaneSkinData skin = skinDatabase.GetSkin(skinId);
        if (skin == null)
        {
            ShowMessage("Skin không tồn tại.");
            return;
        }

        int price = skin.price;
        if (SaveLoadManager.Instance.Coin < price)
        {
            ShowMessage("Không đủ coin!");
            return;
        }

        if (!SaveLoadManager.Instance.SpendCoin(price))
        {
            ShowMessage("Không đủ coin!");
            return;
        }

        SaveLoadManager.Instance.UnlockSkin(skinId);
        
        // Tự động set làm skin hiện tại khi mua thành công
        if (SaveLoadManager.Instance.SetCurrentSkin(skinId))
        {
            // Áp dụng skin vào gameplay nếu đang chơi
            if (AirplaneController.Instance != null)
                AirplaneController.Instance.ApplyCurrentSkin();
        }

        RefreshAllSlots();
        RefreshCoin();
        if (UIMainMenu.Instance != null)
            UIMainMenu.Instance.UpdateCoinUI();
        ShowMessage("Mở khóa thành công!");
    }

    /// <summary>
    /// Chọn skin đang dùng. Gọi từ ShopSkinSlot.
    /// </summary>
    public void UseSkin(int skinId)
    {
        if (SaveLoadManager.Instance == null) return;
        if (SaveLoadManager.Instance.SetCurrentSkin(skinId))
        {
            // Áp dụng skin vào gameplay nếu đang chơi
            if (AirplaneController.Instance != null)
                AirplaneController.Instance.ApplyCurrentSkin();

            RefreshAllSlots();
            ShowMessage("Đã đổi skin!");
        }
    }

    void ShowMessage(string msg)
    {
        if (messageText != null)
        {
            messageText.text = msg;
            messageText.gameObject.SetActive(true);
            messageHideTime = Time.unscaledTime + messageDuration;
        }
    }
}
