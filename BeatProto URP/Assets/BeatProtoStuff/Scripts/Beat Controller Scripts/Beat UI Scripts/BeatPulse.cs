using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class BeatPulse : MonoBehaviour
{
    /// <summary>
    /// The Beat Input UI controller for the set of blocks we want to pulse to the beat
    /// </summary>
    public BeatUI beatUI;

    /// <summary>
    /// How large the beat block will become in relation to it's initial size. 1 will result in no change.
    /// </summary>
    public float sizeIncreaseMultiplier;

    /// <summary>
    /// Speed at which the beat block will increase to a maximum size.
    /// </summary>
    public float increasingSpeed = 5f;


    /// <summary>
    /// Speed at which the beat block will decrease back to it's initial size after a pulse.
    /// </summary>
    public float decreasingSpeed = 1f;

    /// <summary>
    /// Will be used to detect the moment the current beat index changes.
    /// </summary>
    private int currentPulseIndex = 0;

    private bool increasingPulse = false;

    private Vector3 initialScale;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (beatUI.UIReady)
        {
            // We can't do this in Start, as the instantiation process may not be finished yet
            if (initialScale == Vector3.zero)
            {
                initialScale = beatUI.beatBlockSprites[0].transform.localScale;
            }

            // This if statement will be true the moment that the current beat number changes
            if (BeatTimingManager.btmInstance.GetBeatNumber() % beatUI.beatBlockSprites.Length != currentPulseIndex)
            {
                // At this moment, we want to tell the next beat block that it should increase in size
                increasingPulse = true;

                // We also want to update our currentPulseIndex, as we've already dealt with the moment of change
                currentPulseIndex = BeatTimingManager.btmInstance.GetBeatNumber() % beatUI.beatBlockSprites.Length;
            }

            // This handles increasing a beat block's size to it's pulse maximum each time the beat number changes
            if (increasingPulse)
            {
                beatUI.beatBlockSprites[currentPulseIndex].transform.localScale =
                    Vector3.MoveTowards(beatUI.beatBlockSprites[currentPulseIndex].transform.localScale, initialScale * sizeIncreaseMultiplier,
                    increasingSpeed * Time.deltaTime);

                // Here we check if the current scale has approximately reached the final larger scale value.
                // We don't use Vector3.Distance to improve performance by avoiding Square Root functions
                if (((initialScale.x * sizeIncreaseMultiplier) - beatUI.beatBlockSprites[currentPulseIndex].transform.localScale.x) < 0.1f)
                {
                    // If we've reached the larger scale, it's time to start decreasing in size again. We start by resetting our bool
                    increasingPulse = false;
                }
            }

            // Next, we want to decrease the size of all our other beat boxes, which may still be scaled up from a previous beat.
            for (int i = 0; i < beatUI.beatBlockSprites.Length; i++)
            {
                // We don't want to reduce the size of a beat block that is currently pulsing.
                // If either of these conditions is met, then we cannot be dealing with a block that's still growing
                if (i != currentPulseIndex || !increasingPulse)
                {
                    // Next, we don't want to waste time lerping anything that's already the correct size, so let's check if the block
                    // actually needs lerping.
                    if ((beatUI.beatBlockSprites[i].transform.localScale.x - initialScale.x) > 0.01f ||
                        (beatUI.beatBlockSprites[i].transform.localScale.y - initialScale.y) > 0.01f)
                    {
                        // Lerp back to original size
                        beatUI.beatBlockSprites[i].transform.localScale =
                            Vector3.Lerp(beatUI.beatBlockSprites[i].transform.localScale, initialScale,
                            decreasingSpeed * Time.deltaTime);
                    }
                    else
                    {
                        beatUI.beatBlockSprites[i].transform.localScale = initialScale;
                    }
                }
            }
        }
    }
}
