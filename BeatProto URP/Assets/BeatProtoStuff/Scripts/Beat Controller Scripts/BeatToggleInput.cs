using UnityEngine;

public class BeatToggleInput : MonoBehaviour
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

    [Tooltip("If true, the moment the player makes a beat inactive that was already active, the event linked to that beat will trigger.")]
    /// <summary>
    /// If true, the moment the player makes a beat inactive that was already active, the event linked to that beat will trigger.
    /// </summary>
    public bool triggerOnBeatOnClearInput = false;


    // Defines whether the user's inputs (and lack thereof) affect the construction of a beat pattern or not.
    // True when the interface is open.
    private bool editMode = false;

    private int currentBeat = -1;

    // new variables for Micky. Seperating the UI from the Beat Input console.
    private bool playerInRange = false;


    /// <summary>
    /// Used to prevent the OnBeat event from being invoked by the player opening the menu. Only relevant if triggerOnBeatInput is true.
    /// </summary>
    bool ignoreOpeningInput = false;


    [Tooltip("The horizontal axis value the player needs to reach before the terminal will close from them walking away.")]
    /// <summary>
    /// The horizontal axis value the player needs to reach before the terminal will close from them walking away.
    /// </summary>
    public float terminalCloseSensitivity = 0.1f;


    /// <summary>
    /// Used to detect changes in the GetAxisRaw result between frames, to simulate the effect of a GetButtonDown for a movement axis.
    /// </summary>
    float previousXAxisRaw = 0;


    /// <summary>
    /// Micky's sprite representing when the player is close enough to input song notes
    /// </summary>
    public SpriteRenderer inputPossibleSprite;


    public bool multipleTriesPerBeat = false;

    bool inputGivenThisBeat = false;


    // Update is called once per frame
    void Update()
    {
        if (editMode)
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
                if ((Input.GetAxisRaw("Horizontal") != 0 && !(previousXAxisRaw > terminalCloseSensitivity || previousXAxisRaw < -terminalCloseSensitivity)))
                {
                    CloseEditor();
                }
            }



            // We check if the beat has changed to the next one in the cycle - this detects the moment this happens
            if (currentBeat != BeatTimingManager.btmInstance.GetBeatNumber() % beatController.activeBeats.Length)
            {
                if (!multipleTriesPerBeat)
                {
                    // This will be used to check if the player is still allowed to enter a beat at this point
                    inputGivenThisBeat = false;
                }

                // We also take the oppportunity to update our current beat.
                currentBeat = BeatTimingManager.btmInstance.GetBeatNumber() % beatController.activeBeats.Length;
            }

            // Gets user input, activating the current beat if it is present.
            if (Input.GetButtonDown("Sing") || Input.GetButtonDown("SingTemp"))
            {
                if (multipleTriesPerBeat)
                {
                    // We change the current beat's state to its opposite, effectively toggling its state.
                    beatController.activeBeats[currentBeat] = !beatController.activeBeats[currentBeat];

                    // As the player makes a beat active, the active beat's effect will take place if this is set to true.
                    if (triggerOnBeatOnInput && !ignoreOpeningInput)
                    {
                        beatController.OnBeat.Invoke();
                    }
                }
                else
                {
                    // We check if the player has already tried to enter something this beat - preventing them from toggling the beat multiple times.
                    if (!inputGivenThisBeat)
                    {
                        // We change the current beat's state to its opposite, effectively toggling its state.
                        beatController.activeBeats[currentBeat] = !beatController.activeBeats[currentBeat];

                        inputGivenThisBeat = true;

                        // As the player makes a beat active, the active beat's effect will take place if this is set to true.
                        if (triggerOnBeatOnInput && !ignoreOpeningInput && (beatController.activeBeats[currentBeat] || triggerOnBeatOnClearInput))
                        {
                            beatController.OnBeat.Invoke();
                        }
                    }
                }
            }
        }
        else
        {
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
    }


    /// <summary>
    /// To be run after the player has had a chance to enter a pattern. Once the final editable beat is passed, the UI will close and that
    /// beat pattern will be locked in.
    /// </summary>
    public void CloseEditor()
    {
        editMode = false;

        inputBeatUI.UpdateInteractableState(false);
        inputBeatUI.UIVisible = false;
    }


    public void OpenEditor()
    {
        editMode = true;

        currentBeat = -1;

        inputBeatUI.UIVisible = true;

        ignoreOpeningInput = true;

        inputBeatUI.UpdateInteractableState(true);
    }


    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = true;

            if (inputPossibleSprite != null)
            {
                inputPossibleSprite.enabled = true;
            }
        }
    }


    private void OnTriggerExit(Collider collision)
    {
        playerInRange = false;

        if (inputPossibleSprite != null)
        {
            inputPossibleSprite.enabled = false;
        }

        CloseEditor();
    }
}
