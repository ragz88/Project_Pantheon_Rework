/// <summary>
/// Defines the way in which we select the next sound to play on an active beat.
/// </summary>
public enum BeatSoundOrder
{
    /// <summary>
    /// Beat sounds will always play in order, moving to the next sound in the array. 
    /// <br>Once the array is complete, we'll cycle back to the first sound in the array.</br> 
    /// </summary>
    Cycle,

    /// <summary>
    /// Select the sound to play at random.
    /// </summary>
    Random,

    /// <summary>
    /// Select the sound to play at random, but avoids playing the same sound twice in a row.
    /// </summary>
    PsuedoRandom,

    /// <summary>
    /// Each beat in a beat pattern array will be linked to a sound - <br>so if we're on the third beat in the array, we'll always play the 
    /// third sound in the arrray etc.</br> 
    /// </summary>
    IndexLink
}
