using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Một ô skin trong Shop: icon, tên, giá, nút Mua / Dùng.
/// </summary>
public class ShopSkinSlot : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private GameObject lockOverlay;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button useButton;
    [SerializeField] private TMP_Text useButtonText;
    [SerializeField] private GameObject selectedBadge;

    private int skinId;
    private PlaneSkinData data;

    void Awake()
    {
        if (buyButton != null) buyButton.onClick.AddListener(OnBuyClick);
        if (useButton != null) useButton.onClick.AddListener(OnUseClick);
    }

    /// <summary>
    /// Gán dữ liệu skin và trạng thái (đã mở khóa, đang dùng).
    /// </summary>
    public void SetData(PlaneSkinData skinData, bool unlocked, bool isCurrent)
    {
        if (skinData == null) return;

        data = skinData;
        skinId = skinData.skinId;

        if (nameText != null) nameText.text = skinData.skinName;
        if (priceText != null) priceText.text = skinData.price <= 0 ? "FREE" : skinData.price.ToString();
        if (iconImage != null && skinData.icon != null) iconImage.sprite = skinData.icon;

        if (lockOverlay != null) lockOverlay.SetActive(!unlocked);
        if (buyButton != null) buyButton.gameObject.SetActive(!unlocked);
        if (useButton != null) useButton.gameObject.SetActive(unlocked);
        if (useButtonText != null) useButtonText.text = isCurrent ? "Đang dùng" : "Dùng";
        if (useButton != null) useButton.interactable = !isCurrent;
        if (selectedBadge != null) selectedBadge.SetActive(isCurrent);
    }

    void OnBuyClick()
    {
        ShopController shop = GetComponentInParent<ShopController>();
        if (shop != null)
            shop.TryBuySkin(skinId);
    }

    void OnUseClick()
    {
        ShopController shop = GetComponentInParent<ShopController>();
        if (shop != null)
            shop.UseSkin(skinId);
    }
}
