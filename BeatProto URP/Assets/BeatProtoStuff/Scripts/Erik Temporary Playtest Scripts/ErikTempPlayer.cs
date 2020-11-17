using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErikTempPlayer : MonoBehaviour
{
    public float moveSpeed = 2f;

    public Sprite leftSprite;
    public Sprite rightSprite;
    public Sprite middleSprite;

    Rigidbody2D playerBody;

    SpriteRenderer rend;
    
    // Start is called before the first frame update
    void Start()
    {
        playerBody = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float movement = Input.GetAxis("Horizontal");

        if (movement > 0.3f)
        {
            rend.sprite = rightSprite;
        }
        else if (movement < -0.3f)
        {
            rend.sprite = leftSprite;
        }
        else
        {
            rend.sprite = middleSprite;
        }

        playerBody.velocity = new Vector2(movement * moveSpeed, playerBody.velocity.y);
    }
}
