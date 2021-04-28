using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(JumpInputManager))]
public class WallJump_ZeroG_CumMomentum : MonoBehaviour
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

    [Tooltip("The horizontalvelocity at which the character jumps off the wall.")]
    /// <summary>
    /// The horizontalvelocity at which the character jumps off the wall.
    /// </summary>
    [SerializeField]
    private float jumpXVelocity = 6;

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
        characterController.calculateWallJumpVelocity += CalculateWallJumpVelocity;

        playerBody = characterController.gameObject.GetComponent<Rigidbody>();

        jumpInputManager = GetComponent<JumpInputManager>();

        Physics.gravity = new Vector3(0, standardGravityAugment, 0);
    }

    private void OnDisable()
    {
        // We want to remove the jump functionality of this class to the character controller.
        characterController.calculateWallJumpVelocity -= CalculateWallJumpVelocity;

        Physics.gravity = new Vector3(0, defaultUnityGravity, 0);
    }

    Vector2 CalculateWallJumpVelocity(WallDirection wallDirection)
    {
        if (jumpInputManager.DetectInput(wallDirection))
        {
            Physics.gravity = new Vector3(0, zeroGravityAugment, 0);

            if (jumpInputManager.HoldTimer == 0 && wallDirection != WallDirection.None)
            {
                // Converts information about nearby walls into an integer multiplier for our x velocity.
                int xDir = ConvertXDirection(wallDirection);
                
                Vector2 newVelocity = new Vector3(xDir * jumpXVelocity, jumpCumulativeVelocity + playerBody.velocity.y);

                if (newVelocity.y < minJumpVelocity)
                {
                    newVelocity = new Vector3(xDir * jumpXVelocity, minJumpVelocity);
                }
                else if (newVelocity.y > maxJumpVelocity)
                {
                    newVelocity = new Vector3(xDir * jumpXVelocity, maxJumpVelocity);
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

    /// <summary>
    /// Considers the nearby walls and either returns 1 or -1. This int describes the direction the character needs to jump in.
    /// </summary>
    /// <param name="wallDirection">The position of the wall the character is jumping off of, relative to the player.</param>
    /// <returns>Either 1 or -1, depending on the direction the player needs to jump in response.</returns>
    int ConvertXDirection(WallDirection wallDirection)
    {
        if (wallDirection == WallDirection.Left)
        {
            // Character must jump right off of a wall on their left.
            return 1;
        }
        else if (wallDirection == WallDirection.Right)
        {
            // Character must jump left off of a wall on their right.
            return -1;
        }

        return 0;
    }
}
