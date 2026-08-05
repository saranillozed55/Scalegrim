using UnityEngine;

[CreateAssetMenu(fileName = "Audio/CardProfile")]
public class CardAudioProfileSO : ScriptableObject
{

    public AudioClip play;
    public AudioClip hover;
    public AudioClip draw;
    public AudioClip discard; //don't know if this is needed yet

    [Range(0f, 1f)]
    public float volume = 1f;

    public Vector2 pitchRange = new Vector2(0.95f, 1.05f);

}
