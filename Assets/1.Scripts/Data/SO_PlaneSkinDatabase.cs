using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dữ liệu 1 skin máy bay: đạn, bom, giá, v.v.
/// </summary>
[System.Serializable]
public class PlaneSkinData
{
    [Tooltip("Id skin (0-4 cho 5 skin).")]
    public int skinId = 0;

    [Tooltip("Tên hiển thị (Shop, UI).")]
    public string skinName = "Default";

    [Tooltip("Icon hiển thị trong Shop.")]
    public Sprite icon;

    [Header("Weapon - đạn và bom theo skin")]
    [Tooltip("Prefab đạn (có component Bullet).")]
    public GameObject bulletPrefab;

    [Tooltip("Prefab bom.")]
    public GameObject bombPrefab;

    [Header("Shop")]
    [Tooltip("Giá mua (coin). 0 = mặc định/miễn phí.")]
    public int price = 0;

    [Tooltip("Skin mặc định được mở khóa sẵn (thường skin 0).")]
    public bool defaultUnlocked = true;
}

/// <summary>
/// Database 5 skin máy bay. Mỗi skin có bullet/bomb prefab riêng.
/// </summary>
[CreateAssetMenu(fileName = "SO_PlaneSkinDatabase", menuName = "Game/SO_PlaneSkinDatabase")]
public class SO_PlaneSkinDatabase : ScriptableObject
{
    public const int SKIN_COUNT = 5;

    [Tooltip("Danh sách 5 skin (skinId 0-4).")]
    public List<PlaneSkinData> skins = new List<PlaneSkinData>();

    public PlaneSkinData GetSkin(int skinId)
    {
        if (skins == null) return null;
        foreach (var s in skins)
        {
            if (s != null && s.skinId == skinId)
                return s;
        }
        return null;
    }

    public PlaneSkinData GetSkinByIndex(int index)
    {
        if (skins == null || index < 0 || index >= skins.Count) return null;
        return skins[index];
    }
}
