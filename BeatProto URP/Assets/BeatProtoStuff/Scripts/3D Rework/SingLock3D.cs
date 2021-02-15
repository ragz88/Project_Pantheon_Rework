using UnityEngine;
using UnityEngine.UI;

public class SingLock3D : MonoBehaviour
{

    [HideInInspector]
    public SingLockDoor3D myDoor;
    //private SpriteRenderer mySprite;
    private MeshRenderer meshRend;

    public bool unlocked = false;
    public bool addedToDoor = false;
    private bool singCollision = false;

    public float playerMaxDistance;
    private float playerDistance;

    public float stayUnlockedTime;

    private float unlockedTimeRemaining = 0;

    private Text countDownText;


    public Color defaultColour;
    public Color unlockedColor;

    void Start()
    {
        meshRend = GetComponent<MeshRenderer>();
        meshRend.material.color = defaultColour;
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
                    if (unlockedTimeRemaining > 0)
                    {
                        unlockedTimeRemaining -= Time.deltaTime;
                        countDownText.text = ((int)unlockedTimeRemaining + 1).ToString();
                    }
                    if (unlockedTimeRemaining <= 0)
                    {
                        unlocked = false;
                        meshRend.material.color = defaultColour;
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


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "SingCollider")
        {
            unlockedTimeRemaining = stayUnlockedTime;
            meshRend.material.color = unlockedColor;
            unlocked = true;
            countDownText.text = ((int)unlockedTimeRemaining + 1).ToString();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "SingCollider")
        {
            unlockedTimeRemaining = stayUnlockedTime;
            countDownText.text = ((int)unlockedTimeRemaining + 1).ToString();
            //singCollision = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "SingCollider")
        {
            //singCollision = false;
        }
    }

}
