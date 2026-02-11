using UnityEngine;

public class GamePlaySoudVFX : MonoBehaviour
{
    public static GamePlaySoudVFX Instance;
    [SerializeField] AudioClip winAudioClip;
    [SerializeField] AudioClip loseAudioClip;
    [SerializeField] AudioClip enemyDieClip;

    private void Awake() {
        Instance = this;
    }

    public void PlayWin()
    {
        AudioManager.Instance.PlaySFX(winAudioClip);
    }

    public void PlayLose()
    {
        AudioManager.Instance.PlaySFX(loseAudioClip);
    }

    public void EnemyDie()
    {
        AudioManager.Instance.PlaySFX(enemyDieClip);
    }
}
