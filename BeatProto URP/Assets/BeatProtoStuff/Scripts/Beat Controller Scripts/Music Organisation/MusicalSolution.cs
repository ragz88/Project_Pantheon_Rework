using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A collection of beat patterns that represent one of the feasible beat setups that will allow the player to solve the level.
/// <br>This solution is also connected to a piece of music specifically composed with its beats in mind, to be played in the
/// platforming section to follow the initial puzzle level.</br>
/// </summary>
[CreateAssetMenu(fileName = "New Solution", menuName = "Music Objects/Musical Solution")]
public class MusicalSolution : ScriptableObject
{
    [Tooltip("The piece of music specifically composed around these solution beats - to be played in the following platforming phase.")]
    /// <summary>
    /// The piece of music specifically composed around these solution beats - to be played in the following platforming phase.
    /// </summary>
    public AudioClip solutionComposition;

    [Tooltip("The introductory section for the piece of music specifically composed around these solution beats - to be played as the player exits the puzzle phase. \n The MusicManager will automatically transition from this into solutionComposition when this piece ends.")]
    /// <summary>
    /// The introductory section for the piece of music specifically composed around these solution beats - to be played as the player 
    /// exits the puzzle phase.
    /// The MusicManager will automatically transition from this into solutionComposition when this piece ends.
    /// </summary>
    public AudioClip solutionCompositionIntro;


    [Tooltip("A collection of beat patterns, categorised by Colour (ie. Beat Affected Element Type), that represent one of the feasible beat setups which will allow the player to solve the level.")]
    /// <summary>
    /// A collection of beat patterns, categorised by Colour (ie. Beat Affected Element Type), that represent one of the feasible beat 
    /// setups which will allow the player to solve the level.
    /// </summary>
    public BeatPattern[] correctPatterns;
}
