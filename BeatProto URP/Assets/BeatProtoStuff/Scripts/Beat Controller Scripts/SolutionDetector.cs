using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class should be used when the player finishes a puzzle level. It will analyse the current state of the beat controllers and 
/// identify which solution the player has found. Using the MusicalSolution scriptable object system, it will then identify the correct
/// composed piece of music to play.
/// </summary>
public class SolutionDetector : MonoBehaviour
{

    public UnityEvent OnSolutionFound;

    /// <summary>
    /// To be called once the player exits the puzzle portion of a level and enters the solely platforming section.
    /// </summary>
    public void OnLevelComplete()
    {
        // This will store the index of the solution the player is using from our possibleSolutions array.
        // If it remains -1, the player has found a new solution to the level.
        int solutionIndex = -1;

        // If a solution is found, it may be the correct pattern but shifted a number of beats forward. This variable will store the number of
        // beats the solution is shifted, allowing us to play the solution audio at the correct time.
        int solutionShift = -1;
        
        // Firstly, we need to go through all the plausible solutions and check them against the current activeBeat setup in the level.
        for (int i = 0; i < BeatTimingManager.btmInstance.currentSong.possibleSolutions.Length; i++)
        {
            
            // The first beat pattern stored in a solution should always be the longest one
            // We create a new bool array that will represent which of the shifted versions of the beat controller's array
            // match our current solution. If any beat controller's shifted pattern at a certain index does not match the solution array of that colour
            // that index will get set to false. Analysing the arry at the end, any indeces still set to 'True' represent matches to our predicted
            // solution that have been shifted 'index' beats forward in time.
            bool[] possibleShifts = new bool[BeatTimingManager.btmInstance.currentSong.possibleSolutions[i].correctPatterns[0].pattern.Length];

            // We'll initialise the whole array as true, then set it's values to false as we find mismatches.
            for (int j = 0; j < possibleShifts.Length; j++)
            {
                possibleShifts[j] = true;
            }


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
                        
                        // We creates a teporary copy of the ActiveBeats array which we can freely shift without affecting the player's current setup.
                        bool[] tempPattern = BeatTimingManager.btmInstance.beatControllers[j].activeBeats;



                        // But we can't just compare the arrays blindly - we need to check if the player has found a solution that's been shifted by some value.
                        for (int h = 0; h < possibleShifts.Length; h++)
                        {
                            // If this index is false, it implies we've already found a mismatch here before and there's no point in checking a different colour's array.
                            if (possibleShifts[h] != false)
                            {
                                // We check if the beat pattern arrays of each are the same
                                if (CompareArrays<bool>(BeatTimingManager.btmInstance.currentSong.possibleSolutions[i].correctPatterns[k].pattern,
                                    tempPattern) == false)
                                {
                                    // If not, we remember that a mimatch was found at this shifted index.
                                    possibleShifts[h] = false;
                                }
                            }
                            
                            ShiftArrayValuesRight<bool>(ref tempPattern);
                        }
                                                
                        // We've already found the correct coloured pattern to compare, there's no need to look at the rest, so we'll leave
                        // this internal colour-matching loop.
                        break;
                    }
                }

                // If all possible shift positions are false already, there's no need to keep analysing this solution.
                // So we loop through our possible shifts array looking for any values that are still true.
                bool allPossibleShiftsMismatched = true;

                for (int k = 0; k < possibleShifts.Length; k++)
                {
                    if (possibleShifts[k])
                    {
                        allPossibleShiftsMismatched = false;
                        break;
                    }
                }

                // If so, no true values were found and we can move on to the next solution.
                if (allPossibleShiftsMismatched)
                {
                    break;
                }

            }


            // At this point, we've either checked all the combinations of beat controllers or we have a shifts array filled with falses. However, if there 
            // is even a single 'true' value in the array, it implies this was the predefined solution the player has found, and it's been shifted by a number
            // of beats equal to the index of that 'true' value.
            bool solutionFound = false;

            for (int k = 0; k < possibleShifts.Length; k++)
            {
                if (possibleShifts[k])
                {
                    solutionFound = true;
                    solutionShift = k;
                    break;
                }
            }

            if (solutionFound)
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
                BeatTimingManager.btmInstance.currentSong.possibleSolutions[solutionIndex].solutionCompositionIntro;
            MusicManager.MMInstance.OnLevelCompleted(solutionIndex, solutionShift);
        }
        else
        {
            // if no solution found, play default composition and record the unkown solution.
            MusicManager.MMInstance.compositionAudioSource.clip =
                BeatTimingManager.btmInstance.currentSong.possibleSolutions[0].solutionCompositionIntro;
            MusicManager.MMInstance.OnLevelCompleted(-1, 0);

            UnexpectedSolutionRecorded.StoreNewSolution();
        }

        OnSolutionFound.Invoke();
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


    /// <summary>
    /// Takes a given array of type T and shifts all it's contents one index to the left - the first element now becomming the new last element
    /// </summary>
    /// <param name="array">Array to be left-shifted</param>
    private void ShiftArrayValuesRight<T>(ref T[] array)
    {
        T[] newArray = new T[array.Length];
        
        for (int i = 0; i < array.Length; i++)
        {
            int newPos = i - 1;

            if (newPos < 0)
            {
                newPos = array.Length - 1;
            }
            
            newArray[newPos] = array[i];
        }

        array = newArray;
    }

    
}
