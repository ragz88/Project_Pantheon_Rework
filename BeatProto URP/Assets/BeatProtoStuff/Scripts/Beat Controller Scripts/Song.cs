using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Song", menuName = "Music Objects/Song")]
public class Song : ScriptableObject
{
    /// <summary>
    /// The Beats Per Minute of the final piece as well as the individual beat sounds to be played in the puzzle levels.
    /// </summary>
    public float BPM = 120;

    /// <summary>
    /// The total number of beats in a single cycle for the initial puzzle section.
    /// </summary>
    public int totalBeatNum = 8;
}
