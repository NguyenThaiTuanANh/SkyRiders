using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip loadingClip;
    private AudioSource loadingAudioSource;
    [Header("Attributes")]
    [SerializeField] private float timeLoading = 5f;

    [Header("Components")]
    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private GameObject[] firstActiveObjects;
    private float _timer = 0f;

    void Start()
    {

        progressBar.fillAmount = 0;
        loadingText.text = "0%";
        foreach (var item in firstActiveObjects)
        {
            item.SetActive(false);
        }
        // 🔇 Dừng BGM hiện tại
        MusicPlayer.Stop();

        // ▶️ Tạo AudioSource và phát loading sound
        loadingAudioSource = gameObject.AddComponent<AudioSource>();
        loadingAudioSource.clip = loadingClip;
        loadingAudioSource.loop = true;
        loadingAudioSource.Play();
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
            // 🔇 Tắt loading sound
            if (loadingAudioSource != null)
            {
                loadingAudioSource.Stop();
            }

            // ▶️ Phát lại BGM theo scene hiện tại
            Scene currentScene = SceneManager.GetActiveScene();
            if (GameSceneManager.Instance != null)
            {
                if (currentScene.name == GameSceneManager.Instance.mainMenuScene)
                    MusicPlayer.Play(GameSceneManager.Instance.mainMenuBGM);
                else
                    MusicPlayer.Play(GameSceneManager.Instance.gamePlayBGM);
            }

            foreach (var item in firstActiveObjects)
            {
                item.SetActive(true);
            }

            gameObject.SetActive(false);
        }
    }
}
