using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProgressor : MonoBehaviour
{
    public int currentLevel = 0;

    // Containers for all the necessary objects for each level.
    public GameObject[] levelObjects;

    [System.Serializable]
    public struct ControllerArray
    {
        public BeatController[] beatControllerArray;

        public SolutionArray[] solutions;
    }


    [System.Serializable]
    public struct SolutionArray
    {
        public bool[] solutionPattern;
    }


    public ControllerArray[] beatControllerArrays;


    public ShowMessage messanger;

    public string[] levelEndMessages;

    [System.Serializable]
    public struct MovementGhostPairs
    {
        public MovingObject[] actualObjects;
        public MovingObject[] ghosts;
    }

    public MovementGhostPairs[] movementGhostPairs;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckSolution();
    }



    public void CheckSolution()
    {
        bool levelSolved = true;

        for (int i = 0; i < beatControllerArrays[currentLevel].beatControllerArray.Length; i++)
        {
            if (!CompareArrays<bool>( beatControllerArrays[currentLevel].beatControllerArray[i].activeBeats, 
                beatControllerArrays[currentLevel].solutions[i].solutionPattern))
            {
                 levelSolved = false;
            }
        }

        for (int i = 0; i < movementGhostPairs[currentLevel].actualObjects.Length; i++)
        {
            if (movementGhostPairs[currentLevel].actualObjects[i].currentMovePoint != movementGhostPairs[currentLevel].ghosts[i].currentMovePoint)
            {
                levelSolved = false;
            }
        }

        if (levelSolved && !IsInvoking("OnLevelSolved"))
        {
            Invoke("OnLevelSolved", 3);
            messanger.DisplayMessage(levelEndMessages[currentLevel]);

        }

    }


    public void OnLevelSolved()
    {
        if (currentLevel < levelObjects.Length - 1)
        {
            levelObjects[currentLevel].SetActive(false);
            currentLevel++;
            levelObjects[currentLevel].SetActive(true);
            BeatTimingManager.btmInstance.beatControllers = beatControllerArrays[currentLevel].beatControllerArray;
        }
        else
        {
            // NEXT SCENE NEXT SCENE NEXT SCENE NEXT SCENE NEXT SCENE NEXT SCENE NEXT SCENE NEXT SCENE NEXT SCENE NEXT SCENE NEXT SCENE 
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
