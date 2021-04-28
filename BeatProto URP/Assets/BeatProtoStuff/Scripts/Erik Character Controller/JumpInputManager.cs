using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpInputManager : MonoBehaviour
{
    /// <summary>
    /// If true, DetectInput will keep returning true when the player holds the button down - allowing for hold-responsive jumps.
    /// Otherwise, only the initial tap of the button will matter.
    /// </summary>
    [SerializeField]
    private bool holdResponsive = true;

    /// <summary>
    /// The mamximum amount of seconds the input manager will return true for on each individual instance of the jump button being held.
    /// </summary>
    [SerializeField]
    private float maxHoldTime = 1.2f;

    /// <summary>
    /// Used to monitor when a held button should return true and when it should stop doing so.
    /// </summary>
    private bool acceptingInput = true;

    /// <summary>
    /// Used to time each individual hold of the jump button.
    /// </summary>
    private float holdTimer = 0;

    // DO I need a min time?

    #region Accessors
    /// <summary>
    /// The mamximum amount of seconds the input manager will return true for on each individual instance of the jump button being held.
    /// </summary>
    public float MaxHoldTime
    {
        get {
            return maxHoldTime;
        }

        set {
            maxHoldTime = value;
        }
    }

    /// <summary>
    /// Used to time each individual hold of the jump button.
    /// </summary>
    public float HoldTimer
    {
        get {
            return holdTimer;
        }
    }

    #endregion

    /// <summary>
    /// Takes into account the player's interaction with the jump button and how long said interaction has been happening to decide whether 
    /// the jump should still be responding or not. This overload considers if the player is on the ground when they press jump or not.
    /// </summary>
    /// <param name="grounded">Whether the player is on the ground or not.</param>
    /// <returns>bool representing whether the character should respond to input or not.</returns>
    public bool DetectInput(bool grounded)
    {
        if (holdResponsive)
        {
            if (Input.GetButtonUp("Jump"))
            {
                acceptingInput = true;
            }

            if (acceptingInput)
            {
                // Player tried to start a jump from the ground.
                if (Input.GetButtonDown("Jump") && grounded)
                {
                    holdTimer = 0;
                    return true;
                }
                // Player is holding jump button
                else if (Input.GetButton("Jump"))
                {
                    holdTimer += Time.deltaTime;

                    if (holdTimer < maxHoldTime)
                    {
                        return true;
                    }
                    else
                    {
                        acceptingInput = false;
                        return false;
                    }
                }
            }

            return false;
        }
        else
        {
            if (Input.GetButtonDown("Jump"))
            {
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Takes into account the player's interaction with the jump button and how long said interaction has been happening to decide whether 
    /// the jump should still be responding or not. This overload considers if the player is against a wall when they press jump or not.
    /// </summary>
    /// <param name="grounded">Whether the player is on the ground or not.</param>
    /// <returns>bool representing whether the character should respond to input or not.</returns>
    public bool DetectInput(WallDirection wallDirection)
    {
        if (holdResponsive)
        {
            if (Input.GetButtonUp("Jump"))
            {
                acceptingInput = true;
            }

            if (acceptingInput)
            {
                // Player pressed jump and is currently against a wall
                if (Input.GetButtonDown("Jump") && wallDirection != WallDirection.None)
                {
                    holdTimer = 0;
                    return true;
                }
                // Player is holding jump
                else if (Input.GetButton("Jump"))
                {
                    holdTimer += Time.deltaTime;

                    if (holdTimer < maxHoldTime)
                    {
                        return true;
                    }
                    else
                    {
                        acceptingInput = false;
                        return false;
                    }
                }
            }

            return false;
        }
        else
        {
            if (Input.GetButtonDown("Jump"))
            {
                return true;
            }

            return false;
        }
    }


    public void StopDetectingHold()
    {
        acceptingInput = false;
    }
}
