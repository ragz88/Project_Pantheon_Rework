using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays a visual representation of the timing and setup of a specific beat controller, and hence a specific 
/// set of beat-controlled environmental elements
/// </summary>
public class BeatUI : MonoBehaviour
{
    [Tooltip("The BeatController whose information this UI should represent.")]
    /// <summary>
    /// The BeatController whose information this UI should represent.
    /// </summary>
    public BeatController beatController;


    [Tooltip("The images used to display the beat blocks for a specific beat controller.")]
    /// <summary>
    /// The images used to display the beat blocks for a specific beat controller
    /// </summary>
    public Image[] beatBlockImages;


    [Tooltip("This should be set to true when the player opens the editor interface.")]
    /// <summary>
    /// This should be set to true when the player opens the editor interface
    /// </summary>
    public bool UIVisible = false;


    [Tooltip("Sprite to be show if a beat in a beat pattern is filled - ie. the OnBeat event for this beat WILL fire.")]
    /// <summary>
    /// Sprite to be show if a beat in a beat pattern is filled - ie. the OnBeat event for this beat WILL fire
    /// </summary>
    public Sprite activeBeatSprite;


    [Tooltip("Sprite to be show if a beat in a beat pattern is empty - ie. the OnBeat event for this beat WON'T fire.")]
    /// <summary>
    /// Sprite to be show if a beat in a beat pattern is empty - ie. the OnBeat event for this beat WON'T fire
    /// </summary>
    public Sprite inactiveBeatSprite;


    [SerializeField]
    /// <summary>
    /// Speed at which the UI beat blocks fade in
    /// </summary>
    float fadeInSpeed = 0.3f;

    [SerializeField]
    /// <summary>
    /// Speed at which the UI beat blocks fade out
    /// </summary>
    float fadeOutSpeed = 1f;

    // Used to show whether the UI is ready for player interaction or not
    private Color initialImageColour;


    private void Start()
    {
        initialImageColour = beatBlockImages[0].color;

        //UpdateInteractableState(false);
    }

    // Update is called once per frame
    void Update()
    {
        #region Fade UI in and out

        // We'll check if the UI is meant to be showing, and if the last black in our UI array is fully visible yet.
        if (UIVisible && beatBlockImages[beatBlockImages.Length - 1].color.a < 1)
        {
            // If it isn't, we'll increase all our beat blocks' alphas
            for (int i = 0; i < beatBlockImages.Length; i++)
            {
                beatBlockImages[i].color = beatBlockImages[i].color + new Color(0, 0, 0, fadeInSpeed * Time.deltaTime);
            }
        }
        // If it's not meant to be visible, we'll check if the last block is completely hidden
        else if (!UIVisible && beatBlockImages[beatBlockImages.Length - 1].color.a > 0)
        {
            // If it isn't, we'll decrease all our beat blocks' alphas
            for (int i = 0; i < beatBlockImages.Length; i++)
            {
                beatBlockImages[i].color = beatBlockImages[i].color - new Color(0, 0, 0, fadeOutSpeed * Time.deltaTime);
            }
        }
        #endregion

        UpdateBeatUI();
    }

    /// <summary>
    /// Checks the linked beat controller's activeBeats array, and fills in the blocks which represent beats the player has defined as
    /// active. LATER: TO ONLY BE CALLED WHEN PLAYER INPUTS A CHANGE FOR THIS SYSTEM
    /// </summary>
    public void UpdateBeatUI()
    {
        for (int i = 0; i < beatController.activeBeats.Length; i++)
        {
            if (beatController.activeBeats[i])
            {
                beatBlockImages[i].sprite = activeBeatSprite;
            }
            else
            {
                beatBlockImages[i].sprite = inactiveBeatSprite;
            }
        }
    }


    /// <summary>
    /// Changes the colour of the UI based on whether it's ready for player input or not (defined by the given bool)
    /// </summary>
    /// <param name="newState">Whether the UI is ready for input (true) or not (false)</param>
    public void UpdateInteractableState(bool newState)
    {
        if (newState == true)
        {
            for (int i = 0; i < beatBlockImages.Length; i++)
            {
                beatBlockImages[i].color = initialImageColour;
            }
        }
        else
        {
            for (int i = 0; i < beatBlockImages.Length; i++)
            {
                beatBlockImages[i].color = Color.gray;
            }
        }
    }
}
