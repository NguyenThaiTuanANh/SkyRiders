using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    [Header("UI Panels")]
    public GameObject gameplayUI;
    public GameObject gameOverUI;
    public GameObject levelCompleteUI;
    public GameObject pauseUI;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        ShowGameplayUI();
    }

    // ================= SHOW UI =================

    public void ShowGameplayUI()
    {
        gameplayUI?.SetActive(true);
        gameOverUI?.SetActive(false);
        levelCompleteUI?.SetActive(false);
        pauseUI?.SetActive(false);
    }

    public void ShowGameOver()
    {
        gameplayUI?.SetActive(false);
        levelCompleteUI?.SetActive(false);
        pauseUI?.SetActive(false);
        gameOverUI?.SetActive(true);

#if UNITY_EDITOR
        Debug.Log("UI: Game Over");
#endif
    }

    public void ShowLevelComplete()
    {
        gameplayUI?.SetActive(false);
        gameOverUI?.SetActive(false);
        levelCompleteUI?.SetActive(true);
        pauseUI?.SetActive(false);

#if UNITY_EDITOR
        Debug.Log("UI: Level Complete");
#endif
    }

    public void ShowPauseUI()
    {
        if (pauseUI != null) pauseUI.SetActive(true);
    }

    public void HidePauseUI()
    {
        if (pauseUI != null) pauseUI.SetActive(false);
    }

    // ================= BUTTON EVENTS =================

    public void OnRetryButton()
    {
        if (LevelManager.Instance != null)
            LevelManager.Instance.RetryLevel();
    }

    public void OnNextLevelButton()
    {
        if (LevelManager.Instance != null)
            LevelManager.Instance.NextLevel();
    }

    public void OnBackToMenuButton()
    {
        if (LevelManager.Instance != null)
            LevelManager.Instance.BackToMenu();
    }

    public void OnPauseButton()
    {
        if (LevelManager.Instance == null) return;
        LevelManager.Instance.PauseGame();
        ShowPauseUI();
    }

    public void OnResumeButton()
    {
        if (LevelManager.Instance == null) return;
        LevelManager.Instance.ResumeGame();
        HidePauseUI();
    }
}
