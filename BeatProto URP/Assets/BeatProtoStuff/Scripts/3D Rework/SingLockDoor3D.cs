using UnityEngine;

public class SingLockDoor3D : MonoBehaviour
{

    public enum unlockType { changeColor, disappear }

    public unlockType myUnlockType;

    public SingLock3D[] myLocks;

    private SpriteRenderer mySprite;

    private int unlockCount = 0;

    public bool doorUnlocked = false;

    bool startFade = false;

    public Color unlockColor;


    void Start()
    {
        mySprite = GetComponent<SpriteRenderer>();
        SetDoorForLocks();

    }


    void Update()
    {
        if (!doorUnlocked)
            CheckLocks();

        if (startFade)
        {
            Color newCol = mySprite.color;
            newCol.a -= 0.001f;

            mySprite.color = newCol;
        }
    }

    public void CheckLocks()
    {
        foreach (SingLock3D singLock in myLocks)
        {
            if (singLock.unlocked)
            {
                if (!singLock.addedToDoor)
                {
                    unlockCount++;
                    singLock.addedToDoor = true;
                }
            }
            else if (!singLock.unlocked)
            {
                if (singLock.addedToDoor)
                {
                    unlockCount--;
                    singLock.addedToDoor = false;
                }
            }
        }


        if (unlockCount == myLocks.Length)
        {
            doorUnlocked = true;
        }

        if (doorUnlocked)
        {
            if (myUnlockType == unlockType.changeColor)
            {
                mySprite.color = unlockColor;
                GetComponent<BoxCollider>().enabled = false;
            }
            else if (myUnlockType == unlockType.disappear)
            {
                startFade = true;
                //gameObject.SetActive(false);
                GetComponent<BoxCollider>().enabled = false;
            }
        }
    }

    public void SetDoorForLocks()
    {
        foreach (SingLock3D singLock in myLocks)
        {
            singLock.myDoor = this;
        }
    }




}
