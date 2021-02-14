using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleRotate : BeatToggle
{
    private bool rotating = false;

    public float rotationAmount;

    public enum RotationDirection { clockWise, antiClockwise };

    public RotationDirection rotationDirection;

    public override void BeatEvent()
    {
        if (rotationDirection == RotationDirection.clockWise) 
        {
            toggleObject.transform.Rotate(0, 0, rotationAmount);
        }
        else if (rotationDirection == RotationDirection.antiClockwise)
        {
            toggleObject.transform.Rotate(0, 0, -rotationAmount);
        }
    }

    

}
