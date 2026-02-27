using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelection : MonoBehaviour
{

    [SerializeField] private int levelIndex;
    [SerializeField] private GameObject lockImage;
    [SerializeField] private Button button;

    private void Start()
    {
        if (button != null)
            button.onClick.AddListener(OnClickLevel);
        RefreshUnlockState();
    }

    private void OnEnable()
    {
        RefreshUnlockState();
    }

    private void RefreshUnlockState()
    {
        int highestLevel = SaveLoadManager.Instance != null ? SaveLoadManager.Instance.LevelUnlocked : 1;
        bool unlocked = levelIndex <= highestLevel;

        if (lockImage != null) lockImage.SetActive(!unlocked);
        if (button != null) button.interactable = unlocked;
    }

    public void OnClickLevel()
    {
        if (GameSceneManager.Instance != null)
            GameSceneManager.Instance.LoadGamePlaySceneForLevel(levelIndex);
    }
    //public void OnClickLevel()
    //{
    //    StartCoroutine(LoadLevelWithDelay());
    //}

    //private IEnumerator LoadLevelWithDelay()
    //{
    //    // Bật loading
    //    UIMainMenu.Instance.Loading.SetActive(true);

    //    // Chờ 5 giây
    //    yield return new WaitForSeconds(5f);

    //    // Sau 5 giây mới load scene
    //    if (GameSceneManager.Instance != null)
    //    {
    //        GameSceneManager.Instance.LoadGamePlaySceneForLevel(levelIndex);
    //    }
    //}

}
