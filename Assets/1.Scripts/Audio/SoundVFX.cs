using UnityEngine;

public class SoundVFX : MonoBehaviour
{
    [SerializeField] AudioClip buttonClip;
    public static void Play(AudioClip clip)
    {
        AudioManager.Instance.PlaySFX(clip);
    }
    public void PlaySbutton()
    {
        AudioManager.Instance.PlaySFX(buttonClip);
    }
}
