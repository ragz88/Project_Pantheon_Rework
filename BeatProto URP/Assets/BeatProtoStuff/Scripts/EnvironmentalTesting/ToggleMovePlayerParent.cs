using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMovePlayerParent : MonoBehaviour
{
    private bool playerMoving;
    private GameObject player;
    private bool playerFound = false;

    private void Update()
    {
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            if (player != null)
            { 
                playerMoving = true;
                player.transform.SetParent(null);
            }
        }
        else 
        {
            playerMoving = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            if (!playerFound)
            {
                player = other.gameObject;
                playerFound = true;
            }

            if (!playerMoving)
            {
                other.transform.SetParent(this.gameObject.transform, true);
                Debug.LogError("doing");
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!playerFound)
            { 
                player = other.gameObject;
                playerFound = true;
            }

            if (!playerMoving)
            {
                other.transform.SetParent(this.gameObject.transform, true);
                Debug.LogError("doing");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.parent = null;
            Debug.LogError("cancelling");
        }
    }
}
