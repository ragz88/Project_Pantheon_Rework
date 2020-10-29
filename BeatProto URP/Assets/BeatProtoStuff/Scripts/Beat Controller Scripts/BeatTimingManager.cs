using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Keeps track of how long we are into the current song/beat collection, which beat we're currently on and fires the OnBeat events
/// of various beat controllers at the appropriate time.
/// </summary>
public class BeatTimingManager : MonoBehaviour
{
    /// <summary>
    /// The Song Scriptable Object associated with the current level
    /// </summary>
    public Song currentSong;

    /// <summary>
    /// Array of BeatControllers - each associated with one type of beat-controlled element within the level.
    /// </summary>
    public BeatController[] beatControllers;



    // The current position of the seeker (in seconds) within the current song/beat collection
    private double seekerTime = 0;
    
    // The BPM of the currently active song/music collection
    //private float currentBPM = 120;

    // The length of a single beat (in seconds) - calculated based on the BPM
    private double beatLength = 0;

    // This will time how long it's been since the last beat fired. When it becomes greater than beatLength, we'll fire another beat
    private double beatTimer = 0;

    // The index/number of the last beat that played - comparable to the index of a boolean in a BeatController's ActiveBeats array.
    private int currentBeatNumber = 0;


    #region Singleton
    // This singleton ensures that there's only one BeatTimingManager present in any given scene - making it easy to reference and 
    // ensuring the times we're working with are always the correct ones

    public static BeatTimingManager btmInstance;

    private void Awake()
    {
        if (btmInstance == null)
        {
            btmInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion


    // Start is called before the first frame update
    void Start()
    {
        //currentBPM = currentSong.BPM;
        beatLength = currentSong.BPM / 60;
    }

    // Update is called once per frame
    void Update()
    {
        // check if the current beat is finished based on our beat timer
        if (beatTimer >= beatLength)
        {
            // reset timer
            beatTimer = beatTimer - beatLength;  // We don't want to reset to 0 - then any milliseconds difference between 
                                                 // the timer and beatLength will be lost, resulting in a compounding time shift

            // Activate all the onBeat events for our currently present BeatControllers
            for (int i = 0; i < beatControllers.Length; i++)
            {
                // First we'll check if this specific beat controller is meant to fire on this beat, based on the settings the player
                // has locked into it.
                // The % (mod) function allows for various beat controllers' activeBeats arrays to be different lengths.
                // Note that they must still be a multiple of the total number of beats from the current song
                if (beatControllers[i].activeBeats[currentBeatNumber % beatControllers[i].activeBeats.Length])
                {
                    beatControllers[i].OnBeat.Invoke();
                }
            }

            // We also want to increase the current beat counter - or cycle it back to 0 if we've completed an entire cycle of beats.
            // We subtract 1 due to the array's 0 index
            if (currentBeatNumber >= currentSong.totalBeatNum - 1)
            {
                currentBeatNumber = 0;
            }
            else
            {
                currentBeatNumber++;
            }
        }

        // Increase our timers by the time that's passed
        seekerTime += Time.deltaTime;
        beatTimer += Time.deltaTime;
    }


    private void LoadNewSong(Song newSong)
    {
        currentSong = newSong;
    }

    /// <summary>
    /// Stops playing the currently present beats and activating the elements associated with them.
    /// </summary>
    public void StopBeat()
    {

    }

    /// <summary>
    /// Restarts the current beat from 0
    /// </summary>
    public void RestartBeat()
    {

    }
}
