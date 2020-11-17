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


    [HideInInspector]
    /// <summary>
    /// Clips put in here will automatically be transitioned to when the current clip ends if the compositionSource is still playing.
    /// </summary>
    public AudioClip transitionClip;


    /// <summary>
    /// This prefab will be used when the player re-enters puzzle areas or when music needs to transition at extreme speed and 
    /// cannot wait for the current AudioClip to end (for instance, if the Monster came crashing through a wall). We'll fade out 
    /// the current clip using this, and immediately move into the transition clip.
    /// </summary>
    public GameObject decayingAudioSource;


    // This boolean will be set to true when the player exits the level. It will tell this class to wait for the current beat cycle to end, 
    // then start the final composed musical piece.
    private bool waitForCycleEnd = false;

    // Set to true when the cycle reaches the totalBeatNumber after the MusicManager has been told to wait for the end of a cycle.
    // Once the cycle returns to 0 after this point, we'll play the final music piece.
    private bool playCompositionAtZero = false;

    // This will be set to true when the compositionAudioSource should be playing. 
    private bool playCompositionAudio = false;


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


    private void Update()
    {
        // We've now started looking out for the final beat in the cycle - alowing our beatSoundPlayers to play the beats for 
        // the remainder of this beat cycle before changing to the composed version of the musical piece.
        if (waitForCycleEnd)
        {
            // We now look out for when the TimingManager hits the final beat in a cycle. When it does, we'll tell it that it can play 
            // the composed piece for this level when we hit 0 again.
            if (BeatTimingManager.btmInstance.GetBeatNumber() == BeatTimingManager.btmInstance.currentSong.totalBeatNum - 1)
            {
                playCompositionAtZero = true;
            }

            // This will only be true if we've already reached the final beat in a cycle AFTER the player has finished the puzzle level.
            if (playCompositionAtZero)
            {
                // We finally look for the moment our beat number returns to 0 - marking the start of a new cycle.
                // At this moment, we transition to the composed piece of music.
                if (BeatTimingManager.btmInstance.GetBeatNumber() == 0)
                {
                    // We'll tell all our beatSoundPlayers that their job is done - they can stop playing until the next puzzle level.
                    for (int i = 0; i < beatSoundPlayers.Length; i++)
                    {
                        beatSoundPlayers[i].Stop();
                    }

                    // We also need to reset the booleans used to detect this moment, making them ready for the next puzzle level end.
                    playCompositionAtZero = false;
                    waitForCycleEnd = false;

                    // And finally, we'll tell our specially selected composition to play
                    compositionAudioSource.Play();
                }
            }
        }


        // Here, we check if the compositionAudioSource is not laying when it actually should be
        if (!compositionAudioSource.isPlaying && playCompositionAudio)
        {
            // We then check if there's a clip the audioSource is supposed to transition to
            if (transitionClip != null)
            {
                // and if there is, we change to it. 
                compositionAudioSource.clip = transitionClip;
            }

            // Regardless of whether there's a transition clip or not, the Audiosource should be playing, so let's start it up again
            compositionAudioSource.Play();
        }
    }

    /// <summary>
    /// Should be called when the player finishes the puzzle portion of a level - starting the new sequence of composed music and platforming.
    /// </summary>
    public void OnLevelCompleted(int boop)
    {
        // Testing Debug
        Debug.Log("You got it! Solution " + boop + " found.");

        // Tells the Music Manager to look out for the end of a cycle, at which point it should start playing.
        waitForCycleEnd = true;
    }

    public void OnPuzzleLevelEnter()
    {
        FadeOutCompositionSource(true, 0.5f);      // Playtest This value!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        compositionAudioSource.clip = null;
        transitionClip = null;


        // find new BeatSoundPlayers from TimingManager ===========================================================================
    }


    public void ChangeCompositionClipInstantaneously()
    {
        // Create a decaying audio source with the settings of our CompositionAudioSource
        // Given the nature of the function, this will happen quite quickly               
        FadeOutCompositionSource(true, 3);   // PLAYTEST THIS VALUE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        // After creating our fading Audio Source, we'll update the compositionSource to play the new musical piece
        compositionAudioSource.clip = transitionClip;
    }


    /// <summary>
    /// Creates a decaying audio source with all the properties of our current composition source.
    /// This source is setup such that it will become perpetually softer until it destroys itself.
    /// </summary>
    private void FadeOutCompositionSource(bool fadeOutVolume, float fadeSpeed)
    {
        // Instantiate decaying source - while keeping a reference to it
        GameObject decayingSourceObj = Instantiate(decayingAudioSource) as GameObject;
        DecayingAudioSource decayingSource = decayingSourceObj.GetComponent<DecayingAudioSource>();

        // Update the decaying source's setting to match those of our composition source.
        decayingSource.sourceClip = compositionAudioSource.clip;
        decayingSource.volume = compositionAudioSource.volume;
        decayingSource.startTime = compositionAudioSource.time;
        decayingSource.fadeVolume = fadeOutVolume;
        decayingSource.fadeOutSpeed = fadeSpeed;
    }
}
