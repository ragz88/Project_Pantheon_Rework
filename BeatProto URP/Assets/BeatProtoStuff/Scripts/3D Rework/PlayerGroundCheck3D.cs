using UnityEngine;

public class PlayerGroundCheck3D : MonoBehaviour
{

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag != "TutTrigger")
        {
            PlayerController3D.pController3D.SetGrounded(true);
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.tag != "TutTrigger")
        {
            PlayerController3D.pController3D.SetGrounded(true);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag != "TutTrigger")
        {
            PlayerController3D.pController3D.SetGrounded(false);
        }
    }

}
