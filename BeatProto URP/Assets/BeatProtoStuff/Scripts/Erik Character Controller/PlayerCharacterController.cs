using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerCharacterController : MonoBehaviour
{

    /// <summary>
    /// The Rigidbody component of the player character.
    /// </summary>
    private Rigidbody playerBody;

    /// <summary>
    /// Describes if the character is currently securely on the ground.
    /// </summary>
    private bool grounded = true;

    /// <summary>
    /// Set to true when the player tries to sing and still has some SingTime left.
    /// </summary>
    private bool isSinging = false;

    /// <summary>
    /// Set to true if player grabs onto wall while falling.
    /// </summary>
    private bool isHoldingWall = false;

    /// <summary>
    /// Describes the speed the player will fall at when singing.
    /// </summary>
    private float singFallVelocity = 0;

    /// <summary>
    /// Describes the speed the player will slip down walls at.
    /// </summary>
    private float wallHoldFallVelocity = 0;

    /// <summary>
    /// Dictates whether or not the currently equipped jump is responsive to the amount of time the Jump button is held down.
    /// If true, holding down jump makes the player character jump higher.
    /// </summary>
    private bool holdJump = true;

    
    /// <summary>
    /// Description of the current walls surrounding the player for wall jumping purposes. Either left, right or none.
    /// </summary>
    private WallDirection currentWalls = WallDirection.None;

    /// <summary>
    /// Describes the type of terrain the player is currently standing on, if any.
    /// </summary>
    private TerrainType currentTerrain = TerrainType.Standard;

    /// <summary>
    /// Describes the type of medium/fluid the player is currently moving through.
    /// </summary>
    private MediumType currentMedium = MediumType.Air;

    #region Delegate Declarations
    // These will allow us to dynamically assign different movement functions at the start of the game.
    // This allows for more effective testing and better modular design.

    public delegate Vector2 CalculateXVelocity(float xInput, TerrainType currentTerrain, MediumType currentMedium);
    public delegate Vector2 CalculateJumpVelocity(bool grounded);
    public delegate bool CheckWallHold(WallDirection wallDirection, float xInput);
    public delegate Vector2 CalculateWallJumpVelocity(WallDirection wallDirection);
    public delegate WallDirection DetectWalls();
    public delegate TerrainType DetectTerrain();
    public delegate bool HandleSing();


    /// <summary>
    /// Takes into account the player's input, and potentially terrain type, and returns a vector describing the appropriate x movment for the player.
    /// </summary>
    public CalculateXVelocity calculateXVelocity;

    /// <summary>
    /// Takes into account the player's input and type of jump assigned to them, and returns a vector representing the appropriate y movement for the player.
    /// </summary>
    public CalculateJumpVelocity calculateJumpVelocity;

    /// <summary>
    /// Looks to left an right of the player. Returns a WallDirection value describing the nearby walls.
    /// </summary>
    public DetectWalls detectWalls;

    /// <summary>
    /// Looks at what the player is standing on and returns a TerrainType describing the floor. If player is not grounded, it will return TerrainType.None.
    /// </summary>
    public DetectTerrain detectTerrain;

    /// <summary>
    /// Creates the Sing area of effect around the player, and returns true, if the player still has SingTime and is attempting to sing.
    /// </summary>
    public HandleSing handleSing;

    /// <summary>
    /// Returns true if the player is moving into a wall.
    /// </summary>
    public CheckWallHold checkWallHold;

    /// <summary>
    /// Returns a vector2 representing the correct velocity for the player after they've jumped when airborn next to a wall.
    /// </summary>
    public CalculateWallJumpVelocity calculateWallJumpVelocity;

    #endregion

    #region Accessors
    /// <summary>
    /// Describes if the character is currently securely on the ground.
    /// </summary>
    public bool Grounded
    {
        get
        {
            return grounded;
        }

        set
        {
            grounded = value;
        }
    }

    /// <summary>
    /// Describes the speed the player will fall at when singing.
    /// </summary>
    public float SingFallVelocity
    {
        get
        {
            return singFallVelocity;
        }

        set
        {
            singFallVelocity = value;
        }
    }

    /// <summary>
    /// Describes the speed the player will fall at when singing.
    /// </summary>
    public float WallHoldFallVelocity
    {
        get
        {
            return wallHoldFallVelocity;
        }

        set
        {
            wallHoldFallVelocity = value;
        }
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        playerBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get Movement Input
        float xMovementInput = Input.GetAxis("Horizontal");

        AnalyseEnvironment();

        // Handle Jump
        bool jumpInput = GetJumpInput();
        Vector2 jumpVelocity = Vector2.zero;

        // Implies player is attempting to jump
        if (jumpInput)
        {
            jumpVelocity = HandleJump();
        }

        // Calculates Horizontal Movement (Potentially Accounting For Friction)
        Vector2 horizontalVelocity = Vector2.zero;
        if (calculateXVelocity != null)
        {
            horizontalVelocity = calculateXVelocity(xMovementInput, currentTerrain, currentMedium);
        }
        else
        {
            Debug.LogError("calculateXVelocity Delegate has no functionality assigned to it.");
        }


        //Checks if player is both trying to and able to sing.
        if (handleSing != null)
        { 
            isSinging = handleSing();
        }
        else
        {
            Debug.LogError("handleSing Delegate has no functionality assigned to it.");
        }


        // Checks if player isn't grounded and moves into a wall
        if (checkWallHold != null)
        { 
            isHoldingWall = checkWallHold(currentWalls, xMovementInput);
        }
        else
        {
            Debug.LogWarning("checkWallHold Delegate has no functionality assigned to it.");
        }

        // This float describes the falling speed of the player - this speed can be changed by grabbing onto a wall
        // or by singing
        float fallVelocity = CalculateFallVelocity();


        // Finally, we add all the individual parts together to get a vector for movement.
        Vector2 playerMovement = CalculateFinalVelocity(horizontalVelocity, jumpVelocity, fallVelocity);

        playerBody.velocity = playerMovement;
    }

    /// <summary>
    /// Checks for various actions that could change the player's fall speed, such as wall grabbing and singing.
    /// Then returns a float that represents the correct fall velocity for the player.
    /// Always prioritises the slowest fall type possible, for the benefit of the player.
    /// </summary>
    /// <returns>A float that represents the correct fall velocity for the player, based on Singing and Wall-grabbing.</returns>
    private float CalculateFallVelocity()
    {
        float fallVelocity = playerBody.velocity.y;

        if (isSinging && singFallVelocity > fallVelocity)
        {
            fallVelocity = singFallVelocity;
        }

        if (isHoldingWall && wallHoldFallVelocity > fallVelocity)
        {
            fallVelocity = wallHoldFallVelocity;
        }


        return fallVelocity;
    }


    /// <summary>
    /// Takes into account all the previously calculated velocities and constructs a Vector which represents the correct combination of them all.
    /// </summary>
    /// <param name="horizontalVelocity">Velocity due to x movement input.</param>
    /// <param name="jumpVelocity">Velocity due to jump actions.</param>
    /// <param name="fallVelocity">Velocity player should fall at, if they are falling, based on various factors that augment fall speed.</param>
    /// <returns></returns>
    private Vector2 CalculateFinalVelocity(Vector2 horizontalVelocity, Vector2 jumpVelocity, float fallVelocity)
    {
        Vector2 finalVelocity = Vector2.zero;

        // This implies a jump was done off of a wall, and its x velocity must be prioritised over the standard movement velocity
        // for this frame.
        if (jumpVelocity.x != 0)
        {
            finalVelocity = jumpVelocity;
        }
        // Otherwise, we naturally combine the x and y velocities resulting from movement inputs
        else
        {
            finalVelocity = horizontalVelocity + jumpVelocity;
        }

        // Finally, we check if the player is currently falling (has a negative y velocity) and thus account for any fall 
        // augmentations that are currently active.
        if (playerBody.velocity.y < 0 && jumpVelocity.y < 0)
        {
            playerBody.velocity = new Vector2 (playerBody.velocity.x, fallVelocity);
        }


        return finalVelocity;
    }


    /// <summary>
    /// Used to check the area directly around the player for walls, ground, and (if ground is found) the type of ground.
    /// Stores this information in the relevant variables.
    /// </summary>
    private void AnalyseEnvironment()
    {
        if (detectTerrain != null)
        {
            currentTerrain = detectTerrain();
        }
        else
        {
            Debug.LogError("detectTerrain Delegate has no functionality assigned to it.");
        }

        if (detectWalls != null)
        {
            currentWalls = detectWalls();
        }
        else
        {
            Debug.LogWarning("detectWalls Delegate has no functionality assigned to it.");
        }

        // Implies the player is standing on something
        if (currentTerrain != TerrainType.None)
        {
            grounded = true;
        }
    }

    /// <summary>
    /// Based on characters position relative to the ground and nearby walls, constructs a Vector2 
    /// that represents the movement resulting from a jump.
    /// </summary>
    /// <returns>A Vector2 that represents the movement resulting from a jump.</returns>
    private Vector2 HandleJump()
    {
        // Implies this is the first frame in which a jump has happened from the wall
        if (!grounded && (currentWalls != WallDirection.None))
        {
            if (calculateWallJumpVelocity != null)
            {
                return calculateWallJumpVelocity(currentWalls);
            }
            else
            {
                Debug.LogWarning("calculateWallJumpVelocity Delegate has no functionality assigned to it.");
            }

            return Vector2.zero;
        }
        else
        {
            if (calculateJumpVelocity != null)
            {
                return calculateJumpVelocity(grounded);
            }
            else
            {
                Debug.LogError("calculateJumpVelocity Delegate has no functionality assigned to it.");
            }

            return Vector2.zero;
        }
    }


    /// <summary>
    /// Examines holdJump and uses either GetButtonDown or GetButton when detecting Jump input in response.
    /// </summary>
    /// <returns></returns>
    private bool GetJumpInput()
    {
        if (holdJump)
        {
            if (Input.GetButton("Jump"))
            {
                return true;
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
}
