using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach to player's head. Prevents them from singing when submerged in water, and resets their sing time.
/// </summary>
public class WaterSingCancel : MonoBehaviour
{
    // MAKE DYNAMIC!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    // Use inheretance?

    /// <summary>
    /// The current SingController being used by the player
    /// </summary>
    private SingController_GroundReset singController;


    /// <summary>
    /// The script controlling the animations on our player character.
    /// </summary>
    [SerializeField]
    private PlayerAnimManager animManager;

    private void Start()
    {
        singController = GetComponentInParent<SingController_GroundReset>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Water") || other.gameObject.CompareTag("DeepWater"))
        {
            singController.ResetCurrentSingTime();
            animManager.Submerged = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Water") || other.gameObject.CompareTag("DeepWater"))
        {
            animManager.Submerged = false;
        }
    }
}
