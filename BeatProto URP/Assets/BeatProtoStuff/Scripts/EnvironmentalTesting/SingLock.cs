using UnityEngine;
using UnityEngine.UI;

public class SingLock : MonoBehaviour
{

    public SingLockDoor myDoor;
    private SpriteRenderer mySprite;

    public bool unlocked = false;
    public bool addedToDoor = false;
    private bool singCollision = false;

    public float playerMaxDistance;
    private float playerDistance;

    public float stayUnlockedTime;

    private Text countDownText;


    public Color defaultColour;
    public Color unlockedColor;

    void Start()
    {
        mySprite = GetComponent<SpriteRenderer>();
        mySprite.color = defaultColour;
        countDownText = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        //playerDistance = Vector2.Distance(PlayerController.pController.player.transform.position, transform.position);

        // if (playerDistance < playerMaxDistance)
        // {
        //     if (PlayerController.pController.singing)
        //     {
        //         stayUnlockedTime = 5f;
        //         mySprite.color = unlockedColor;
        //         unlocked = true;
        //         countDownText.text = "";
        //     }
        // }
        if (!singCollision)
        {
            if (!myDoor.doorUnlocked)
            {
                if (unlocked)
                {
                    if (stayUnlockedTime > 0)
                    {
                        stayUnlockedTime -= Time.deltaTime;
                        countDownText.text = ((int)stayUnlockedTime + 1).ToString();
                    }
                    if (stayUnlockedTime <= 0)
                    {
                        unlocked = false;
                        mySprite.color = defaultColour;
                        countDownText.text = "";
                    }
                }

            }
        }

        if (myDoor.doorUnlocked)
        {
            countDownText.text = "";
        }

    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "SingCollider")
        {
            stayUnlockedTime = 5f;
            mySprite.color = unlockedColor;
            unlocked = true;
            countDownText.text = "";
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.gameObject.name == "SingCollider")
        {
            stayUnlockedTime = 5f;
            countDownText.text = "";
            singCollision = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "SingCollider")
        {
            singCollision = false;
        }
    }
}
