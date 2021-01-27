using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// We'll figure out how we trigger these actions once we start level design - but this is a script that allows us to activate them for testing.
/// </summary>
public class TriggerSolutionCheck : MonoBehaviour
{
    public SolutionDetector solDetector;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            solDetector.OnLevelComplete();
        }
    }
}
