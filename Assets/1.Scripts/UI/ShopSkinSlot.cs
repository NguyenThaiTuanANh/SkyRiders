using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Một ô skin trong Shop: icon, tên, giá, nút Mua / Dùng.
/// </summary>
public class ShopSkinSlot : MonoBehaviour
{
    [Header("UI")]
    //[SerializeField] private Image iconImage;
    //[SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private GameObject lockOverlay;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button useButton;
    [SerializeField] private TMP_Text useButtonText;
    [SerializeField] private ShopController shop;
    //[SerializeField] private GameObject selectedBadge;

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

        if (priceText != null)
            priceText.text = skinData.price <= 0 ? "FREE" : skinData.price.ToString();

        // ===== LOCK =====
        if (!unlocked)
        {
            lockOverlay?.SetActive(true);
            buyButton?.gameObject.SetActive(true);
            useButton?.gameObject.SetActive(false);
            return;
        }

        // ===== UNLOCKED =====
        lockOverlay?.SetActive(false);
        buyButton?.gameObject.SetActive(false);
        useButton?.gameObject.SetActive(true);

        if (useButtonText != null)
            useButtonText.text = isCurrent ? "Used" : "Use";

        if (useButton != null)
            useButton.interactable = !isCurrent;
    }

    void OnBuyClick()
    {
        //ShopController shop = GetComponentInParent<ShopController>();
        if (shop != null)
            shop.TryBuySkin(skinId);
    }

    void OnUseClick()
    {
        //ShopController shop = GetComponentInParent<ShopController>();
        if (shop != null)
            shop.UseSkin(skinId);
    }
}
