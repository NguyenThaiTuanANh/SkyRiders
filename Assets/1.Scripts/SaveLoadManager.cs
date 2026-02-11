using UnityEngine;
using System;

/// <summary>
/// Dữ liệu lưu của player (coin, level đã mở khóa, level hiện tại, ...).
/// </summary>
[Serializable]
public class PlayerSaveData
{
    public int coin;
    public int bombLevel;
    public bool unlockGun;
    public bool unlockRocket;

    [Header("Level Progress")]
    [Tooltip("Level cao nhất đã mở khóa (1-based). Level 1 luôn chơi được.")]
    public int levelUnlocked = 1;

    [Tooltip("Level hiện tại / vừa chơi (để hiển thị hoặc continue).")]
    public int levelCurrent = 1;

    [Header("Plane Skins")]
    [Tooltip("Skin đang dùng (0-4).")]
    public int currentSkinId = 0;

    [Tooltip("Các skin đã mở khóa, lưu dạng \"0,1,2\" (skin 0 mặc định mở).")]
    public string unlockedSkinIds = "0";
}

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }

    const string KEY = "PLAYER_DATA";

    public PlayerSaveData Data = new PlayerSaveData();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Load();
    }

    // ================= SAVE / LOAD =================

    public void Save()
    {
        string json = JsonUtility.ToJson(Data);
        PlayerPrefs.SetString(KEY, json);
        PlayerPrefs.Save();

#if UNITY_EDITOR
        Debug.Log("Save Data: " + json);
#endif
    }

    public void Load()
    {
        if (!PlayerPrefs.HasKey(KEY))
        {
            Data = new PlayerSaveData();
            EnsureDefaultLevelProgress();
            return;
        }

        string json = PlayerPrefs.GetString(KEY);
        Data = JsonUtility.FromJson<PlayerSaveData>(json);
        EnsureDefaultLevelProgress();

#if UNITY_EDITOR
        Debug.Log("Load Data: " + json);
#endif
    }

    void EnsureDefaultLevelProgress()
    {
        if (Data.levelUnlocked < 1) Data.levelUnlocked = 1;
        if (Data.levelCurrent < 1) Data.levelCurrent = 1;
        if (string.IsNullOrEmpty(Data.unlockedSkinIds)) Data.unlockedSkinIds = "0";
        if (Data.currentSkinId < 0 || Data.currentSkinId >= SO_PlaneSkinDatabase.SKIN_COUNT)
            Data.currentSkinId = 0;
    }

    // ================= GETTERS (PlayerData) =================

    public int Coin => Data.coin;
    public int LevelUnlocked => Data.levelUnlocked;
    public int LevelCurrent => Data.levelCurrent;

    // ================= COIN =================

    public void AddCoin(int amount)
    {
        Data.coin += amount;
        Save();
    }

    public bool SpendCoin(int amount)
    {
        if (Data.coin < amount) return false;
        Data.coin -= amount;
        Save();
        return true;
    }

    // ================= LEVEL PROGRESS =================

    /// <summary>
    /// Gọi khi hoàn thành level: mở khóa level tiếp theo (levelIndex + 1).
    /// </summary>
    public void UnlockNextLevel(int completedLevelIndex)
    {
        int nextLevel = completedLevelIndex + 1;
        if (nextLevel > Data.levelUnlocked)
            Data.levelUnlocked = nextLevel;
        Data.levelCurrent = completedLevelIndex;
        Save();
    }

    /// <summary>
    /// Cập nhật level hiện tại (vd. khi bắt đầu chơi level).
    /// </summary>
    public void SetLevelCurrent(int levelIndex)
    {
        Data.levelCurrent = levelIndex;
        Save();
    }

    /// <summary>
    /// Kiểm tra level đã mở khóa chưa (1-based).
    /// </summary>
    public bool IsLevelUnlocked(int levelIndex)
    {
        return levelIndex >= 1 && levelIndex <= Data.levelUnlocked;
    }

    // ================= PLANE SKINS =================

    /// <summary>
    /// Skin đang được chọn (0-4).
    /// </summary>
    public int CurrentSkinId => Data.currentSkinId;

    /// <summary>
    /// Kiểm tra skin đã mở khóa chưa.
    /// </summary>
    public bool IsSkinUnlocked(int skinId)
    {
        if (string.IsNullOrEmpty(Data.unlockedSkinIds)) return skinId == 0;
        string[] parts = Data.unlockedSkinIds.Split(',');
        foreach (string p in parts)
        {
            if (int.TryParse(p.Trim(), out int id) && id == skinId)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Mở khóa skin (đã trả coin bên ngoài). Chỉ thêm vào danh sách unlocked.
    /// </summary>
    public void UnlockSkin(int skinId)
    {
        if (IsSkinUnlocked(skinId)) return;
        Data.unlockedSkinIds = Data.unlockedSkinIds.Trim();
        if (Data.unlockedSkinIds.Length > 0)
            Data.unlockedSkinIds += "," + skinId;
        else
            Data.unlockedSkinIds = skinId.ToString();
        Save();
    }

    /// <summary>
    /// Đổi skin đang dùng (chỉ khi đã mở khóa).
    /// </summary>
    public bool SetCurrentSkin(int skinId)
    {
        if (skinId < 0 || skinId >= SO_PlaneSkinDatabase.SKIN_COUNT) return false;
        if (!IsSkinUnlocked(skinId)) return false;
        Data.currentSkinId = skinId;
        Save();
        return true;
    }
}
