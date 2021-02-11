using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Should be added to water sources that will take away the player's ability to sing (Like waterfalls).
/// </summary>
public class WaterSingCancel : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController3D.pController3D.singTime = 0;
        }
    }
}
