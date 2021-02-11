using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach to player's head. Prevents them from singing when submerged in water, and resets their sing time.
/// </summary>
public class WaterSingCancel : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Water") || other.gameObject.CompareTag("DeepWater"))
        {
            PlayerController3D.pController3D.singTime = 0;
        }
    }
}
