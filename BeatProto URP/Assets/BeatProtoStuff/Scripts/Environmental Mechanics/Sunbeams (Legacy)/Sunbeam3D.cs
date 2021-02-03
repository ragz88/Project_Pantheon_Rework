using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sunbeam3D : MonoBehaviour
{
    public SunbeamColliderGroup[] colliderGroups;

    // FOR TESTING ONLY
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            for (int i = 0; i < colliderGroups.Length; i++)
            {
                colliderGroups[i].CheckCurrentCasterStates();
            }
        }
    }
}
