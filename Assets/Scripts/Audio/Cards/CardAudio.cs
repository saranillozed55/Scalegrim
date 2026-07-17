using UnityEngine;

public class CardAudio
{
    private AudioSource _audioSource;
    private AudioClip _audioClip;
    private Transform _transform;

    private float minPitch = 0.8f;
    private float maxPitch = 1.2f;

    public CardAudio(AudioSource audioSource, AudioClip audioClip, Transform transform)
    {
        _audioSource = audioSource;
        _audioClip = audioClip;
        _transform = transform;
    }

    public void PlayHoverSound()
    {


        _audioSource.pitch = Random.Range(minPitch, maxPitch);
        _audioSource.PlayOneShot(_audioClip);
    }
}
