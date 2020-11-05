using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingLockDoor : MonoBehaviour
{

    public SingLock[] myLocks;

    private SpriteRenderer mySprite;

    private int unlockCount = 0;

    public bool doorUnlocked = false;

    void Start()
    {
        mySprite = GetComponent<SpriteRenderer>();
        SetDoorForLocks();
    }

    
    void Update()
    {
        if (!doorUnlocked)
            CheckLocks();
    }

    public void CheckLocks() 
    {
        foreach (SingLock singLock in myLocks) 
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
            mySprite.color = Color.black;
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    public void SetDoorForLocks()
    {
        foreach (SingLock singLock in myLocks)
        {
            singLock.myDoor = this;
        }
    }
}
