using UnityEngine;

public class GamePlaySoudVFX : MonoBehaviour
{
    public static GamePlaySoudVFX Instance;
    [SerializeField] AudioClip winAudioClip;
    [SerializeField] AudioClip loseAudioClip;
    [SerializeField] AudioClip enemyDieClip;
    [SerializeField] AudioClip playerFireClip;
    [SerializeField] AudioClip enemyFireClip;
    [SerializeField] AudioClip boomClip;
    [SerializeField] AudioClip TankDieClip;

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
    public void PlayerFire()
    {
        AudioManager.Instance.PlaySFX(playerFireClip);
    }
    public void EnemyFire()
    {
        AudioManager.Instance.PlaySFX(enemyFireClip);
    }
    public void BoomPlay()
    {
        AudioManager.Instance.PlaySFX(boomClip);
    }
    public void TankDiePlay()
    {
        AudioManager.Instance.PlaySFX(boomClip);
    }
}
