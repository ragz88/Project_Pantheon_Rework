using UnityEngine;

/// <summary>
/// A pattern of booleans which represent active and inactive beats. This class links them to a colour, allowing for specific 
/// comparrisons to player-defined active beat arrays and solution detection.
/// </summary>
//[CreateAssetMenu(fileName = "New Beat Pattern", menuName = "Music Objects/Beat Pattern")]
[System.Serializable]
public class BeatPattern
{
    /// <summary>
    /// The colour (ie. type) of beat activated elements that this beat pattern is linked to - important for identifying which
    /// solution to a level the player has found.
    /// </summary>
    public BeatElementColour beatColour;

    /// <summary>
    /// Specific pattern of bools that represent a collection of active and inactive beats. 
    /// To be compared to a beat controller's activeBeats array.
    /// </summary>
    public bool[] pattern;
}
