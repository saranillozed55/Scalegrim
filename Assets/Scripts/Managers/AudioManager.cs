using UnityEngine;

public class AudioManager : GenericSingleton<AudioManager> 
{
    [SerializeField] CardAudioProfileSO defaultCardAudioProfile;
    [SerializeField] private AudioSource sfxSource;
    
    public void Play(CardDataSO cardData, CardAudioType type)
    {
        var profile = cardData.AudioProfile ?? defaultCardAudioProfile;

        AudioClip clip = null;

        switch (type)
        {
            case CardAudioType.Play:
                clip = profile.GetRandomAudioClip(profile.play);
                break;
            case CardAudioType.Hover:
                clip = profile.GetRandomAudioClip(profile.hover);
                break;
            case CardAudioType.Draw:
                clip = profile.GetRandomAudioClip(profile.draw);
                break;
            case CardAudioType.Discard:
                clip = profile.GetRandomAudioClip(profile.discard);
                break;
        }

        if (clip != null)
        {
            sfxSource.pitch = Random.Range(profile.pitchRange.x, profile.pitchRange.y);
            sfxSource.PlayOneShot(clip, profile.volume);
            Debug.Log("AudioManager played");
        }
    }

}
