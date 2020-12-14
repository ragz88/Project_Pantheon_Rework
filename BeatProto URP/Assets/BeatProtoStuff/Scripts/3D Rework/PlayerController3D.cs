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
    }


    void Update() //lots of stuff happening here. Not sure if should simplify or if it's fine. 
    {

        print(grounded);

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
                    float xMovement = Input.GetAxisRaw("Horizontal");
                    float totalSpeed = xMovement * moveSpeed;

                    playerRB.velocity = new Vector2(totalSpeed, playerRB.velocity.y);
                }
            }
            else if (grounded)
            {
                float xMovement = Input.GetAxisRaw("Horizontal");
                float totalSpeed = xMovement * moveSpeed;

                playerRB.velocity = new Vector2(totalSpeed, playerRB.velocity.y);
            }
        }

    }

    public void Jump(float speed) //BASE JUMP - simply sets a Y vel while maintaining X vel
    {
        playerRB.velocity = new Vector2(playerRB.velocity.x, speed);
        playerAnimator.SetTrigger("jumping");
        grounded = false;
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
        CheckForWall();

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
            }
            else if (hitDirection == 0)
            {
                allowHoriz = false;
                playerRB.velocity = new Vector2(-moveSpeed, jumpSpeed);
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
        int mask = LayerMask.GetMask("Walls");
        RaycastHit2D hitRight = Physics2D.Raycast(player.transform.position + new Vector3(0.2f, 0.3f, 0), Vector2.right, 0.3f, mask);
        RaycastHit2D hitLeft = Physics2D.Raycast(player.transform.position - new Vector3(0.2f, -0.3f, 0), Vector2.left, 0.3f, mask);
        Debug.DrawRay(player.transform.position + new Vector3(0.2f, 0.3f, 0), new Vector2(0.2f, 0), Color.red);
        Debug.DrawRay(player.transform.position - new Vector3(0.2f, -0.3f, 0), new Vector2(-0.2f, 0), Color.red);

        if (hitRight)
        {
            hitDirection = 0;
            canGrab = true;
            grabPositionandParent = hitRight.transform.gameObject.transform;
        }
        if (hitLeft)
        {
            hitDirection = 1;
            canGrab = true;
            grabPositionandParent = hitLeft.transform.gameObject.transform;
        }
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
            GravityToggle(0);
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

