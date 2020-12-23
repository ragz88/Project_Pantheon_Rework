using UnityEngine;

public class PlayerWallCheck : MonoBehaviour
{


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            //PlayerController.pController.SetGrabPossible(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {

            if (PlayerController.pController.movingUpWall) 
            {
                PlayerController.pController.Jump(PlayerController.pController.jumpSpeed/2);
            }

            PlayerController.pController.SetGrabPossible(false);

            PlayerController.pController.SetGrabState(false);

            PlayerController.pController.movingUpWall = false;
        }
    }
}
