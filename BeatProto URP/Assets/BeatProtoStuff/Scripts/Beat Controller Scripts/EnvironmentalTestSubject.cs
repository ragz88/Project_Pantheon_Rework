using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentalTestSubject : MonoBehaviour
{
    Image objectImage;

    private void Start()
    {
        objectImage = GetComponent<Image>();
    }

    public void OnBeat()
    {
        if (objectImage.color == Color.cyan)
        {
            objectImage.color = Color.magenta;
        }
        else
        {
            objectImage.color = Color.cyan;
        }
    }
}
