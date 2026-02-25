using System.Threading.Tasks;
using UnityEngine;

public class BaseVFX : MonoBehaviour
{
    public ParticleSystem vfx;
    [SerializeField] AudioClip soundVFX;
    [SerializeField] AudioSource audioSource;


    public void Play()
    {
        if (!vfx.gameObject.activeSelf)
            vfx.gameObject.SetActive(true);

        if (!vfx.isPlaying)
            vfx.Play();

        if (audioSource == null || !audioSource.isPlaying)
        {
            audioSource = AudioManager.Instance.PlaySFX(soundVFX);
        }
    }

    public async Task PlayAndStop(int time)
    {
        Play();
        await Task.Delay(time);
        Stop();
    }

    public void Stop()
    {
        if (vfx.isPlaying)
            vfx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (vfx.gameObject.activeSelf)
            vfx.gameObject.SetActive(false);

        if (audioSource && audioSource.isPlaying)
        {
            audioSource.Stop();
            audioSource.clip = null;
        }
        audioSource = null;
    }
}
