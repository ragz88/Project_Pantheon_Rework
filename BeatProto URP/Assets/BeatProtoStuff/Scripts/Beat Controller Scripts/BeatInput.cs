using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatInput : MonoBehaviour
{
    [Tooltip("The beat controller which will be changed by this input console. Essentially, which type of environmental assets we want this input console to affect.")]
    /// <summary>
    /// The beat controller which will be changed by this input console. Essentially, which type of environmental assets we want
    /// this input console to affect.
    /// </summary>
    public BeatController beatController;

    [Tooltip("To be shown when the player opens the beat input menu for this BeatInput's specific controller.")]
    /// <summary>
    /// To be shown when the player opens the beat input menu for this BeatInput's specific controller.
    /// </summary>
    public BeatUI inputBeatUI;
    
    // Defines whether the user's inputs (and lack thereof) affect the construction of a beat pattern or not.
    bool editMode = false;


    bool readyForInput = false;

    // This will be set to true if the player opened the beat editor on the 0th beat - 
    // we still want the system to wait for a cycle before unlocking the editor in this case
    bool startedOnZero = false;

    int currentBeat = -1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // TEMPORARY INTERACTION FOR TESTING
        if (Input.GetKeyDown(KeyCode.F))
        {
            OpenEditor();
        }

        if (editMode)
        {
            if (readyForInput)
            {
                // We check if the beat has changed to the next one in the cycle
                if (currentBeat != BeatTimingManager.btmInstance.GetBeatNumber() % beatController.activeBeats.Length)
                {
                    // This represents the moment the beat cycles back to the beginning after the editing run - we want to close the 
                    // menu at this point. 
                    if (BeatTimingManager.btmInstance.GetBeatNumber() % beatController.activeBeats.Length == 0)
                    {
                        CloseEditor();
                    }
                    else // The beat changed, but not back to 0.
                    {
                        currentBeat = BeatTimingManager.btmInstance.GetBeatNumber() % beatController.activeBeats.Length;
                    }
                }

                // Gets user input, activating the current beat if it is present.
                if (Input.GetButtonDown("Sing"))  
                {
                    beatController.activeBeats[currentBeat] = true;
                }
            }
            else
            {
                // Checks if the beat cycle has returned to its starting point - thus being ready for editing
                if ((BeatTimingManager.btmInstance.GetBeatNumber() % beatController.activeBeats.Length) == 0)
                {
                    // This if ensures we don't thrust our player into editing the moment the menu opens
                    if (!startedOnZero)
                    {
                        readyForInput = true;

                        beatController.ClearActiveBeats();

                        inputBeatUI.UpdateInteractableState(true);

                        currentBeat = 0;
                    }
                }
                else
                {
                    // The else implies we're currently on a non-zero beat, meaning that the next time the beat is on 0 it represents a 
                    // point where the cycle is repeating itself
                    startedOnZero = false;
                }
            }
        }
    }


    /// <summary>
    /// Opens the editing UI, allowing the player to enter in a beat pattern.
    /// </summary>
    public void OpenEditor()
    {
        editMode = true;

        // We check if the editor opened while the beat was on the first block - in this case, we'll wait for a cycle to give the
        // player a chance to prepare before having to enter anything in.
        if ((BeatTimingManager.btmInstance.GetBeatNumber() % beatController.activeBeats.Length) == 0)
        {
            startedOnZero = true;
        }
        else
        {
            startedOnZero = false;
        }

        currentBeat = -1;

        inputBeatUI.UIVisible = true;
        inputBeatUI.UpdateInteractableState(false);
    }


    /// <summary>
    /// To be run after the player has had a chance to enter a pattern. Once the final editable beat is passed, the UI will close and that
    /// beat pattern will be locked in.
    /// </summary>
    public void CloseEditor()
    {
        editMode = false;
        readyForInput = false;

        inputBeatUI.UpdateInteractableState(false);
        inputBeatUI.UIVisible = false;
    }
}
