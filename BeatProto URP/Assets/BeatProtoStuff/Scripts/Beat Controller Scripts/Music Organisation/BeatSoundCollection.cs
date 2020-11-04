using UnityEngine;

/// <summary>
/// A collection of specific musical sounds to be played on the active beats of various beat controllers in a level.
/// Class includes a colour category for these sounds and a setting for how one beat should progress to the next.
/// </summary>
[CreateAssetMenu(fileName = "New Beat Sound Collection", menuName = "Music Objects/Beat Sound Collection")]
public class BeatSoundCollection : ScriptableObject
{
    [Tooltip("The list of sounds we'll choose from when playing a musical note on an active beat in a level.")]
    /// <summary>
    /// The list of sounds we'll choose from when playing a musical note on an active beat in a level.
    /// </summary>
    public AudioClip[] beatSounds;
}
