using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : GenericSingleton<SoundMixerManager>
{

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private PlayerSoundSO soundSO;

    protected override void Awake()
    {
        base.Awake();

        soundSO.LoadFromPrefs();

        soundSO.OnMasterVolumeChanged += SetMasterVolume;
        soundSO.OnSFXVolumeChanged += SetSFXVolume;
        soundSO.OnMusicVolumeChanged += SetMusicVolume;
    }
    private void Start()
    {
        SetMasterVolume(soundSO.masterVolume);
        SetSFXVolume(soundSO.sfxVolume);
        SetMusicVolume(soundSO.musicVolume);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        soundSO.OnMasterVolumeChanged -= SetMasterVolume;
        soundSO.OnSFXVolumeChanged -= SetSFXVolume;
        soundSO.OnMusicVolumeChanged -= SetMusicVolume;
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause) PlayerPrefs.Save();
    }

    public void SetMasterVolume(float level)
    {
        bool ok = audioMixer.SetFloat("masterVolume", LevelToDb(level));
        if (!ok) Debug.LogWarning($"Failed to set masterVolume - check exposed parameter name on the AudioMixer.");
    }

    public void SetSFXVolume(float level)
    {
        bool ok = audioMixer.SetFloat("sfxVolume", LevelToDb(level));
        if (!ok) Debug.LogWarning("Failed to set sfxVolume - check exposed parameter name on the AudioMixer.");
    }


    public void SetMusicVolume(float level)
    {
        bool ok = audioMixer.SetFloat("musicVolume", LevelToDb(level));
        if (!ok) Debug.LogWarning($"Failed to set musicVolume - check exposed parameter name on the AudioMixer.");

    }
    private float LevelToDb(float level) => Mathf.Log10(Mathf.Max(level, 0.0001f)) * 20f;
}
