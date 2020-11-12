using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasPlayerInfo : MonoBehaviour
{
    public Image singFill;


    private void Update()
    {
        singFill.fillAmount = PlayerController.pController.singTime / PlayerController.pController.maxSingTime;
    }
}
