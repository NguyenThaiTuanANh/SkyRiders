using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public static void Play(AudioClip bgm)
    {
        AudioManager.Instance.PlayBGM(bgm);
    }

    public static void Stop()
    {
        AudioManager.Instance.StopBGM();
    }
}
