using UnityEngine;


// MAKE SURE PLAYER RB IS SET TO INTERPOLATE!!!!!!

public class PlayerController3D : MonoBehaviour
{
    //static instance of this script
    public static PlayerController3D pController3D;

    //Player
    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public bool playerFound = false;

    //Player components needed
    private Rigidbody playerRB;
    private SpriteRenderer playerSprite;

    //Movement bools
    private bool grounded;
    private bool floating;
    private bool swimming = false;
    private bool useAcceleration = false;  // Used on some terrains - like ice.

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
    private float moveSpeedModifier = 1;  // This represents a percentage multiplier for the initial move speed - used to augment the speed on different terrains.
    private float moveAcceleration;       // Used for different terrains. How quickly the player accelerates to full speed.
    private float moveDecceleration;      // Used for different terrains. How quickly the player deccelerates to a standstill.

    [HideInInspector]
    public bool againstWallLeft = false;
    [HideInInspector]
    public bool againstWallRight = false;
    

    //Gravity vars
    public float floatGravityFactor;
    public float fallGravityFactor;
    public float defaultGravity;


    //Wall grab vars
    public LayerMask wallJumpMask;
    private bool grabbing;
    private bool canGrab = false;

    //Below is to let the wall jump work without having to hold a direction key. Without these variables, one must hold a direction and then press jump
    //With this, PC will automatically jump away from the wall. 
    private float hitDirection = 0;
    private bool allowHoriz = true;
    private float horizontalDisableTimer;
    public float defaultHorizontalTime;
    private Transform grabPositionandParent;
    public bool movingUpWall;


    //Singing variables
    [HideInInspector]
    public bool singing = false;
    private PlayerSingControl singCont;


    //Animation Stuff (consider moving?) 
    private Animator playerAnimator;


    //tutorial specific stuff
    public bool isControlTutorial; //to disable some singing stuff during the controller tutorial **WILL BE REMOVED**
    public bool tapWallJump;  //should the player be able to simply tap in order to walljump? 

    public bool wallJumpTutorial;


    private void Awake()
    {
        if (pController3D == null)
            pController3D = this;
        else
            return;

        GetPlayer();
    }

    void Start()
    {
        horizontalDisableTimer = defaultHorizontalTime;
        maxSingTime = singTime;
        SRefill = singRefillDelay;
        SetGravity(defaultGravity);
    }


    void Update() //lots of stuff happening here. Not sure if should simplify or if it's fine. 
    {

        //CheckForWalls();

        if (!grabbing) //the stuff below should only happen if the player isn't grabbing a wall. Includes most movement mechanics.
        {
            HorizontalMovement();

            //if (grounded)

            if (Input.GetButtonDown("Jump") && grounded)
            {
                Jump(jumpSpeed);
            }

            if (!grounded)
                PlayerFloat();

            player.transform.parent = null;
            playerRB.isKinematic = false;
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
            if (Input.GetButton("HoldWall"))
            {
                GrabWall(); //grab wall if possible
            }

            if (grabbing) //stuff to do/check while the player is grabbing a wall
            {
                //player.transform.position = grabPosition;
                JumpFromWall(); //Jump while grabbing. Distinct from normal jump as it has X vel

                //functionality to move up a wall that is grabbed
                //See PlayerWallCheck for the little jump that happens if leaving the wall.
               /* if (Input.GetKey(KeyCode.W))
                {
                    movingUpWall = true;
                    player.transform.Translate(0, 0.005f, 0);
                }
                else 
                {
                    movingUpWall = false;
                }*/
            }

            if (Input.GetKey(KeyCode.S))
            {
                StopGrabbing();
            }
        }

        if (tapWallJump)
        {
            if (Input.GetButtonDown("Jump") && !grounded)
            {
                WallJump();
            }
        }

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
                singTime += (Time.deltaTime * maxSingTime) / singRefillTime;
        }

        //For the wall jump tutorial
        if (wallJumpTutorial)
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                tapWallJump = !tapWallJump;
            }
        }
    }

    //Gets all the player GameObject information needed.
    private void GetPlayer()
    {
        if (player == null)
        {
            player = GameObject.Find("Player3D");

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
            playerRB = player.GetComponent<Rigidbody>(); // finds the players Rigidbody
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
        if (allowHoriz) //needed for wall jump to work nicely
        {
            //Using GetAxisRaw because it allows the PC to stop immediately when letting go of horizontal direction key (while on the ground)
            //Allows precision
            // the ground checks ensure that the player keeps moving with horizontal momentum in the air, but will stop when on the ground.
            if (!grounded)
            {
                if (Input.GetAxisRaw("Horizontal") != 0)
                {
                    if (!againstWallLeft && !againstWallRight)
                    {
                        float xMovement = Input.GetAxisRaw("Horizontal");
                        float totalSpeed = xMovement * moveSpeed * moveSpeedModifier;

                        playerRB.velocity = new Vector2(totalSpeed, playerRB.velocity.y);
                    }
                    else if (againstWallLeft) 
                    {
                        if (Input.GetAxisRaw("Horizontal") < 0)
                        {
                            playerRB.velocity = new Vector2(0, playerRB.velocity.y);
                        }
                        else if (Input.GetAxisRaw("Horizontal") > 0) 
                        {
                            float xMovement = Input.GetAxisRaw("Horizontal");
                            float totalSpeed = xMovement * moveSpeed * moveSpeedModifier;

                            playerRB.velocity = new Vector2(totalSpeed, playerRB.velocity.y);
                        }
                    }
                    else if (againstWallRight)
                    {
                        if (Input.GetAxisRaw("Horizontal") > 0)
                        {
                            playerRB.velocity = new Vector2(0, playerRB.velocity.y);
                        }
                        else if (Input.GetAxisRaw("Horizontal") < 0)
                        {
                            float xMovement = Input.GetAxisRaw("Horizontal");
                            float totalSpeed = xMovement * moveSpeed * moveSpeedModifier;

                            playerRB.velocity = new Vector2(totalSpeed, playerRB.velocity.y);
                        }
                    }
                }
            }
            else if (grounded)
            {
                float xMovement = Input.GetAxisRaw("Horizontal");

                float totalSpeed = 0;

                if (!useAcceleration)
                {
                    totalSpeed = xMovement * moveSpeed * moveSpeedModifier;
                }
                else
                {
                    // player actively trying to move
                    if (xMovement != 0)
                    {
                        // player has yet to reach top speed or is opposing their current direction
                        if (Mathf.Abs(playerRB.velocity.x) < (moveSpeed * moveSpeedModifier) || (Mathf.Sign(playerRB.velocity.x) != Mathf.Sign(xMovement)))
                        {
                            // We increase their speed through linear acceleration
                            float risingSpeed = playerRB.velocity.x + (moveAcceleration * Time.deltaTime * xMovement);

                            // and if they accelerate to faster than top speed, we bring the number back down to top speed
                            if (Mathf.Abs(risingSpeed) >= (moveSpeed * moveSpeedModifier))
                            {
                                totalSpeed = (xMovement * moveSpeed * moveSpeedModifier);
                            }
                            else
                            {
                                totalSpeed = risingSpeed;
                            }
                        }
                        else // Player is maintaining top speed
                        {
                            totalSpeed = (xMovement * moveSpeed * moveSpeedModifier);
                        }
                        
                    }
                    else // player released controls - trying to stop
                    {
                        // Player has yet to stop
                        if (playerRB.velocity.x != 0)
                        {
                            // Implies player is just about stopped, so we'll stop them
                            if (Mathf.Abs(playerRB.velocity.x) < (moveDecceleration * Time.deltaTime))
                            {
                                totalSpeed = 0;
                            }
                            else // Player still has to slow down gradually.
                            {
                                // We know this won't result in them passing 0, due to if statement above.
                                totalSpeed = playerRB.velocity.x - (moveDecceleration * Time.deltaTime * Mathf.Sign(playerRB.velocity.x));
                            }
                        }
                    }
                }

                playerRB.velocity = new Vector2(totalSpeed, playerRB.velocity.y);
            }
        }

    }

    public void Jump(float speed) //BASE JUMP - simply sets a Y vel while maintaining X vel
    {
        playerRB.velocity = new Vector2(playerRB.velocity.x, speed);
        playerAnimator.SetTrigger("jumping");
        grounded = false;
        swimming = false;  
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
        //CheckForWall();

        if (canGrab && !grounded) //if the player can grab (i.e. are in range of a wall)
        {
            allowHoriz = true;
            horizontalDisableTimer = defaultHorizontalTime;
            grabbing = true; //vv important bool - controls what movement is allowed while grabbing. See Update() for all functionality.
            playerRB.velocity = Vector2.zero; //Stop the player from moving
            playerRB.isKinematic = true;
            //playerRB.Sleep(); //disable playerRB so that the player does not fall. Wanted to use gravity, but there was constant overlap with other things.
            playerAnimator.SetTrigger("grabbing");

            player.transform.parent = grabPositionandParent;
        }
    }

    private void StopGrabbing()
    {
        if (grabbing)
        {
            playerRB.isKinematic = false;
            grabbing = false;
        }
    }

    private void WallJump() //tap to wall jump
    {
        CheckForWall();

        if (canGrab)
        {
            GravityToggle(2);
            horizontalDisableTimer = defaultHorizontalTime;

            if (hitDirection == 1)
            {
                allowHoriz = false;
                playerRB.velocity = new Vector2(moveSpeed, jumpSpeed);
                ResetWallJumpVars();
            }
            else if (hitDirection == 0)
            {
                allowHoriz = false;
                playerRB.velocity = new Vector2(-moveSpeed, jumpSpeed);
                ResetWallJumpVars();
            }
        }
    }

    private void JumpFromWall() //Allows the player to wall jump if they are currently grabbing a wall. 
    {
        if (Input.GetButtonDown("Jump") && hitDirection == 1) //hitdirection == 1 is the same as LEFT, i.e wall is on PCs left, so they jump right
        {
            allowHoriz = false; //stop horizontal movement input - see var declaration and HorizontalMovement () for more info.
            playerRB.isKinematic = false;
            //playerRB.WakeUp(); //re-activated playerRB because grabbing wall deactivates it
            playerRB.velocity = new Vector2(moveSpeed, jumpSpeed); //jump right and up
        }
        else if (Input.GetButtonDown("Jump") && hitDirection == 0) //hitdirection == 1 is the same as RIGHT, i.e wall is on PCs right, so they jump left
        {
            allowHoriz = false; //stop horizontal movement input - see var declaration and HorizontalMovement () for more info.
            playerRB.isKinematic = false;
            //playerRB.WakeUp(); //re-activated playerRB because grabbing wall deactivates it
            playerRB.velocity = new Vector2(-moveSpeed, jumpSpeed); //jump left and up
        }
    }
    public void CheckForWall()
    {
        // I made this public to allow multiple layers to be publically assigned for wall jumping.
        // int mask = LayerMask.GetMask("Walls");
        

        RaycastHit hitRight3D;
        Ray rightRay = new Ray(player.transform.position + new Vector3(0.2f, 0.3f, 0), Vector3.right);
        Debug.DrawRay(player.transform.position + new Vector3(0.2f, 0.3f, 0), Vector3.right);

        if (Physics.Raycast(rightRay, out hitRight3D, 0.3f, wallJumpMask)) 
        {
            hitDirection = 0;
            canGrab = true;
            grabPositionandParent = hitRight3D.transform.gameObject.transform;
        }
        


        RaycastHit hitLeft3D;
        Ray leftRay = new Ray(player.transform.position - new Vector3(0.2f, -0.3f, 0), Vector3.left);

        if (Physics.Raycast(leftRay, out hitLeft3D, 0.3f, wallJumpMask))
        {
            hitDirection = 1;
            canGrab = true;
            grabPositionandParent = hitLeft3D.transform.gameObject.transform;
        }

    }

   /* public void CheckForWalls() 
    {
        int[] masks;

        RaycastHit raycastHit;


        bool boxHit = Physics.BoxCast(player.transform.position - new Vector3(0.2f,0,0), player.transform.localScale, transform.right, out raycastHit, transform.rotation, 2f);
        if (boxHit)
        {
            print(raycastHit.collider.gameObject.name);
        }
    }*/

    void ResetWallJumpVars() 
    {
        canGrab = false;
        grabbing = false;
    }

    private void SetGravity(float gravity) 
    {
        Physics.gravity = new Vector3(0, -gravity, 0);
    }

    private void GravityToggle(float gravScale) //function to set gravity to desired amount
    {
        playerRB.AddForce(Physics.gravity * gravScale);
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
        {
            GravityToggle(2);
            //print("nothing");
        }
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

    /// <summary>
    /// Used to change the state of the swimming bool. Should be true when the player is in a potentially deep body of water.
    /// </summary>
    /// <param name="swimmingState">The updated state of the swimming bool.</param>
    public void SetSwimming(bool swimmingState)
    {
        swimming = swimmingState;
    }

    /// <summary>
    /// Returns the current swimming state of the player.
    /// </summary>
    public bool GetSwimming()
    {
        return swimming;
    }

    /// <summary>
    /// Updates moveSpeedModifier - changing the percent of the standard move speed that the player will move at.
    /// Use to adjust for different terrains.
    /// </summary>
    public void SetMoveSpeedModifier(float newPercent)
    {
        moveSpeedModifier = newPercent;
    }


    /// <summary>
    /// Use when player enters terrain that affects their inertia, like ice.
    /// </summary>
    /// <param name="acceleration">The rate at which the player gets up to full speed.</param>
    /// <param name="decceleration">The rate at which the player slows down to a stop.</param>
    public void MoveWithAcceleration(float acceleration, float decceleration)
    {
        useAcceleration = true;
        moveAcceleration = acceleration;
        moveDecceleration = decceleration;
    }

    /// <summary>
    /// Use when player enters stable terrain - like stone. Movement will no longer use inertia and friction.
    /// </summary>
    public void MoveWithoutAcceleration()
    {
        useAcceleration = false;
    }

}

