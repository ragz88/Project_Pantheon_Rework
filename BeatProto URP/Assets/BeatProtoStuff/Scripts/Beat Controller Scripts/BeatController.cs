using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Allows one to associate a series of actions/functions with a specific array of active beats.
/// There should be one of these present for each TYPE of beat-controlled element in the scene.
/// </summary>
public class BeatController: MonoBehaviour
{
    public bool[] activeBeats;
    
    /// <summary>
    /// The set of actions that should happen each time a beat is present and fires off for elements of this beat controller's type
    /// </summary>
    public UnityEvent OnBeat;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            OnBeat.Invoke();
        }
    }

    /// <summary>
    /// Sets the boolean associated with the given beat index to true in the activeBeats array
    /// </summary>
    public void ActivateBeat(int beatIndex)
    {
        activeBeats[beatIndex] = true;
    }

    /// <summary>
    /// Sets the boolean associated with the given beat index to false in the activeBeats array
    /// </summary>
    public void DeactivateBeat(int beatIndex)
    {
        activeBeats[beatIndex] = false;
    }

}
