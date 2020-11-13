using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "TutTrigger")
        {
            PlayerController.pController.SetGrounded(true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "TutTrigger")
        {
            PlayerController.pController.SetGrounded(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "TutTrigger")
        {
            PlayerController.pController.SetGrounded(false);
        }
    }

}
