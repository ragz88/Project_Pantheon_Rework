using UnityEngine;
using UnityEngine.UI;

public class CanvasPlayerInfo3D : MonoBehaviour
{

    public bool wallJumpTutorial;

    public Image singFill;

    public Text wallJumpMode;

    public GameObject HoldAndSpace;
    public GameObject JustSpace;

    private void Update()
    {
        singFill.fillAmount = PlayerController3D.pController3D.singTime / PlayerController3D.pController3D.maxSingTime;


       /* if (wallJumpTutorial)
        {
            if (PlayerController3D.pController3D.tapWallJump)
            {
                wallJumpMode.text = "Wall jump mode: JUMP towards the pink wall, then tap SPACE to WALLJUMP";
                JustSpace.SetActive(true);
                HoldAndSpace.SetActive(false);
            }
            else if (!PlayerController3D.pController3D.tapWallJump)
            {
                wallJumpMode.text = "Wall jump mode: jump towards the pink wall and tap E to hold onto it. Tap SPACE to WALLJUMP";
                JustSpace.SetActive(false);
                HoldAndSpace.SetActive(true);
            }
        }
         else if (!wallJumpTutorial)
         { 
             wallJumpMode.text = "";

             JustSpace.SetActive(false);
             HoldAndSpace.SetActive(false);
         }*/
    }
}
