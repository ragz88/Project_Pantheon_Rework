using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    [HideInInspector]
    /// <summary>
    /// The BeatSoundPlayers in the current level. Extracted from the BeatControllers stored in the BeatTimingManager;
    /// </summary>
    public BeatSoundPlayer[] beatSoundPlayers;


    /// <summary>
    /// The audio source that will be used to play final compositions once the player has found a viable solution. Will be playing in 
    /// platforming exclusive levels and off in the puzzle levels.
    /// </summary>
    public AudioSource compositionAudioSource;


    #region Singleton
    // This singleton ensures that there's only one MusicManager present in any given scene - making it easy to reference and
    // ensuring all AudioSources for music are accessible in a single place.

    public static MusicManager MMInstance;

    private void Awake()
    {
        if (MMInstance == null)
        {
            MMInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    public void OnLevelCompleted()
    {
        //Call functions in sound players
        // play finished audio
    }

    public void OnPuzzleLevelEnter()
    {
        // stop finished audio
        // find new BeatSoundPlayers from TimingManager
    }
}
