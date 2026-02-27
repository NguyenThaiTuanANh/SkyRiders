using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    /// <summary>
    /// Level được chọn từ LevelSelection trước khi load GamePlay. Set trước khi gọi LoadGamePlayScene().
    /// </summary>
    public static int LevelToLoad { get; set; } = 1;

    [Header("Level Info")]
    public int levelIndex = 1;
    public TMP_Text LevelInfor;

    [Header("Optional - assign to use reward from level data")]
    [SerializeField] private SO_LevelDatabase levelDatabase;

    [Header("Scene Names")]
    [Tooltip("Tên scene Main Menu (phải trùng Build Settings).")]
    [SerializeField] private string mainMenuSceneName = "MainMenuScene";

    private bool levelFinished = false;
    private bool isPaused = false;

    public int reward = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (LevelToLoad > 0)
        {
            levelIndex = LevelToLoad;
            //levelIndex = 9;
            LevelInfor.text = "Level: " + LevelToLoad.ToString();
        }
    }

    void Start()
    {
        StartLevel();
    }

    // ================= LEVEL FLOW =================

    public void StartLevel()
    {
        levelFinished = false;
        Time.timeScale = 1f;

        if (SaveLoadManager.Instance != null)
            SaveLoadManager.Instance.SetLevelCurrent(levelIndex);

#if UNITY_EDITOR
        Debug.Log("Level Started: " + levelIndex);
#endif
    }

    public void LevelComplete()
    {
        if (levelFinished) return;
        levelFinished = true;

        //Time.timeScale = 0.5f;

#if UNITY_EDITOR
        Debug.Log("LEVEL COMPLETE!");
#endif

        // Reward coin from level data if available
        if (levelDatabase != null)
        {
            SO_LevelData levelData = levelDatabase.GetLevel(levelIndex);
            if (levelData != null)
                reward = levelData.rewardCoin;
        }
        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.AddCoin(reward);
            SaveLoadManager.Instance.UnlockNextLevel(levelIndex);
        }
        MusicPlayer.Stop();
       GamePlaySoudVFX.Instance.PlayWin();
        if (UIController.Instance != null)
            UIController.Instance.ShowLevelComplete();
    }

    public void GameOver()
    {
        if (levelFinished) return;
        levelFinished = true;

        Time.timeScale = 0f;

#if UNITY_EDITOR
        Debug.Log("GAME OVER");
#endif

        if (UIController.Instance != null)
            UIController.Instance.ShowGameOver();
    }

    // ================= BUTTON =================

    public void RetryLevel()
    {
        isPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Có level tiếp theo không (dùng để ẩn/hiện nút Next Level trên UI).
    /// </summary>
    public bool HasNextLevel()
    {
        if (levelDatabase == null) return false;
        return levelDatabase.HasLevel(levelIndex + 1);
    }

    public void NextLevel()
    {
        isPaused = false;
        Time.timeScale = 1f;

        int nextLevelIndex = levelIndex + 1;
        LevelToLoad = nextLevelIndex;

        if (levelDatabase != null)
        {
            SO_LevelData nextData = levelDatabase.GetLevel(nextLevelIndex);
            if (nextData != null && !string.IsNullOrEmpty(nextData.sceneName))
            {
                SceneManager.LoadScene(nextData.sceneName);
                return;
            }
        }

        // Không còn level: về Main Menu
        if (levelDatabase != null && nextLevelIndex > levelDatabase.LevelCount)
        {
            SceneManager.LoadScene(mainMenuSceneName);
            return;
        }

        int nextBuildIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextBuildIndex);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // ================= PAUSE =================

    public bool IsPaused => isPaused;

    public void PauseGame()
    {
        if (levelFinished) return;
        isPaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (levelFinished) return;
        isPaused = false;
        Time.timeScale = 1f;
    }

    public void TogglePause()
    {
        if (levelFinished) return;
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
    }
}
