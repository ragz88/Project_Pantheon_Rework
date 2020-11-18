﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkToSurvey : MonoBehaviour
{   
    /// <summary>
    /// Loads a URL, taking the player to the google forms survey created for this iteration.
    /// </summary>
    public void GoToSurvey()
    {
        Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSevfxfSBURNUjOVWDo-upzsIkyNxLnekSvlkVOJRyvko_Er7Q/viewform?usp=sf_link");
    }
}
