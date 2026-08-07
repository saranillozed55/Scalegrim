using UnityEngine;

[CreateAssetMenu(fileName = "Audio/CardProfile")]
public class CardAudioProfileSO : ScriptableObject
{
    public AudioClip[] play;
    public AudioClip[] hover;
    public AudioClip[] draw;
    public AudioClip[] discard; //don't know if this is needed yet maybe use it for when card is destroyed

    [Range(0f, 1f)]
    public float volume = 1f;

    public Vector2 pitchRange = new Vector2(0.95f, 1.05f);

    public AudioClip GetRandomAudioClip(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0)
            return null;

        int randomIndex = Random.Range(0, clips.Length);
        return clips[randomIndex];
    }
}
