using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Song", menuName = "Music Objects/Song")]
public class Song : ScriptableObject
{
    [Tooltip("The Beats Per Minute of the final piece as well as the individual beat sounds to be played in the puzzle levels.")]
    /// <summary>
    /// The Beats Per Minute of the final piece as well as the individual beat sounds to be played in the puzzle levels.
    /// </summary>
    public float BPM = 120;


    [Tooltip("The total number of beats in a single cycle for the initial puzzle section.")]
    /// <summary>
    /// The total number of beats in a single cycle for the initial puzzle section.
    /// </summary>
    public int totalBeatNum = 8;


    [Tooltip("Allows us to access and play the specific beat sounds designed for the puzzle portion of the level. Array of Scriptable Objects.")]
    /// <summary>
    /// Allows us to access and play the specific beat sounds designed for the puzzle portion of the level.
    /// </summary>
    public BeatSoundCollection[] beatSoundCollections;


    [Tooltip("A collection of possible solutions to the current level (beat setups that would feasibly allow the player to progress through the level's platforming phase).")]
    /// <summary>
    /// A collection of possible solutions to the current level (beat setups that would feasibly allow the player to progress
    /// through the level's platforming phase).
    /// </summary>
    public MusicalSolution[] possibleSolutions;
}
