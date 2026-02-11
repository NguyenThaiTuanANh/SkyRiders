using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private float timeLoading = 5f;

    [Header("Components")]
    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private GameObject[] firstActiveObjects;
    //[SerializeField] private UIMainMenuTitleEffect_Enhanced vfxTitle;

    //private AsyncOperation _sceneOperation;
    private float _timer = 0f;

    void Start()
    {
        //_sceneOperation = SceneManager.LoadSceneAsync("GameplayScene");
        //_sceneOperation.allowSceneActivation = false;

        progressBar.fillAmount = 0;
        loadingText.text = "0%";
        foreach (var item in firstActiveObjects)
        {
            item.SetActive(false);
        }
    }

    void Update()
    {
        if (_timer < timeLoading)
        {
            _timer += Time.deltaTime;

            float progress = Mathf.Clamp01(_timer / timeLoading);

            progressBar.fillAmount = progress;
            loadingText.text = $"Loading... {(int)(progress * 100)}%";
        }
        else
        {
            //_sceneOperation.allowSceneActivation = true;
            foreach (var item in firstActiveObjects)
            {
                item.SetActive(true);
            }
            //if (vfxTitle != null) vfxTitle.PlayIntro();
            gameObject.SetActive(false);
        }
    }
}
