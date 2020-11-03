﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Allows one to associate a series of actions/functions with a specific array of active beats.
/// There should be one of these present for each TYPE of beat-controlled element in the scene.
/// </summary>
public class BeatController: MonoBehaviour
{
    /// <summary>
    /// Defines which elements in the environment this controller is linked to. Important for detecting if the player has found a 
    /// feasible solution, and for playing the correct sounds with those elements.
    /// </summary>
    public BeatElementColour controllerColour;
    
    /// <summary>
    /// This array represents which beats the user has defined as active - and thus on these beats the OnBeat event will fire.
    /// </summary> 
    public bool[] activeBeats;
    
    /// <summary>
    /// The set of actions that should happen each time a beat is present and fires off for elements of this beat controller's type
    /// </summary>
    public UnityEvent OnBeat;


    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Sets the boolean associated with the given beat index to true in the activeBeats array
    /// </summary>
    /*public void ActivateBeat(int beatIndex)
    {
        activeBeats[beatIndex] = true;
    }

    /// <summary>
    /// Sets the boolean associated with the given beat index to false in the activeBeats array
    /// </summary>
    public void DeactivateBeat(int beatIndex)
    {
        activeBeats[beatIndex] = false;
    }*/

    /// <summary>
    /// Sets all beats in our activeBeats array to false
    /// </summary>
    public void ClearActiveBeats()
    {
        for (int i = 0; i < activeBeats.Length; i++)
        {
            activeBeats[i] = false;
        }
    }

}
