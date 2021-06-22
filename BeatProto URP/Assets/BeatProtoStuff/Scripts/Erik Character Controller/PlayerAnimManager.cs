using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerAnimManager : MonoBehaviour
{
    [SerializeField]
    Animator playerAnim;

    PlayerCharacterController charController;

    [SerializeField]
    private float zeroVelocityAccuracy = 0.05f;

    private bool grounded = false;
    private bool singing = false;
    private bool holdingWall = false;

    // True when any form of horizontal input is present
    private bool xInputPresent = false;

    // True when the player's horizontal velocity approaches zero.
    private bool xVelocityZero = true;

    // True if player's y Velocity is larger than 0
    private bool yVelocityPositive = false;

    /// <summary>
    /// True if the player's input is in the same direction as the existing motion. If they are opposed to one another, will be false.
    /// </summary>
    private bool xVelocityAndInputAlign = false;


    /// <summary>
    /// True if the player presses space when airborne next to a wall. Used exclusively to transition between airborne states and the wall jump animation.
    /// </summary>
    private bool wallJumping = false;

    /// <summary>
    /// True when character in deep water.
    /// </summary>
    private bool swimming = false;

    /// <summary>
    /// True when the character's head is underwater - thus they cannot sing.
    /// </summary>
    private bool submerged = false;

    // This will be used to flip the image horizontally - as flip sprite is not an option.
    private float initialXScale;

    private WallDirection currentWall;


    #region Accessors

    /// <summary>
    /// True when character in deep water.
    /// </summary>
    public bool Swimming
    {
        set
        {
            swimming = value;
        }
    }

    /// <summary>
    /// True when the character's head is underwater - thus they cannot sing.
    /// </summary>
    public bool Submerged
    {
        set
        {
            submerged = value;
        }
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        charController = GameObject.FindObjectOfType<PlayerCharacterController>();
        initialXScale = transform.localScale.x;
    }


    // Update is called once per frame
    void Update()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        float xVelocity = charController.FinalVelocity.x;
        float yVelocity = charController.FinalVelocity.y;
        singing = charController.Singing;

        holdingWall = charController.IsHoldingWall;
        currentWall = charController.CurrentWalls;


        if (xInput != 0)
        {
            xInputPresent = true;
        }
        else
        {
            xInputPresent = false;
        }


        if (xVelocity < -zeroVelocityAccuracy || xVelocity > zeroVelocityAccuracy)
        {
            xVelocityZero = false;
        }
        else
        {
            xVelocityZero = true;
        }
        
        
        if (Mathf.Sign(xInput) != Mathf.Sign(xVelocity))
        {
            xVelocityAndInputAlign = false;
        }
        else
        {
            xVelocityAndInputAlign = true;
        }


        if (yVelocity > 0)
        {
            yVelocityPositive = true;
        }
        else
        {
            yVelocityPositive = false;
        }


        if (currentWall != WallDirection.None && Input.GetButtonDown("Jump") && !grounded)
        {
            wallJumping = true;
        }
        else
        {
            wallJumping = false;
        }


        playerAnim.SetBool("xInputPresent", xInputPresent);
        playerAnim.SetBool("xVelocityZero", xVelocityZero);
        playerAnim.SetBool("xVelocityAndInputAlign", xVelocityAndInputAlign);
        playerAnim.SetBool("yVelocityPositive", yVelocityPositive);
        playerAnim.SetBool("Grounded", charController.Grounded);
        playerAnim.SetBool("HoldingWall", holdingWall);
        playerAnim.SetBool("WallJumping", wallJumping);
        playerAnim.SetBool("Swimming", swimming);
        playerAnim.SetBool("Submerged", submerged);
        playerAnim.SetBool("Singing", singing);


        // Flip image based on horizontal movement
        if (holdingWall)
        {
            if (currentWall == WallDirection.Left)
            {
                transform.localScale = new Vector3(initialXScale, transform.localScale.y, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(-initialXScale, transform.localScale.y, transform.localScale.z);
            }
        }
        else if (xInputPresent)
        {
            if (xInput < 0)
            {
                transform.localScale = new Vector3(-initialXScale, transform.localScale.y, transform.localScale.z);
            }
            else if (xInput > 0)
            {
                transform.localScale = new Vector3(initialXScale, transform.localScale.y, transform.localScale.z);
            }
        }
        else
        {
            if (xVelocity < 0)
            {
                transform.localScale = new Vector3(-initialXScale, transform.localScale.y, transform.localScale.z);
            }
            else if (xVelocity > 0)
            {
                transform.localScale = new Vector3(initialXScale, transform.localScale.y, transform.localScale.z);
            }
        }
    }


    void FlipImage()
    {

    }
}
