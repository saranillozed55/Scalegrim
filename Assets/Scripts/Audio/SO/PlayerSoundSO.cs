using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSoundSO")]
public class PlayerSoundSO : ScriptableObject
{
    [Range(0.0001f, 1f)]
    public float masterVolume = 1.0f;
    [Range(0.0001f, 1f)]

    public float sfxVolume = 1.0f;

    [Range(0.0001f, 1f)]
    public float musicVolume = 1.0f;

    private const string MasterKey = "MasterVolume";
    private const string SFXKey = "SFXVolume";
    private const string MusicKey = "MusicVolume";

    public event Action<float> OnMasterVolumeChanged;
    public event Action<float> OnSFXVolumeChanged;
    public event Action<float> OnMusicVolumeChanged;

    public void SetMasterVolume(float value)
    {
        masterVolume = value;
        PlayerPrefs.SetFloat(MasterKey, value);
        OnMasterVolumeChanged?.Invoke(value);
    }
    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        PlayerPrefs.SetFloat(SFXKey, value);
        OnSFXVolumeChanged?.Invoke(value);
    }
    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        PlayerPrefs.SetFloat(MusicKey, value);
        OnMusicVolumeChanged?.Invoke(value);
    }

    public void LoadFromPrefs()
    {
        masterVolume = PlayerPrefs.GetFloat(MasterKey, 1f);
        sfxVolume = PlayerPrefs.GetFloat(SFXKey, 1f);
        musicVolume = PlayerPrefs.GetFloat(MusicKey, 1f);
    }
}
