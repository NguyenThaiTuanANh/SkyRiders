using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;

    [Header("Scene Names")]
    public string mainMenuScene = "MainMenu";
    public string gamePlayScene = "GamePlayScene";

    [Header("Optional - load scene by level data")]
    [SerializeField] private SO_LevelDatabase levelDatabase;

    [Header("BGM Clips")]
    public AudioClip mainMenuBGM;
    public AudioClip gamePlayBGM;
    public event System.Action<Scene> OnSubLevelLoaded;
    public event System.Action<Scene> OnSubLevelUnloaded;

    private readonly List<string> loadedScenes = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        this.SetTimeScale(1);
        if (scene.name == mainMenuScene)
        {
            MusicPlayer.Play(mainMenuBGM);
        }
        else if (scene.name == gamePlayScene || (levelDatabase != null && IsGamePlayScene(scene.name)))
        {
            MusicPlayer.Play(gamePlayBGM);
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void LoadMainMenuScene()
    {
        this.LoadScene(mainMenuScene);
    }

    public void LoadGamePlayScene()
    {
        LoadScene(gamePlayScene);
    }

    /// <summary>
    /// Load scene gameplay cho level: set LevelToLoad và load đúng scene theo SO_LevelData.sceneName.
    /// </summary>
    public void LoadGamePlaySceneForLevel(int levelIndex)
    {
        LevelManager.LevelToLoad = levelIndex;

        string sceneName = gamePlayScene;
        if (levelDatabase != null)
        {
            SO_LevelData data = levelDatabase.GetLevel(levelIndex);
            if (data != null && !string.IsNullOrEmpty(data.sceneName))
                sceneName = data.sceneName;
        }

        LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }

    //public void LoadSubLevel(string sceneName)
    //{
    //    if (loadedScenes.Contains(sceneName)) return;
    //    StartCoroutine(LoadSceneAsync(sceneName));
    //}

    //private IEnumerator LoadSceneAsync(string sceneName)
    //{
    //    AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    //    yield return op;

    //    loadedScenes.Add(sceneName);
    //    Scene scene = SceneManager.GetSceneByName(sceneName);
    //    SceneManager.SetActiveScene(scene);

    //    OnSubLevelLoaded?.Invoke(scene); // 🔥 ACTION TẠI ĐÂY
    //}

    //public void UnloadSubLevel(string sceneName)
    //{
    //    if (!loadedScenes.Contains(sceneName)) return;
    //    StartCoroutine(UnloadSceneAsync(sceneName));
    //}

    //private IEnumerator UnloadSceneAsync(string sceneName)
    //{
    //    Scene scene = SceneManager.GetSceneByName(sceneName);
    //    AsyncOperation op = SceneManager.UnloadSceneAsync(sceneName);
    //    yield return op;

    //    loadedScenes.Remove(sceneName);
    //    OnSubLevelUnloaded?.Invoke(scene); // 🔥
    //}

    //public void UnloadAllSubLevels()
    //{
    //    foreach (var scene in new List<string>(loadedScenes))
    //        UnloadSubLevel(scene);
    //}

    private bool IsGamePlayScene(string sceneName)
    {
        if (levelDatabase == null || levelDatabase.levels == null) return false;
        foreach (var level in levelDatabase.levels)
        {
            if (level != null && level.sceneName == sceneName) return true;
        }
        return false;
    }
}
