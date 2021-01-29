using UnityEngine;

/// <summary>
/// This class will instantiate an Audio Source each time a beat controller fires on an active beat. 
/// This Audio Source will automatically self-destruct once its sound is complete.
/// </summary>
public class BeatSoundPlayer : MonoBehaviour
{
    [Tooltip("The Beat Sound Collection Scriptable Object which stores all the sounds this player can potentially play.")]
    /// <summary>
    /// The Beat Sound Collection Scriptable Object which stores all the sounds this player can potentially play.
    /// </summary>
    public BeatSoundCollection soundCollection;


    [Tooltip("Prefab for an audiosource which will destroy itself after it's completed playing it's clip \n (to be spawned each time a beat controller identifies an active beat).")]
    /// <summary>
    /// Prefab for an audiosource which will destroy itself after it's completed playing it's clip.
    /// To be spawned each time a beat controller identifies an active beat.
    /// </summary>
    public GameObject decayingAudioSource;


    [Tooltip("Defines the way in which we select the next sound to play on an active beat. See BeatSoundOrder for details")]
    /// <summary>
    /// Defines the way in which we select the next sound to play on an active beat.
    /// </summary>
    public BeatSoundOrder soundOrder;

    /// <summary>
    /// When true, the BeatSoundPlayer will play on each active beat associated with it. When false, it will not play a sound, even when an active beat fires.
    /// </summary>
    private bool isPlaying = true;


    [Tooltip("Visible for testing, no need to edit")]
    [SerializeField]
    // Stores the index of the last sound the player played - used to calculate which sound should come next
    private int currentSoundIndex = 0;



    /// <summary>
    /// This will calculate the index of the next sound that should be played from the given sound collection, using the soundOrder
    /// ruleset given, and then play the respective sound using a decaying audioSource.
    /// </summary>
    public void OnActiveBeat()
    {
        if (isPlaying)
        {
            NextSoundIndex();
            PlayCurrentSound();
        }
    }


    /// <summary>
    /// Considers the sound order settings and changes the currentSoundIndex to a new value based on the chosen ruelset.
    /// </summary>
    private void NextSoundIndex()
    {
        switch (soundOrder)
        {
            case BeatSoundOrder.Cycle:
                NextCycleIndex();
                break;

            case BeatSoundOrder.IndexLink:
                CalculateIndexLinkValue();
                break;

            case BeatSoundOrder.PsuedoRandom:
                NextPsuedoRandomIndex();
                break;

            case BeatSoundOrder.Random:
                NextRandomIndex();
                break;
        }
    }


    /// <summary>
    /// Sets our current sound index to the current beat number of our Beat Timing Manager
    /// </summary>
    private void CalculateIndexLinkValue()
    {
        currentSoundIndex = BeatTimingManager.btmInstance.GetBeatNumber() % soundCollection.beatSounds.Length;
    }

    /// <summary>
    /// Update the currentSoundIndex based on the Cycle BeatSoundOrder rules (Mouseover BeatSoundOrder.Cycle for details).
    /// </summary>
    private void NextCycleIndex()
    {
        // The mod function cycles the value of currentSoundIndex back to 0 if it ever becomes larger than the length of the sound clip array
        currentSoundIndex = (currentSoundIndex + 1) % soundCollection.beatSounds.Length;
    }


    /// <summary>
    /// Update the currentSoundIndex based on the PsuedoRandom BeatSoundOrder rules (Random without repeating values).
    /// </summary>
    private void NextPsuedoRandomIndex()
    {
        // This if statement prevents an infinite loop if there's only 1 item in the beatSounds array
        if (soundCollection.beatSounds.Length > 1)
        {
            // We'll cache our current index to compare to the newly randomised one
            int previousIndex = currentSoundIndex;

            // We'll only progress past the randomisation stage if a value not matching the previous one is generated
            while (currentSoundIndex == previousIndex)
            {
                // Randomise a new value for current index
                currentSoundIndex = Random.Range(0, soundCollection.beatSounds.Length);
            }
        }
        else
        {
            currentSoundIndex = 0;
        }
    }


    /// <summary>
    /// Update the currentSoundIndex to a new random integer within the bounds of the soundCollection's clip array.
    /// </summary>
    private void NextRandomIndex()
    {
        currentSoundIndex = Random.Range(0, soundCollection.beatSounds.Length);
    }


    /// <summary>
    /// Considers the currentSoundIndex and plays the matching Audio Clip from the beatSounds array in our assigned BeatSoundCollection.
    /// Note, this is done by instantiating a decaying audio source.
    /// </summary>
    private void PlayCurrentSound()
    {
        GameObject decayingSourceObj = Instantiate(decayingAudioSource) as GameObject;
        decayingSourceObj.GetComponent<DecayingAudioSource>().sourceClip = soundCollection.beatSounds[currentSoundIndex];
    }


    /// <summary>
    /// Prevents the BeatSoundPlayer from playing further beats once the current cycle of beats ends. This should be called as we transition
    /// to the finished composed musical piece.
    /// </summary>
    public void Stop()
    {
        isPlaying = false;
    }

    /// <summary>
    /// Reactivates BeatSoundPlayer, making it play on each active beat again if it was previously stopped.
    /// </summary>
    public void Play()
    {
        isPlaying = true;
    }
}
