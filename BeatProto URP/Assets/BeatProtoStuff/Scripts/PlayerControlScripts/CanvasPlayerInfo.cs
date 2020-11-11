using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasPlayerInfo : MonoBehaviour
{
    public Image floatFill;


    private void Update()
    {
        floatFill.fillAmount = PlayerController.pController.floatTime / PlayerController.pController.maxFloatTime;
    }
}
