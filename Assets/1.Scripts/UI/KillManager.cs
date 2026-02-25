using UnityEngine;
using TMPro; // Nếu bạn dùng TextMeshPro

public class KillManager : MonoBehaviour
{
    public static KillManager Instance;

    [Header("UI")]
    public TextMeshProUGUI killText; // Nếu bạn dùng Text thường thì đổi sang Text

    private int killCount = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        UpdateUI();
    }

    public void AddKill()
    {
        killCount++;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (killText != null)
            killText.text = killCount.ToString();
    }

    public void ResetKill()
    {
        killCount = 0;
        UpdateUI();
    }

    public int GetKillCount()
    {
        return killCount;
    }
}