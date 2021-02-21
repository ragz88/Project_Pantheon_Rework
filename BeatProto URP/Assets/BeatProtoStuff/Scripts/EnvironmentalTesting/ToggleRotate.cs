using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleRotate : BeatToggle
{
    private bool rotating = false;

    public Vector3 rotationAmount;

    private Vector3 targetRotation;

    private Vector3 outputRotation;

    private float time;

    private float beatTime;

    public enum RotationDirection { clockWise, antiClockwise };

    public RotationDirection rotationDirection;

    public override void BeatEvent()
    {

        if (rotationDirection == RotationDirection.clockWise)
        {
            targetRotation -= rotationAmount;
        }
        else if (rotationDirection == RotationDirection.antiClockwise)
        {
            targetRotation += rotationAmount;
        }

        time = 0;
        rotating = true;
    }

    private void Update()
    {
        beatTime = 60 / BeatTimingManager.btmInstance.currentSong.BPM;

        if (rotating)
        {
            time += Time.deltaTime / beatTime/180;
            outputRotation = new Vector3(0, 0, Mathf.LerpAngle(outputRotation.z, targetRotation.z, time));

            transform.eulerAngles = outputRotation;
            if (transform.eulerAngles == targetRotation) 
            {
                rotating = false;
                time = 0;
            }
        }

    }



}
