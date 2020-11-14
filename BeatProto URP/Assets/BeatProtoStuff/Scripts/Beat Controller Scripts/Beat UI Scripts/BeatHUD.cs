using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatHUD : MonoBehaviour
{
    public BeatController beatController;

    [Tooltip("The IMAGE BEATBLOCK prefab - used to instantiate the correct number of beat blocks for the HUD.")]
    /// <summary>
    /// The IMAGE BEATBLOCK prefab - used to instantiate the correct number of beat blocks for the HUD.
    /// </summary>
    public GameObject imageBeatBlockPrefab;

    [Tooltip("Sprite showing what a beat block should look like if it represents an ACTIVE beat.")]
    /// <summary>
    /// Sprite showing what a beat block should look like if it represents an ACTIVE beat.
    /// </summary>
    public Sprite activeBeatSprite;

    [Tooltip("Sprite showing what a beat block should look like if it represents an INACTIVE beat.")]
    /// <summary>
    /// Sprite showing what a beat block should look like if it represents an INACTIVE beat.
    /// </summary>
    public Sprite inactiveBeatSprite;


    [Tooltip("The colour that will be assigned to each beat block in this HUD.")]
    /// <summary>
    /// The colour that will be assigned to each beat block in this HUD.
    /// </summary>
    public Color hudColour;


    [Tooltip("The space between two consecutive beat blocks.")]
    /// <summary>
    /// The space between two consecutive beat blocks.
    /// </summary>
    public float padding = 0.5f;


    [Tooltip("The width and height each beat block should have.")]
    /// <summary>
    /// The width and height each beat block should have.
    /// </summary>
    public Vector3 beatBlockSize;

    [Tooltip("If true, the beat blocks will be instantiated in a vertical row. \nIf false, the beat blocks will be instantiated in a horizontal row.")]
    /// <summary>
    /// If true, the beat blocks will be instantiated in a vertical row.
    /// If false, the beat blocks will be instantiated in a horizontal row.
    /// </summary>
    public bool verticalHUD = false;


    [Tooltip("If true, when in horizontal format, the left-most beat block will be at local position (0,0,0). \nIf false, the right-most one will be at local position (0,0,0).")]
    /// <summary>
    /// If true, when in horizontal format, the left-most beat block will be at local position (0,0,0).
    /// If false, the right-most one will be at local position (0,0,0).
    /// </summary>
    public bool alignLeft = false;


    [Tooltip("If true, when in vertical format, the highest beat block in the row will be at local position (0,0,0). \nIf false, the lowest beat block will be at local position (0,0,0).")]
    /// <summary>
    /// If true, when in vertical format, the highest beat block in the row will be at local position (0,0,0).
    /// If false, the lowest beat block will be at local position (0,0,0).
    /// </summary>
    public bool alignTop = false;

    [HideInInspector]
    /// <summary>
    /// The collection of images used to represent active and inactive beats on screen.
    /// </summary>
    public Image[] beatBlockImages;

    // Start is called before the first frame update
    void Start()
    {
        // We start by making an array that's big enough to house all the beat blocks we'll need.
        beatBlockImages = new Image[beatController.activeBeats.Length];
        
        // and instantiating a beat block for each of its spaces.
        for (int i = 0; i < beatBlockImages.Length; i++)
        {
            // Note that regardless of alignment, we will assign indeces in our array to allow the player to read left-to-right 
            // and top-to-bottom when reading the active beats on the HUD.
            if (verticalHUD)
            {
                if (alignTop)
                {
                    // We create a new Beat Block. We use negative padding because we want each successive beat block to be lower than the
                    // last one, with the top one being at position (0,0,0).
                    GameObject newBlockObj = 
                        Instantiate(imageBeatBlockPrefab, transform.position + new Vector3(0, i * -1 *padding, 0), 
                        Quaternion.identity,transform) as GameObject;

                    // cache a reference to that new beat block's image
                    beatBlockImages[i] = newBlockObj.GetComponent<Image>();

                    // set the size of the new beat block to the customizable beatBlockSize.
                    beatBlockImages[i].rectTransform.sizeDelta = beatBlockSize;

                    // set the colour of our new beatblock
                    beatBlockImages[i].color = hudColour;
                }
                else
                {
                    // We create a new Beat Block. We use positive padding because we want each successive beat block to be higher than the
                    // last one, with the bottom one being at position (0,0,0).
                    GameObject newBlockObj =
                        Instantiate(imageBeatBlockPrefab, transform.position + new Vector3(0, i * padding, 0),
                        Quaternion.identity, transform) as GameObject;

                    // cache a reference to that new beat block's image. We fill this backwards to allow for top-to-bottom reading of active beats.
                    beatBlockImages[(beatBlockImages.Length - 1) - i] = newBlockObj.GetComponent<Image>();

                    // set the size of the new beat block to the customizable beatBlockSize.
                    beatBlockImages[(beatBlockImages.Length - 1) - i].rectTransform.sizeDelta = beatBlockSize;

                    // set the colour of our new beatblock
                    beatBlockImages[(beatBlockImages.Length - 1) - i].color = hudColour;
                }
            }
            else
            {
                if (alignLeft)
                {
                    // We create a new Beat Block. We use positive padding because we want each successive beat block to be further right 
                    // than the last one, with the leftmost one being at position (0,0,0).
                    GameObject newBlockObj =
                        Instantiate(imageBeatBlockPrefab, transform.position + new Vector3(i * padding, 0, 0),
                        Quaternion.identity, transform) as GameObject;

                    // cache a reference to that new beat block's image
                    beatBlockImages[i] = newBlockObj.GetComponent<Image>();

                    // set the size of the new beat block to the customizable beatBlockSize.
                    beatBlockImages[i].rectTransform.sizeDelta = beatBlockSize;

                    // set the colour of our new beatblock
                    beatBlockImages[i].color = hudColour;

                }
                else
                {
                    // We create a new Beat Block. We use negative padding because we want each successive beat block to be further left 
                    // than the last one, with the rightmost one being at position (0,0,0).
                    GameObject newBlockObj =
                        Instantiate(imageBeatBlockPrefab, transform.position + new Vector3(i * -1 * padding, 0, 0),
                        Quaternion.identity, transform) as GameObject;

                    // cache a reference to that new beat block's image. We fill this backwards to allow for left-to-right reading of active beats.
                    beatBlockImages[(beatBlockImages.Length - 1) - i] = newBlockObj.GetComponent<Image>();

                    // set the size of the new beat block to the customizable beatBlockSize.
                    beatBlockImages[(beatBlockImages.Length - 1) - i].rectTransform.sizeDelta = beatBlockSize;

                    // set the colour of our new beatblock
                    beatBlockImages[(beatBlockImages.Length - 1) - i].color = hudColour;
                }
            }
        }

        UpdateBeatHUD();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBeatHUD();
    }


    /// <summary>
    /// Considers the current state of the beatController's activeBeats array, and changes the image sprites of our beat blocks to
    /// correctly represent which beats are currently active and inactive.
    /// </summary>
    public void UpdateBeatHUD()
    {
        for (int i = 0; i < beatBlockImages.Length; i++)
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


    /*
     * WE AREN"T READY FOR THIS YET - some design decisions need to be made first.
    /// <summary>
    /// Reinitialises the On Screen UI based on the timing manager's current set of beat Controllers.
    /// </summary>
    public void UpdateBeatControllers()
    {
        beatControllers = BeatTimingManager.btmInstance.beatControllers;
    }
    */
}
