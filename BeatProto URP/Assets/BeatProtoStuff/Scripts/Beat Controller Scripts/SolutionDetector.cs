using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class should be used when the player finishes a puzzle level. It will analyse the current state of the beat controllers and 
/// identify which solution the player has found. Using the MusicalSolution scriptable object system, it will then identify the correct
/// composed piece of music to play.
/// </summary>
public class SolutionDetector : MonoBehaviour
{
    /// <summary>
    /// To be called once the player exits the puzzle portion of a level and enters the solely platforming section.
    /// </summary>
    public void OnLevelComplete()
    {
        // This will store the index of the solution the player is using from our possibleSolutions array.
        // If it remains -1, the player has found a new solution to the level.
        int solutionIndex = -1;
        
        // Firstly, we need to go through all the plausible solutions and check them against the current activeBeat setup in the level.
        for (int i = 0; i < BeatTimingManager.btmInstance.currentSong.possibleSolutions.Length; i++)
        {
            // If any of the colour-categorised activeBeat arrays do not match those in the current solution, this will be set to false
            bool foundMismatch = false;

            // We want to loop through all our beat controllers, checking if their current active beats array setup is the same
            // as the boolean array of the same colour within the solution being analysed.
            for (int j = 0; j < BeatTimingManager.btmInstance.beatControllers.Length; j++)
            {
                // We'll loop through all the categories (colours) in our solutions array of beat patterns, finding the one that matches the
                // current beatController colour
                for (int k = 0; k < BeatTimingManager.btmInstance.currentSong.possibleSolutions[i].correctPatterns.Length; k++)
                {
                    // First we make sure we're checking setups of the same colour - we only want to compare arrays of the same category
                    if (BeatTimingManager.btmInstance.currentSong.possibleSolutions[i].correctPatterns[k].beatColour ==
                        BeatTimingManager.btmInstance.beatControllers[j].controllerColour)
                    {
                        // Then we can check if the beat pattern arrays of each are the same
                        if (CompareArrays<bool>(BeatTimingManager.btmInstance.currentSong.possibleSolutions[i].correctPatterns[k].pattern,
                        BeatTimingManager.btmInstance.beatControllers[j].activeBeats) == false)
                        {
                            // if they aren't the same, there's no need to keep loking at this solution - we've already found a mismatch
                            // between it and our current solution. We'll remember that, and exit the loop.
                            foundMismatch = true;
                        }

                        // We've already found the correct coloured pattern to compare, there's no need to look at the rest, so we'll leave
                        // this internal colour-matching loop.
                        break;
                    }
                }

                // Again, as a mismatch has been found when comparing one of the beat controllers to it's relative representation in this 
                // solution, there's no need to keep analysing this solution.
                if (foundMismatch)
                {
                    break;
                }
            }

            // If at this point we haven't found a mismatch, that implies we've checked all the beat controller colours against those
            // in the current solution and found no discrepencies. This is the solution the player used to beat the level.
            if (!foundMismatch)
            {
                solutionIndex = i;
                break;
            }

            // Otherwise, we loop on and check the next possible solution
        }

        // this implies a matching solution was found 
        if (solutionIndex != -1)
        {
            // So we tell our music manager which composition it should play to match said solution
            MusicManager.MMInstance.compositionAudioSource.clip =
                BeatTimingManager.btmInstance.currentSong.possibleSolutions[solutionIndex].solutionComposition;
            MusicManager.MMInstance.OnLevelCompleted();
        }
        else
        {
            // iF no solution found, play default composition and record the unkown solution.
        }
    }

    /// <summary>
    /// Takes in to arrays of a given type, and compares all elements in each of them, checking if they are identical or not.
    /// </summary>
    /// <typeparam name="T">The type of values/objects stored in the 2 arrrays</typeparam>
    /// <param name="array1">The first array to compare with.</param>
    /// <param name="array2">The second array to compare with.</param>
    /// <returns>True if the length and every individual element in both arrays are the same (in value AND index). 
    /// Otherwise, returns false.</returns>
    private bool CompareArrays<T>(T[] array1, T[] array2)
    {
        // First clear signs that the arrays are different is if their lengths are different, so we start by checking that
        if (array1.Length != array2.Length)
        {
            return false;
        }
        else  // If their lengths are the same, we'll have to compare each individual element in each array
        {
            // The lengths are the same, so we can use either length for our For loop
            for (int i = 0; i < array1.Length; i++)
            {
                // != operator does not work on Generic Type T, so we use .Equals() instead
                // If any individual elements in either array differ, the arrays are different and we can return false.
                if (!array1[i].Equals(array2[i]))
                {
                    return false;
                }
            }
        }

        // If we've come this far, then the arrays must be the same. The lengths are the same and no individual elements differe, as 
        // in both these cases, the function would have already returned false.
        // Thus, we can return true.
        return true;
    }

    
}
