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


    [Tooltip("If true, the input UI will close immediately after the player has had a single cycle to input beats. \nIf false, it will only close once they choose to move away.")]
    /// <summary>
    /// If true, the input UI will close immediately after the player has had a single cycle to input beats.
    /// If false, it will only close once they choose to move away.
    /// </summary>
    public bool closeUIAfterOneCycle = false;


    [Tooltip("If this is true, the pattern input will use the top beat (first in the cycle) as it's reset point. If it's false, the first beat a player enters will be treated as the reset point.")]
    /// <summary>
    /// If this is true, the pattern input will use the top beat (first in the cycle) as it's reset point. If it's false,
    /// the first beat a player enters will be treated as the reset point.
    /// </summary>
    public bool resetAtTop = true;


    [Tooltip("When true, the beat input module does not require a nearby player to activate it.")]
    /// <summary>
    /// When true, the beat input module does not require a nearby player to activate it.
    /// </summary>
    public bool allowTestingInput = false;


    [Tooltip("If true, the moment the player makes a new beat active, the event linked to that active beat will trigger. If false, the cycle will have to return to an active beat before it is invoked.")]
    /// <summary>
    /// If true, the moment the player makes a new beat active, the event linked to that active beat will trigger. If false,
    /// the cycle will have to return to an active beat before it is invoked.
    /// </summary>
    public bool triggerOnBeatOnInput = false;

    /// <summary>
    /// Only used when resetAtTop is false. Stores the first beat the player interacts with in a pattern, and disables player input once
    /// this beat is reached again. Possibly more comfortable experience for the user.
    /// </summary>
    private int resetBeatNum = -1;

    // Defines whether the user's inputs (and lack thereof) affect the construction of a beat pattern or not.
    private bool editMode = false;


    private bool readyForInput = false;

    // This will be set to true if the player opened the beat editor on the 0th beat - 
    // we still want the system to wait for a cycle before unlocking the editor in this case
    private bool startedOnZero = false;

    private int currentBeat = -1;

    //new variables for Micky. Seperating the UI from the Beat Input console.
    private bool playerInRange = false;


    /// <summary>
    /// Micky's sprite representing when the player is close enough to input song notes
    /// </summary>
    public SpriteRenderer inputPossibleSprite;


    [Tooltip("The x velocity the player needs to reach before the terminal will close from them walking away.")]
    /// <summary>
    /// The x velocity the player needs to reach before the terminal will close from them walking away.
    /// </summary>
    public float terminalCloseSensitivity = 1.5f;


    /// <summary>
    /// Used to detect changes in the GetAxisRaw result between frames, to simulate the effect of a GetButtonDown for a movement axis.
    /// </summary>
    float previousXAxisRaw = 0;


    /// <summary>
    /// Used to prevent the OnBeat event from being invoked by the player opening the menu. Only relevant if triggerOnBeatInput is true.
    /// </summary>
    bool ignoreOpeningInput = false;

    [Tooltip("For specific types of environmental objects, we may want them to run their OnBeat effect when all the beats are cleared. They will do so if this is true.")]
    /// <summary>
    /// For specific types of environmental objects, we may want them to run their OnBeat effect when all the beats are cleared. They will
    /// do so if this is true.
    /// </summary>
    public bool InvokeOnBeatOnClear = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Here we detect any player inputs that imply they want to close the terminal.
        if ((Input.GetButtonDown("Jump")))
        {
            CloseEditor();
        }
        else
        {
            // Essentially, was this a deliberate input rather than some risidual velocity from a previous movement.
            // We've replicated the effects of a GetButtonDown for an axis (rather than a GetButton)
            if ((Input.GetAxisRaw("Horizontal") != 0 && !(previousXAxisRaw > 0.1f || previousXAxisRaw < -0.1f)))
            {
                CloseEditor();
            }
        }

        // Update our 'previous frame' GetAxisRaw, as we've already done all the checks we needed to use it for.
        previousXAxisRaw = Input.GetAxisRaw("Horizontal");

        if (editMode)
        {
            if (readyForInput)
            {
                // We check if the beat has changed to the next one in the cycle
                if (currentBeat != BeatTimingManager.btmInstance.GetBeatNumber() % beatController.activeBeats.Length)
                {
                    // This represents the moment the beat cycles back to the beginning after the editing run - we want to close the 
                    // menu at this point if closeUIAfterOneCycle is true. 
                    if ((BeatTimingManager.btmInstance.GetBeatNumber() % beatController.activeBeats.Length == 0 && resetAtTop) ||
                        (BeatTimingManager.btmInstance.GetBeatNumber() % beatController.activeBeats.Length == resetBeatNum && !resetAtTop))
                    {
                        if (closeUIAfterOneCycle)
                        {
                            CloseEditor();
                        }
                        else
                        {
                            // Resets the editor to it's initial state, giving the player a moment of peace in which they can review their
                            // pattern or safely leave withot fear that everything will reset.
                            ResetEditor();
                        }
                    }
                    else // The beat changed, but not back to 0.
                    {
                        currentBeat = BeatTimingManager.btmInstance.GetBeatNumber() % beatController.activeBeats.Length;
                    }
                }

                // Gets user input, activating the current beat if it is present.
                if (Input.GetButtonDown("Sing") || Input.GetButtonDown("SingTemp"))
                {
                    beatController.activeBeats[currentBeat] = true;

                    // As the player makes a beat active, the active beat's effect will take place if this is set to true.
                    if (triggerOnBeatOnInput && !ignoreOpeningInput)
                    {
                        beatController.OnBeat.Invoke();
                    }

                    // If this is the first beat the player's entered, and we aren't resetting on the 0th beat, we want to store its index
                    if (!resetAtTop && resetBeatNum == -1)
                    {
                        resetBeatNum = currentBeat;

                        // We also update this beat's sprite, so that the player knows it's special
                        inputBeatUI.DesignateResetBeatMarker(resetBeatNum);
                    }
                }
            }
            else
            {
                int tempResetPoint = 0;

                if (!resetAtTop && resetBeatNum != -1)
                {
                    tempResetPoint = resetBeatNum;
                }

                // Checks if the beat cycle has returned to its starting point - thus being ready for editing
                if ((BeatTimingManager.btmInstance.GetBeatNumber() % beatController.activeBeats.Length) == tempResetPoint)
                {
                    // This if ensures we don't thrust our player into editing the moment the menu opens
                    if (!startedOnZero)
                    {
                        readyForInput = true;

                        beatController.ClearActiveBeats();

                        // As the beats are reset, the OnBeat effect will take place if this is set to true.
                        if (triggerOnBeatOnInput && InvokeOnBeatOnClear)
                        {
                            beatController.OnBeat.Invoke();
                        }

                        inputBeatUI.UpdateInteractableState(true);

                        currentBeat = tempResetPoint;

                        //Before losing our reference to the previous beat reset number, let's turn it back into a regular beatBlock
                        if (resetBeatNum != -1)
                        {
                            inputBeatUI.RevertToStandardSprite(resetBeatNum);

                            // and then we reset our resetBeatNumber, as the player must now reassign it.
                            resetBeatNum = -1;
                        }
                    }
                }
                else
                {
                    // As we are currently on a non-resetBeatNumber point, we know we must have already passed that point at least once,
                    // or we did not start on it to begin with. Hence, we can tell the above algorithm that the next time we pass this point,
                    // the player has had enough time to prepare and should be ready to input again.
                    startedOnZero = false;
                }

            }
        }

        // TEMPORARY INTERACTION FOR TESTING
        if (Input.GetButtonDown("Sing") || Input.GetButtonDown("SingTemp"))
        {
            if (playerInRange || allowTestingInput)
            {
                OpenEditor();
            }
        }

        // Tells our script that a full frame has run since the editor was opened, preventing the opening press of the Sing button
        // from triggering an OnBeat event.
        ignoreOpeningInput = false;
    }


    /// <summary>
    /// Opens the editing UI, allowing the player to enter in a beat pattern.
    /// </summary>
    public void OpenEditor()
    {
        if (!editMode)
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

            ignoreOpeningInput = true;

            if (!closeUIAfterOneCycle)
            {
                inputBeatUI.UpdateInteractableState(true);
                readyForInput = true;
                beatController.ClearActiveBeats();

                // As the beats are reset, the OnBeat effect will take place if this is set to true.
                if (triggerOnBeatOnInput && InvokeOnBeatOnClear)
                {
                    beatController.OnBeat.Invoke();
                }
            }
            else
            {
                inputBeatUI.UpdateInteractableState(false);
            }
            

            // We make sure the old resetBeatNum is downgraded to a standard beatblock sprite again.
            if (resetBeatNum != -1)
            {
                inputBeatUI.RevertToStandardSprite(resetBeatNum);
            }

            resetBeatNum = -1;
        }
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


    /// <summary>
    /// To be called after a player finishes entering a pattern - sets the interactable state of the editor to false, but conserves the
    /// resetBeat, allowing the beatInput interaction to happen again in a loop after a single cycle of review
    /// </summary>
    public void ResetEditor()
    {
        readyForInput = false;

        inputBeatUI.UpdateInteractableState(false);

        startedOnZero = true;
    }


    public void SetPlayerInRange(bool inRange)
    {
        playerInRange = inRange;
    }

    /// <summary>
    /// If the console the player approaches has a sprite that only shows when the player approaches, this function tells it to appear. As UI is not yet
    /// finalised, this will not necessarily be used in every scene.
    /// </summary>
    /// <param name="inputState"></param>
    public void TogglePossibleInputImage(bool inputState)
    {
        if (inputPossibleSprite != null)
        {
            inputPossibleSprite.enabled = inputState;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;

            if (inputPossibleSprite != null)
            {
                inputPossibleSprite.enabled = true;
            }
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        playerInRange = false;

        if (inputPossibleSprite != null)
        {
            inputPossibleSprite.enabled = false;
        }

        CloseEditor();
    }
}
