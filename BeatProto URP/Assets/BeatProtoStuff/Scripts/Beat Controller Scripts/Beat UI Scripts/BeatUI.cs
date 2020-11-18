using UnityEngine;

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


    [Tooltip("This UI Component will automatically spawn in Beat Nodes around it's central circle - this is the prefab of a beat Block it will use to instantiate these nodes.")]
    /// <summary>
    /// This UI Component will automatically spawn in Beat Nodes around it's central circle - this is the prefab of a beat Block
    /// it will use to instantiate these nodes.
    /// </summary>
    public GameObject beatBlockPrefab;

    [Tooltip("The colour that should be given to the beat block sprites when they're ready for input.")]
    /// <summary>
    /// The colour that should be given to the beat block sprites when they're ready for input.
    /// </summary>
    public Color beatBlockColour;

    [Tooltip("Sprite that represents the first beat the player interacted with - which will be where the interactable state of the input module changes.\nThis will be used when that beat is an active one.")]
    /// <summary>
    /// Sprite that represents the first beat the player interacted with - which will be where the interactable state of the input module changes.
    /// This will be used when that beat is an active one.
    /// </summary>
    public Sprite resetPositionSpriteActive;

    [Tooltip("Sprite that represents the first beat the player interacted with - which will be where the interactable state of the input module changes.\nThis will be used when that beat is an inactive one.")]
    /// <summary>
    /// Sprite that represents the first beat the player interacted with - which will be where the interactable state of the input module changes.
    /// This will be used when that beat is an inactive one.
    /// </summary>
    public Sprite resetPositionSpriteInactive;


    [Tooltip("Sometimes, one may want an image behind all the beat blocks to frame them better. This allows the Beat UI to fade this sprite in and out as well.")]
    /// <summary>
    /// Sometimes, one may want an image behind all the beat blocks to frame them better, or a cursor rotating around them. 
    /// This allows the Beat UI to fade these sprite in and out as well.
    /// </summary>
    public SpriteRenderer[] additionalSprites;


    [Tooltip("The radius of the circular path the beat blocks will be instantiated on.")]
    /// <summary>
    /// The radius of the circular path the beat blocks will be instantiated on.
    /// </summary>
    public float radius = 5;

    [Tooltip("If ever you want the prefab of a beat block to be instantiated a little larger or smaller, this float represents a multiplier for the beat block size. 1 will keep the prefab's native size.")]
    /// <summary>
    /// If ever you want the prefab of a beat block to be instantiated a little larger or smaller, this float represents a multiplier for
    /// the beat block size. 1 will keep the prefab's native size.
    /// </summary>
    public float beatBlockRescale = 1;


    [HideInInspector]
    /// <summary>
    /// The sprites used to display the beat blocks for a specific beat controller
    /// </summary>
    public SpriteRenderer[] beatBlockSprites;


    [HideInInspector]
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

    /// <summary>
    /// If true, the beat block UI elements will be rotated such that their bottoms all point towards the centre of the circular path
    /// they're spawned on.
    /// </summary>
    public bool alignBeatsWithCentre = true;

    // Used by UI adon classes like the beat cursor and pulse to ensure they don't try to access a beatblock that hasn't been
    // instantiated yet.
    public bool UIReady = false;


    [SerializeField]
    /// <summary>
    /// Speed at which the UI beat blocks fade in
    /// </summary>
    private float fadeInSpeed = 0.3f;

    [SerializeField]
    /// <summary>
    /// Speed at which the UI beat blocks fade out
    /// </summary>
    private float fadeOutSpeed = 1f;

    // Used to show whether the UI is ready for player interaction or not
    private Color initialImageColour;

    // Used to show whether UI is ready for interaction or not, only changes base image colour
    private Color[] initialAdditionalSpriteColours;

    private void Start()
    {
        // Get our beatBlock array ready
        beatBlockSprites = new SpriteRenderer[beatController.activeBeats.Length];

        // Instantiate our Beat Blocks and keep a reference to each of their sprite renderers
        for (int i = 0; i < beatBlockSprites.Length; i++)
        {
            // Here, we calculate the current position of this beat block in what are essentially polar coordinates
            // We'll get an angle representing it's current rotation amount around the circle. This, combined with our 
            // radius can be used to calculate its x and y positions. Note that the angle must be in radians, not degrees, hence the
            // 2PI rather than 360.
            float currentAngle = i * ((2 * Mathf.PI) / beatBlockSprites.Length);

            // Finding x and y coordinates with simple trig
            float currentYOffset = radius * Mathf.Cos(currentAngle);
            float currentXOffset = radius * Mathf.Sin(currentAngle);

            // Instantiate block in this position.
            GameObject currentBeatBlockObj = Instantiate(beatBlockPrefab, transform.position + new Vector3(currentXOffset, currentYOffset, 0),
                Quaternion.identity, transform) as GameObject;

            // Adjust instantiated beat block's size based on public multiplier.
            currentBeatBlockObj.transform.localScale = currentBeatBlockObj.transform.localScale * beatBlockRescale;

            beatBlockSprites[i] = currentBeatBlockObj.GetComponent<SpriteRenderer>();

            // Here we rotate each beatblock to point at the centre of the circle they lie on (if this setting is true).
            if (alignBeatsWithCentre)
            {
                // Note that Euler angles are measures in degrees, not radians (unlike the Cos and Sin functions)
                beatBlockSprites[i].transform.eulerAngles = new Vector3(0, 0, (beatBlockSprites.Length - i) * (360 / beatBlockSprites.Length));
            }
        }

        // Cache a reference to our initial colours, so we can revert to them later when things are in an interactable state.
        initialImageColour = new Color(beatBlockColour.r, beatBlockColour.g, beatBlockColour.b, 1);

        initialAdditionalSpriteColours = new Color[additionalSprites.Length];

        if (additionalSprites.Length != 0)
        {
            for (int i = 0; i < additionalSprites.Length; i++)
            {
                initialAdditionalSpriteColours[i] = new Color(additionalSprites[i].color.r, additionalSprites[i].color.g, additionalSprites[i].color.b, 1);
            }
        }

        // Tells our add-on UI classes they can start doing their jobs
        UIReady = true;

        //UpdateInteractableState(false);
    }

    // Update is called once per frame
    void Update()
    {
        #region Fade UI in and out

        // We'll check if the UI is meant to be showing, and if the last black in our UI array is fully visible yet.
        if (UIVisible && (beatBlockSprites[beatBlockSprites.Length - 1].color.a < 1 || additionalSprites[additionalSprites.Length - 1].color.a < 1))
        {
            // If it isn't, we'll increase all our beat blocks' alphas
            for (int i = 0; i < beatBlockSprites.Length; i++)
            {
                if (beatBlockSprites[i].color.a < 1)
                {
                    beatBlockSprites[i].color = beatBlockSprites[i].color + new Color(0, 0, 0, fadeInSpeed * Time.deltaTime);
                }
            }

            // Fade in the additional UI sprite, like background UI and cursors, if they exists
            for (int i = 0; i < additionalSprites.Length; i++)
            {
                if (additionalSprites[i].color.a < 1)
                {
                    additionalSprites[i].color = additionalSprites[i].color + new Color(0, 0, 0, fadeInSpeed * Time.deltaTime);
                }
            }
        }
        // If it's not meant to be visible, we'll check if the last block is completely hidden
        else if (!UIVisible && (beatBlockSprites[beatBlockSprites.Length - 1].color.a > 0 || additionalSprites[additionalSprites.Length - 1].color.a > 0))
        {
            // If it isn't, we'll decrease all our beat blocks' alphas
            for (int i = 0; i < beatBlockSprites.Length; i++)
            {
                if (beatBlockSprites[i].color.a > 0)
                {
                    beatBlockSprites[i].color = beatBlockSprites[i].color - new Color(0, 0, 0, fadeOutSpeed * Time.deltaTime);
                }                
            }

            // Fade out the additional UI sprite, like background UI and cursors, if they exists
            for (int i = 0; i < additionalSprites.Length; i++)
            {
                if (additionalSprites[i].color.a > 0)
                {
                    additionalSprites[i].color = additionalSprites[i].color - new Color(0, 0, 0, fadeOutSpeed * Time.deltaTime);
                }
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
                // The type of aactive beat sprite we assign depends on if this is the ResetPosition marker or not, so we check
                // the current state of the sprite before assigning it.
                if (beatBlockSprites[i].sprite == inactiveBeatSprite || beatBlockSprites[i].sprite == activeBeatSprite)
                {
                    beatBlockSprites[i].sprite = activeBeatSprite;
                }
                else
                {
                    beatBlockSprites[i].sprite = resetPositionSpriteActive;
                }
            }
            else
            {
                // The type of aactive beat sprite we assign depends on if this is the ResetPosition marker or not, so we check
                // the current state of the sprite before assigning it.
                if (beatBlockSprites[i].sprite == inactiveBeatSprite || beatBlockSprites[i].sprite == activeBeatSprite)
                {
                    beatBlockSprites[i].sprite = inactiveBeatSprite;
                }
                else
                {
                    beatBlockSprites[i].sprite = resetPositionSpriteInactive;
                }
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
            // Make our beat blocks their initial, bright colour, iplying interaction is allowed
            for (int i = 0; i < beatBlockSprites.Length; i++)
            {
                beatBlockSprites[i].color = new Color(initialImageColour.r, initialImageColour.g, initialImageColour.b, beatBlockSprites[i].color.a);
            }

            // Update additional sprite colours as well, like cursors or background images
            for (int i = 0; i < additionalSprites.Length; i++)
            {
                additionalSprites[i].color = new Color(initialAdditionalSpriteColours[i].r, initialAdditionalSpriteColours[i].g,
                    initialAdditionalSpriteColours[i].b, additionalSprites[i].color.a);
            }
        }
        else
        {
            // Make our beat blocks ta dull grey, iplying interaction is not allowed
            for (int i = 0; i < beatBlockSprites.Length; i++)
            {
                beatBlockSprites[i].color = new Color(Color.gray.r, Color.gray.g, Color.gray.b, beatBlockSprites[i].color.a);
            }

            // Update additional sprite colours as well, like cursors or background images
            for (int i = 0; i < additionalSprites.Length; i++)
            {
                additionalSprites[i].color = new Color(Color.gray.r, Color.gray.g, Color.gray.b, additionalSprites[i].color.a);
            }
        }
    }


    /// <summary>
    /// Should be used to reset a beatblock that has been flagged as the ResetBeat Marker - turning it back into a regular beatblock.
    /// </summary>
    /// <param name="index">The index of the beatblock to reset from our beatBlockSprites array.</param>
    public void RevertToStandardSprite(int index)
    {
        // If true, the sprite returning to standard form represents an active beat.
        if (beatBlockSprites[index].sprite == resetPositionSpriteActive)
        {
            beatBlockSprites[index].sprite = activeBeatSprite;
        }
        else
        {
            beatBlockSprites[index].sprite = inactiveBeatSprite;
        }


    }

    /// <summary>
    /// Should be used when the reset position of the cycle changes, showing the player what the first beat they interacted with was.
    /// </summary>
    /// <param name="index">The index of the beatblock to change into the ResetBeat Marker from our beatBlockSprites array.</param>
    public void DesignateResetBeatMarker(int index)
    {
        // If true, the sprite that will represent the reset position represents an active beat.
        if (beatBlockSprites[index].sprite == activeBeatSprite)
        {
            beatBlockSprites[index].sprite = resetPositionSpriteActive;
        }
        else
        {
            beatBlockSprites[index].sprite = resetPositionSpriteInactive;
        }
    }
}
