using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(JumpInputManager))]
public class Jump_ZeroG_CumMomentum : MonoBehaviour
{
    [Tooltip("The m/s added to Y velocity on each successive jump.")]
    /// <summary>
    /// The m/s added to Y velocity on each successive jump.
    /// </summary>
    [SerializeField]
    private float jumpCumulativeVelocity = 2;

    [Tooltip("The minimum Y velocity of the player right after a successful jump.")]
    /// <summary>
    /// The minimum Y velocity of the player right after a successful jump.
    /// </summary>
    [SerializeField]
    private float minJumpVelocity = 2;

    [Tooltip("The maximum Y velocity the player can reach by chaining jumps.")]
    /// <summary>
    /// The maximum Y velocity the player can reach by chaining jumps.
    /// </summary>
    [SerializeField]
    private float maxJumpVelocity = 30;

    [Tooltip("The value gravity should change to while successfully holding jump down.")]
    /// <summary>
    /// The value gravity should change to while successfully holding jump down.
    /// </summary>
    [SerializeField]
    private float zeroGravityAugment = 0;

    [Tooltip("The value gravity should return to when not jumping.")]
    /// <summary>
    /// The value gravity should return to when not jumping.
    /// </summary>
    [SerializeField]
    private float standardGravityAugment = -9.8f;

    /// <summary>
    /// The character controller for the player character. This class should be a child of said character object.
    /// </summary>
    private PlayerCharacterController characterController;

    /// <summary>
    /// The rigidbody of our Player Character.
    /// </summary>
    private Rigidbody playerBody;

    /// <summary>
    /// Checks the player's interaction with the jump button and decides if the jump function should be responding or not.
    /// </summary>
    private JumpInputManager jumpInputManager;

    /// <summary>
    /// The default gavity in Unity.
    /// </summary>
    private const float defaultUnityGravity = -9.8f;

    private void OnEnable()
    {
        // We want to add the jump functionality of this class to the character controller.
        characterController = GetComponentInParent<PlayerCharacterController>();
        characterController.calculateJumpVelocity += CalculateJumpVelocity;

        playerBody = characterController.gameObject.GetComponent<Rigidbody>();

        jumpInputManager = GetComponent<JumpInputManager>();

        Physics.gravity = new Vector3(0, standardGravityAugment, 0);
    }

    private void OnDisable()
    {
        // We want to remove the jump functionality of this class to the character controller.
        characterController.calculateJumpVelocity -= CalculateJumpVelocity;

        Physics.gravity = new Vector3(0, defaultUnityGravity, 0);
    }

    Vector2 CalculateJumpVelocity(bool grounded)
    {
        if (jumpInputManager.DetectInput(grounded))
        {
            Physics.gravity = new Vector3(0, zeroGravityAugment, 0);

            if (jumpInputManager.HoldTimer == 0 && grounded)
            {
                Vector2 newVelocity = new Vector3(0, jumpCumulativeVelocity + playerBody.velocity.y);

                if (newVelocity.y < minJumpVelocity)
                {
                    newVelocity = new Vector3(0, minJumpVelocity);
                }
                else if (newVelocity.y > maxJumpVelocity)
                {
                    newVelocity = new Vector3(0, maxJumpVelocity);
                }

                return newVelocity;
            }
        }
        else
        {
            Physics.gravity = new Vector3(0, standardGravityAugment, 0);
        }

        return new Vector2(0, playerBody.velocity.y);
    }

}
