using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RadialBeatCursor : MonoBehaviour
{
    [Tooltip("The beatUI this cursor will display information for.")]
    /// <summary>
    /// The beatUI this cursor will display information for.
    /// </summary>
    public BeatUI beatUI;


    [Tooltip("If we don't want the cursor to be exactly on the circular path of the UI, this can be used to offset it to either a larger or smaller radius.")]
    /// <summary>
    /// If we don't want the cursor to be exactly on the circular path of the UI, this can be used to offset it to either a larger or
    /// smaller radius.
    /// </summary>
    public float cursorRadialOffset = 0;


    [Tooltip("If true, the cursor will rotate around its z axis to always point to the center of the circular path it travels on.")]
    /// <summary>
    /// If true, the cursor will rotate around its z axis to always point to the center of the circular path it travels on.
    /// </summary>
    public bool rotateCursor = true;
    
    // Used to fade cursor in and out depending on the visibility settings of the UI
    SpriteRenderer cursorSprite;

    // Start is called before the first frame update
    void Start()
    {
        cursorSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // We want our cursor to highlight the currently firing beat when the beat blocks are visible
        if (beatUI.UIVisible && beatUI.UIReady)
        {
            #region Calculate Current Cursor Position

            // This is the progression (in seconds) within the current beat cycle, based on the seeker time from our timing manager, the 
            // BPM of the current song and the number of beats in a cycle for this specific beatUI module.
            double currentBeatProgressTime = BeatTimingManager.btmInstance.seekerTime % 
                                             (BeatTimingManager.btmInstance.beatLength * beatUI.beatBlockSprites.Length);

            // This is the percentage progression within the current beat cycle - calculated based on the total time of a single cycle
            double currentBeatProgressPercent = currentBeatProgressTime/ (BeatTimingManager.btmInstance.beatLength * beatUI.beatBlockSprites.Length);

            // We need to reverse the current time to the time remaining, as polar notation on a circular graph is designed
            // to progress anti-clockwise rather than clockwise. This will make our progression clockwise.
            float remainingPercentage = 1 - (float)currentBeatProgressPercent;

            // We convert that percentage into an angle, representing how far we've rotated in a circular path
            // We also need to add 90 degrees to the current angle, as polar notation starts on the positive x axis
            // (3 o,clock) rather than on the positive y axis (12 o'clock). Note, 0.5*PI is 90 degrees in radians and 2PI is 360 degrees.
            float currentAngle = (remainingPercentage * 2 * Mathf.PI) + (0.5f * Mathf.PI);

            // We convert that angle into x and y coordinates using some simple trig
            float xPosition = (beatUI.radius + cursorRadialOffset) * Mathf.Cos(currentAngle);
            float yPosition = (beatUI.radius + cursorRadialOffset) * Mathf.Sin(currentAngle);

            // And finally, asign our new cursor position
            transform.position = beatUI.transform.position + new Vector3(xPosition, yPosition, 0);

            // Update the cursor's rotation
            if (rotateCursor)
            {
                // These rotation values are in degrees, so we do a similar conversion as before, but using 360 rather than 2 * PI
                transform.eulerAngles = 
                    new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, (remainingPercentage * 360));
            }

            #endregion


            if (cursorSprite.color.a < 1)
            {
                cursorSprite.color = cursorSprite.color + new Color(0, 0, 0, 5 * Time.deltaTime);
            }
        }
        else
        {
            if (cursorSprite.color.a > 0)
            {
                cursorSprite.color = cursorSprite.color - new Color(0, 0, 0, 5 * Time.deltaTime);
            }
        }
    }
}
