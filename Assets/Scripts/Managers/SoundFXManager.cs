using UnityEngine;
using UnityEngine.Audio;

public class SoundFXManager : GenericSingleton<SoundFXManager>
{
    [SerializeField] private CardAudioProfileSO defaultCardAudioProfile;
    [SerializeField] private AudioSource sfxSource; // this should be Sfx group

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

            sfxSource.clip = clip;
            sfxSource.pitch = Random.Range(profile.pitchRange.x, profile.pitchRange.y);
            sfxSource.PlayOneShot(clip, profile.volume); //this should not be playoneshot because we want to be able to control the volume of the sfx group, so we should use Play() instead and set the clip to the audio source

            Debug.Log("SoundFXManager played");
        }
    }
}
