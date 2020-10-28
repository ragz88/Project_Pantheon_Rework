using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{


    public static PlayerController pController;

    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public bool playerFound = false;

    private Rigidbody2D playerRB;
    private SpriteRenderer playerSprite;

    private bool grounded;
    private bool floating;

    public float moveSpeed;
    public float jumpSpeed;

    public float floatGravityFactor;
    public float fallGravityFactor;


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
        
    }

    
    void Update()
    {
        HorizontalMovement();

        if (grounded)
            Jump();

        if (!grounded)
            Float();

        HandleGravity();



        if (playerRB.velocity.x > 0)
        {
            FlipPlayerSprite(false);
        }
        else if (playerRB.velocity.x < 0) 
        {
            FlipPlayerSprite(true);
        }
        
    }

    private void GetPlayer() 
    {
        if (player == null)
        {
            player = GameObject.Find("Player");

            if (player != null)
                playerFound = true;

            else if (player == null)
                Debug.Log("Error: Player not found");
        }
        else 
        {
            return;
        }

        playerRB = player.GetComponent<Rigidbody2D>();
        playerSprite = player.GetComponentInChildren<SpriteRenderer>();
    }


    private void HorizontalMovement() 
    {
        float xMovement = Input.GetAxisRaw("Horizontal");
        float totalSpeed = xMovement * moveSpeed;

        playerRB.velocity = new Vector2(totalSpeed, playerRB.velocity.y);

        
    }

    private void Jump() 
    {
        if (Input.GetKeyDown(KeyCode.Space))
        { 
            playerRB.velocity = new Vector2(playerRB.velocity.x, jumpSpeed);
            grounded = false;
        }
    }

    private void GravityToggle (float gravScale)
    {
        playerRB.gravityScale = gravScale;
    }

    private void FlipPlayerSprite(bool shouldFlip) 
    {
        playerSprite.flipX = shouldFlip;
    }

    private void Float() 
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            floating = true;
        }
        else
            floating = false;
    }

    private void HandleGravity() 
    {
        if (!grounded && playerRB.velocity.y < 0 && !floating)
        {
            GravityToggle(fallGravityFactor);
        }
        else if (!grounded && playerRB.velocity.y < 0 && floating)
        {
            GravityToggle(floatGravityFactor);
        }
        else if (grounded)
            GravityToggle(1);
    }


    public void SetGrounded (bool ground)
    {
        grounded = ground;
    }
}
