using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("===== SETTINGS =====")]
    [SerializeField] public bool SFXEnabled = true;
    [SerializeField] public bool BGMEnabled = true;
    [SerializeField] float SFXVolume = 1f;
    [SerializeField] float BGMVolume = 1f;

    [Header("===== SOURCES =====")]
    [SerializeField] private int sfxInitialPool = 5;
    private List<AudioSource> sfxSources = new List<AudioSource>();
    private AudioSource bgmSource;


    private void Awake()
    {
        SFXEnabled = true;
        BGMEnabled = true;
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitSFXPool();
        InitBGMSource();
    }


    // ---------- SFX ----------
    private void InitSFXPool()
    {
        for (int i = 0; i < sfxInitialPool; i++)
            CreateNewSfxSource();
    }

    private AudioSource CreateNewSfxSource()
    {
        AudioSource s = gameObject.AddComponent<AudioSource>();
        s.playOnAwake = false;
        s.spatialBlend = 0;
        sfxSources.Add(s);
        return s;
    }


    private AudioSource GetFreeSfxSource()
    {
        foreach (var s in sfxSources)
            if (!s.isPlaying) return s;

        return CreateNewSfxSource(); // pool dynamic
    }

    public AudioSource PlaySFX(AudioClip clip)
    {
        if (!SFXEnabled || clip == null) return null;

        var src = GetFreeSfxSource();
        src.volume = SFXVolume;
        src.PlayOneShot(clip);
        return src;
    }

    public void StopAllSFX()
    {
        foreach (var s in sfxSources)
            s.Stop();
    }


    // ---------- BGM ----------
    private void InitBGMSource()
    {
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.playOnAwake = false;
        bgmSource.loop = true;
        bgmSource.spatialBlend = 0;
        bgmSource.volume = BGMVolume;
    }

    public void PlayBGM(AudioClip music)
    {
        if (music == null) return;

        bgmSource.clip = music;
        bgmSource.volume = BGMVolume;

        if (BGMEnabled)
            bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void SetBGMEnabled(bool enabled)
    {
        BGMEnabled = enabled;

        if (!enabled)
            bgmSource.Stop();
        else if (bgmSource.clip != null)
            bgmSource.Play();
    }

    public void SetSFXEnabled(bool enabled)
    {
        SFXEnabled = enabled;

        if (!enabled)
            StopAllSFX();
    }
}
