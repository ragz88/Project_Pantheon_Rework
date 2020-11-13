﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //static instance of this script
    public static PlayerController pController;

    //Player
    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public bool playerFound = false;

    //Player components needed
    private Rigidbody2D playerRB;
    private SpriteRenderer playerSprite;

    //Movement bools
    private bool grounded;
    private bool floating;
    //sing and float stuff
    public float singTime;
    [HideInInspector]
    public float maxSingTime;  
    public float singRefillDelay; // delay before the sing bar starts refilling
    public float singRefillTime; //how long it takes the sing bar to refill, if it's empty
    private float SRefill;

    //Movement speed vars
    public float moveSpeed;
    public float jumpSpeed;

    //Gravity vars
    public float floatGravityFactor;
    public float fallGravityFactor;


    //Wall grab vars
    private bool grabbing;
    private bool canGrab = false;

    //Below is to let the wall jump work without having to hold a direction key. Without these variables, one must hold a direction and then press jump
    //With this, PC will automatically jump away from the wall. 
    private float hitDirection = 0;
    private bool allowHoriz = true;
    private float horizontalDisableTimer;
    public float defaultHorizontalTime;

    //Singing variables
    [HideInInspector]
    public bool singing = false;
    private PlayerSingControl singCont;


    //Animation Stuff (consider moving?) 
    private Animator playerAnimator;


    //tutorial specific stuff
    public bool isControlTutorial; //to disable some singing stuff during the controller tutorial
    public bool tapWallJump;  //should the player be able to simply tap in order to walljump? 

    private void Awake()
    {
        if (pController == null)
            pController = this;
        else
            return;

        GetPlayer();
    }

    void Start()
    {
        horizontalDisableTimer = defaultHorizontalTime;
        maxSingTime = singTime;
        SRefill = singRefillDelay;
    }


    void Update() //lots of stuff happening here. Not sure if should simplify or if it's fine. 
    {
        if (!grabbing) //the stuff below should only happen if the player isn't grabbing a wall. Includes most movement mechanics.
        {
            HorizontalMovement();

            //if (grounded)
            Jump();

            if (!grounded)
                PlayerFloat();
        }

        #region Sing Update
        //singing functionality, in which player floats while singing. 
        //Sing and float have limited use. Visualised by a bar in-game.
        Sing();

        if (singing)
        {
            singCont.ActiveSing();
            singRefillDelay = SRefill;
            if (!grounded) 
            {
                floating = true;
            }
        }
        if (!singing)
        {
            singCont.StopSing();
            singRefillDelay -= Time.deltaTime;
            floating = false;
        }
        //end of Sing functionality in Update
        #endregion

        #region wall grab in Update

        if (!tapWallJump) //to test the two different wall jump things
        {
            GrabWall(); //grab wall if possible

            if (grabbing) //stuff to do/check while the player is grabbing a wall
            {
                //raycasts are sent left and right to detect only walls
                //depending on which one returns a collision, the player direction is set
                //used to determine which direction the palyer will automatically jump while grabbing
                int mask = LayerMask.GetMask("Walls");
                RaycastHit2D hitRight = Physics2D.Raycast(player.transform.position + new Vector3(0.375f, 0, 0), Vector2.right, 0.5f, mask);
                RaycastHit2D hitLeft = Physics2D.Raycast(player.transform.position - new Vector3(0.375f, 0, 0), Vector2.left, 0.5f, mask);
                Debug.DrawRay(player.transform.position + new Vector3(0.375f, 0, 0), new Vector2(0.2f, 0), Color.red);
                Debug.DrawRay(player.transform.position - new Vector3(0.375f, 0, 0), new Vector2(-0.2f, 0), Color.red);

                if (hitRight)
                {
                    hitDirection = 0;
                }
                if (hitLeft)
                {
                    hitDirection = 1;
                }

                JumpFromWall(); //Jump while grabbing. Distinct from normal jump as it has X vel
            }
        }

       /* if (tapWallJump) 
        {
            WallJump();
        }*/

        //horizontal movement is disabled for a short time after a wall jump. Below code handles that. See "HorizontalMovement()" for explanation
        if (!allowHoriz)
        {
            horizontalDisableTimer -= Time.deltaTime;
        }

        if (horizontalDisableTimer <= 0)
        {
            allowHoriz = true;
            horizontalDisableTimer = defaultHorizontalTime;
        }

        #endregion


        HandleGravity(); //Handles gravity


        #region Sprite flip
        //Flips player sprite based on direction
        if (playerRB.velocity.x > 0)
        {
            FlipPlayerSprite(false);

            if (grounded)
                playerAnimator.SetTrigger("walking");
        }
        else if (playerRB.velocity.x < 0)
        {
            FlipPlayerSprite(true);
            if (grounded)
                playerAnimator.SetTrigger("walking");
        }

        #endregion

        
        // make it so that floating and singing can't be done endlessly
        if (singRefillDelay <= 0) 
        {
            if (singTime < maxSingTime)
                singTime += (Time.deltaTime*maxSingTime)/ singRefillTime;
        }

    }

    //Gets all the player GameObject information needed.
    private void GetPlayer()
    {
        if (player == null)
        {
            player = GameObject.Find("Player");

            if (player != null)
                playerFound = true;

            else if (player == null)
                Debug.LogError("Error: Player not found"); // return an error if player GameObject cannot be found. 
        }
        else
        {
            return;
        }

        if (playerFound)
        {
            playerRB = player.GetComponent<Rigidbody2D>(); // finds the players Rigidbody
            playerSprite = player.GetComponentInChildren<SpriteRenderer>(); //finds the players SpriteRenderer
            playerAnimator = player.GetComponent<Animator>();
            singCont = player.GetComponentInChildren<PlayerSingControl>();
        }
    }

    private void Sing() 
    {
        if (Input.GetButton("Sing"))
        {
            singTime -= Time.deltaTime;

            if (singTime > 0)
            {
                singing = true;
            }
            else
                singing = false;
        }
        else
            singing = false;
        
    }


    private void HorizontalMovement() // Controls how the player moves left and right. 
    {
        //remove the if statement if wall jump should use direction keys, rather than automatic.
        if (allowHoriz)
        {
            //Using GetAxisRaw because it allows the PC to stop immediately when letting go of horizontal direction key
            //Allows precision
            //However, without the above if statement, wall jump cannot be automatic.
            //This is because it constantly checks for horiz input. So even though the walljump sets an X vel, this reads the axis as 0 and sets X vel to 0 (if A/D is not held)
            float xMovement = Input.GetAxisRaw("Horizontal");
            float totalSpeed = xMovement * moveSpeed;

            playerRB.velocity = new Vector2(totalSpeed, playerRB.velocity.y);
        }

    }

    private void Jump() //BASE JUMP - simply sets a Y vel while maintaining X vel
    {
       
        if (Input.GetButtonDown("Jump") && grounded)
        {
            playerRB.velocity = new Vector2(playerRB.velocity.x, jumpSpeed);
            playerAnimator.SetTrigger("jumping");
            grounded = false;
        }
        else if (Input.GetButtonDown("Jump") && !grounded)
        {
            if (canGrab)
            {
                GravityToggle(2);
                horizontalDisableTimer = defaultHorizontalTime;

                int mask = LayerMask.GetMask("Walls");
                RaycastHit2D hitRight = Physics2D.Raycast(player.transform.position + new Vector3(0.375f, 0, 0), Vector2.right, 0.5f, mask);
                RaycastHit2D hitLeft = Physics2D.Raycast(player.transform.position - new Vector3(0.375f, 0, 0), Vector2.left, 0.5f, mask);
                Debug.DrawRay(player.transform.position + new Vector3(0.375f, 0, 0), new Vector2(0.2f, 0), Color.red);
                Debug.DrawRay(player.transform.position - new Vector3(0.375f, 0, 0), new Vector2(-0.2f, 0), Color.red);

                if (hitRight)
                {
                    hitDirection = 0;
                }
                if (hitLeft)
                {
                    hitDirection = 1;
                }

                if (hitDirection == 1)
                {
                    allowHoriz = false;
                    playerRB.velocity = new Vector2(moveSpeed, jumpSpeed);
                }
                else if (hitDirection == 0)
                {
                    allowHoriz = false;
                    playerRB.velocity = new Vector2(-moveSpeed, jumpSpeed);
                }
            }

        }

    }

    private void PlayerFloat() //Allows the player to float down. See "GravityHandler()" for exact functionality 
    {
        /*if (Input.GetButton("Sing") && floatTime > 0)
        {
            floating = true;
            floatRefillDelay = fRefill;
            floatTime -= Time.deltaTime;

            //for tutorial visualisation
            if (isControlTutorial)
                singCont.ActiveSing();
        }
        else
        { 
            floating = false;
            if (isControlTutorial)
                singCont.StopSing();
        }*/
    }

    private void GrabWall() //Allows the player to grab a wall, if they are in range.
    {
        if (Input.GetButton("HoldWall")) 
        {
            if (canGrab && !grounded) //if the player can grab (i.e. are in range of a wall)
            {
                allowHoriz = true;
                horizontalDisableTimer = defaultHorizontalTime;
                grabbing = true; //vv important bool - controls what movement is allowed while grabbing. See Update() for all functionality.
                playerRB.velocity = Vector2.zero; //Stop the player from moving
                playerRB.Sleep(); //disable playerRB so that the player does not fall. Wanted to use gravity, but there was constant overlap with other things.

                playerAnimator.SetTrigger("grabbing");
            }
        }
    }

    private void WallJump() //tap to wall jump
    {
        if (canGrab) 
        {
            if (Input.GetButtonDown("Jump") && !grounded) 
            {
                GravityToggle(2);
                horizontalDisableTimer = defaultHorizontalTime;

                int mask = LayerMask.GetMask("Walls");
                RaycastHit2D hitRight = Physics2D.Raycast(player.transform.position + new Vector3(0.375f, 0, 0), Vector2.right, 0.5f, mask);
                RaycastHit2D hitLeft = Physics2D.Raycast(player.transform.position - new Vector3(0.375f, 0, 0), Vector2.left, 0.5f, mask);
                Debug.DrawRay(player.transform.position + new Vector3(0.375f, 0, 0), new Vector2(0.2f, 0), Color.red);
                Debug.DrawRay(player.transform.position - new Vector3(0.375f, 0, 0), new Vector2(-0.2f, 0), Color.red);

                if (hitRight)
                {
                    hitDirection = 0;
                }
                if (hitLeft)
                {
                    hitDirection = 1;
                }

                if (hitDirection == 1)
                {
                    allowHoriz = false;
                    playerRB.velocity = new Vector2(moveSpeed, jumpSpeed);
                }
                else if (hitDirection == 0) 
                {
                    allowHoriz = false;
                    playerRB.velocity = new Vector2(-moveSpeed, jumpSpeed);
                }

            }
        }
    }


    private void JumpFromWall() //Allows the player to wall jump if they are currently grabbing a wall. 
    {
        if (Input.GetButtonDown("Jump") && hitDirection == 1) //hitdirection == 1 is the same as LEFT, i.e wall is on PCs left, so they jump right
        {
            allowHoriz = false; //stop horizontal movement input - see var declaration and HorizontalMovement () for more info.
            playerRB.WakeUp(); //re-activated playerRB because grabbing wall deactivates it
            playerRB.velocity = new Vector2(moveSpeed, jumpSpeed); //jump right and up
        }
        else if (Input.GetButtonDown("Jump") && hitDirection == 0) //hitdirection == 1 is the same as RIGHT, i.e wall is on PCs right, so they jump left
        {
            allowHoriz = false; //stop horizontal movement input - see var declaration and HorizontalMovement () for more info.
            playerRB.WakeUp(); //re-activated playerRB because grabbing wall deactivates it
            playerRB.velocity = new Vector2(-moveSpeed, jumpSpeed); //jump left and up
        }
    }

    private void GravityToggle(float gravScale) //function to set gravity to desired amount
    {
        playerRB.gravityScale = gravScale;
    }

    private void FlipPlayerSprite(bool shouldFlip) //flips player sprite function. Can prolly remove
    {
        playerSprite.flipX = shouldFlip;
    }
 
    private void HandleGravity()  //Function that defines how gravity should be set based on the state of the game.
    {
        if (!grounded && playerRB.velocity.y < 0 && !floating) //if the player is falling, gravity should be higher for impact
        {
            GravityToggle(fallGravityFactor);
            playerAnimator.SetTrigger("falling");
        }
        else if (!grounded && playerRB.velocity.y < 0 && floating) //if the player is floating, gravity should be very low
        {
            float playerY = Mathf.Clamp(playerRB.velocity.y, -0.5f, 20); //This, and next line slows the player Y down. Because if the player has been falling,
            playerRB.velocity = new Vector2(playerRB.velocity.x, playerY); // even though gravity gets set lower, they will maintain a high fall speed
            GravityToggle(floatGravityFactor);
            playerAnimator.SetTrigger("floating");
        }
        else if (grounded || grabbing) //gravity should be reset to normal when grounded or grabbing
            GravityToggle(2);
    }

    public void SetGrounded(bool ground) //Function that sets whether the player is grounded. See "PlayerGroundCheck" script
    {
        grounded = ground;
    }

    public void SetGrabPossible(bool grab) //Sets whether the player can grab a wall. See "PlayerWallCheck" script.
    {
        canGrab = grab;
    }

    public void SetGrabState(bool isGrabbing) //sets whether the player is currently grabbing a wall. See "PlayerWallCheck" script.
    {
        grabbing = isGrabbing;
    }

}

