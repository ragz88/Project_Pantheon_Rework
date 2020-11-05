using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingLock : MonoBehaviour
{

    public SingLockDoor myDoor;
    private SpriteRenderer mySprite;

    public bool unlocked = false;
    public bool addedToDoor = false;

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
        playerDistance = Vector2.Distance(PlayerController.pController.player.transform.position, transform.position);

        if (playerDistance < playerMaxDistance)
        {
            if (PlayerController.pController.singing)
            {
                stayUnlockedTime = 5f;
                mySprite.color = unlockedColor;
                unlocked = true;
                countDownText.text = "";
            }
        }

        if (!myDoor.doorUnlocked)
        {
            if (!PlayerController.pController.singing)
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

    }
}
