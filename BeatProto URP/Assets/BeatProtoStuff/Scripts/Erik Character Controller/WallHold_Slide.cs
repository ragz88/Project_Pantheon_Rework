using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallHold_Slide : MonoBehaviour
{
    [Tooltip("The speed at which the player should slide down walls when pressing against them.")]
    /// <summary>
    /// The speed at which the player should slide down walls when pressing against them.
    /// </summary>
    [SerializeField]
    private float wallSlideVelocity = -1f;


    /// <summary>
    /// Set to the relevant direction if a player moves onto the wall. Player will keep sliding until they move away or jump.
    /// </summary>
    private WallDirection lockedOntoWall = WallDirection.None;
    
    
    /// <summary>
    /// The character controller for the player character that will be singing. This class should be a child of said character object.
    /// </summary>
    private PlayerCharacterController characterController;


    private void OnEnable()
    {
        characterController = GetComponentInParent<PlayerCharacterController>();
        characterController.checkWallHold += CheckWallHold;

        characterController.WallHoldFallVelocity = wallSlideVelocity;
    }

    private void OnDisable()
    {
        characterController.checkWallHold -= CheckWallHold;
    }

    bool CheckWallHold(WallDirection wallDirection, float xInput)
    {
        /*if (lockedOntoWall != WallDirection.None)
        {
            // First we check if the player is still near a wall in the correct direction
            if (lockedOntoWall != wallDirection)
            {
                lockedOntoWall = WallDirection.None;
                return false;
            }
            // We then check if the xInput of the player is in the opposite direction of the wall they're locked onto
            else if ((xInput > 0 && wallDirection == WallDirection.Left) || (xInput < 0 && wallDirection == WallDirection.Right))
            {
                lockedOntoWall = WallDirection.None;
                return false;
            }
            // We check if the player jumped off the wall
            else if (Input.GetButtonDown("Jump"))
            {
                lockedOntoWall = WallDirection.None;
                return false;
            }
            // And if none of that has happened, they must still be on the wall!
            else
            {
                return true;
            }
        }
        else
        {
            // We check if the xInput of the player is in the same direction as a nearby wall
            if ((xInput < 0 && wallDirection == WallDirection.Left) || (xInput > 0 && wallDirection == WallDirection.Right))
            {
                lockedOntoWall = wallDirection;
                return true;
            }
            else
            {
                return false;
            }
        }*/


        // We check if the xInput of the player is in the same direction as a nearby wall
        if ((xInput < 0 && wallDirection == WallDirection.Left) || (xInput > 0 && wallDirection == WallDirection.Right))
        {
            return true;
        }
        else
        {
            return false;
        }

    }
}
