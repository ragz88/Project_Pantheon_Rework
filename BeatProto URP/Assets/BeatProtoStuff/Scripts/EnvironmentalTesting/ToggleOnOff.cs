using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleOnOff : BeatToggle
{

    public override void BeatEvent()
    {
        toggleObject.SetActive(!toggleObject.activeSelf);
    }

}
