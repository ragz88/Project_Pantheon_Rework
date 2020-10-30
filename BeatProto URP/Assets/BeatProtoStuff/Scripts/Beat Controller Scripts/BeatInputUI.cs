using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatInputUI : MonoBehaviour
{
    /// <summary>
    /// The images used to display the beat blocks for a specific beat controller
    /// </summary>
    public Image[] beatBlockImages;

    /// <summary>
    /// This should be set to true when the player opens the editor interface
    /// </summary>
    public bool UIVisible = false;

    /// <summary>
    /// Sprite to be show if a beat in a beat pattern is filled - ie. the OnBeat event for this beat WILL fire
    /// </summary>
    public Sprite activeBeatSprite;

    /// <summary>
    /// Sprite to be show if a beat in a beat pattern is empty - ie. the OnBeat event for this beat WON'T fire
    /// </summary>
    public Sprite inactiveBeatSprite;


    [SerializeField]
    /// <summary>
    /// The size a UI beat block should increase to when it is the current beat
    /// </summary>
    float blockScaleIncrease = 1.2f;

    [SerializeField]
    /// <summary>
    /// Speed at which the UI beat blocks fade in and out
    /// </summary>
    float fadeInSpeed = 0.3f;

    [SerializeField]
    /// <summary>
    /// Speed at which blocks scale to their full feedback size when they're the block representing the currently firing beat
    /// </summary>
    float blockScaleSpeed = 0.3f;

    // Each beat block has a particle system that will be played when it is the currenty active block (based on BeatTimingManager's 
    // beat number)
    private ParticleSystem[] beatBlockParticles;

    // Will be used to scale the beat blocks for feedback purposes
    private Vector3 initialBlockScale;

    
    // Start is called before the first frame update
    void Start()
    {
        // We store our initial scale so we can return to it later
        initialBlockScale = beatBlockImages[0].transform.localScale;

        // We also extract a reference to the particle systems on each of our beat blocks
        beatBlockParticles = new ParticleSystem[beatBlockImages.Length];
        for (int i = 0; i < beatBlockImages.Length; i++)
        {
            beatBlockParticles[i] = beatBlockImages[i].gameObject.GetComponent<ParticleSystem>();
        }
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
                beatBlockImages[i].color = beatBlockImages[i].color - new Color(0, 0, 0, fadeInSpeed * Time.deltaTime);
            }
        }
        #endregion

        #region Highlighting current Beat Block
        // We want to make the current beat block (based on the current beat number of the BeatTimingManager) to stand out

        if (UIVisible)
        {
            for (int i = 0; i < beatBlockImages.Length; i++)
            {
                // This checks if i represents the index of the currently firing BeatBlock
                if (i == (BeatTimingManager.btmInstance.GetBeatNumber() % beatBlockImages.Length))
                {
                    // We check if the currently active BeatBlock has increased to full size yet
                    // We only check the x value to save some processing power
                    if (Mathf.Abs( (initialBlockScale.x * blockScaleIncrease) - beatBlockImages[i].transform.localScale.x) > 0.025f)
                    {
                        beatBlockImages[i].transform.localScale = 
                            Vector3.Lerp(beatBlockImages[i].transform.localScale, initialBlockScale * blockScaleIncrease, 
                            blockScaleSpeed * Time.deltaTime);
                    }
                    else
                    {
                        if (!beatBlockParticles[i].isPlaying)
                        {
                            beatBlockParticles[i].Play();
                        }
                    }
                }
                else  // Here we make sure the blacks that aren't representative of the current beat return to their regular size
                {
                    if (Mathf.Abs(beatBlockImages[i].transform.localScale.x - initialBlockScale.x) > 0.025f)
                    {
                        beatBlockImages[i].transform.localScale =
                            Vector3.Lerp(beatBlockImages[i].transform.localScale, initialBlockScale,
                            blockScaleSpeed * Time.deltaTime);
                    }
                    else
                    {
                        beatBlockImages[i].transform.localScale = initialBlockScale;
                    }
                }
            }
        }
        else
        {
            // Here we'll just make all the UI blocks return to their normal size as the UI fades out
            for (int i = 0; i < beatBlockImages.Length; i++)
            {
                // Note - if the UI's alpha is 0, we don't need to waste time lerping - we can jump to 0 as the player will not see this jump
                if (beatBlockImages[i].transform.localScale.x - initialBlockScale.x > 0.05f && beatBlockImages[i].color.a > 0)
                {
                    beatBlockImages[i].transform.localScale =
                        Vector3.Lerp(beatBlockImages[i].transform.localScale, initialBlockScale,
                        blockScaleSpeed * Time.deltaTime);
                }
                else
                {
                    beatBlockImages[i].transform.localScale = initialBlockScale;
                }
            }
        }

        #endregion
    }
}
