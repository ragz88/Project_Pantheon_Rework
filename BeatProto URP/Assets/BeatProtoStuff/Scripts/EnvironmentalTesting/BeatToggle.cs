using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatToggle : MonoBehaviour
{

    public GameObject toggleObject;

    public Vector2 defaultSize;
    public Vector2 newSize;

    public enum ToggleType 
    { 
        OnOff, Size
    }

    public ToggleType myType;

    


    public void OnBeat() 
    {
        if (myType == ToggleType.OnOff)
        {
            toggleObject.SetActive(!toggleObject.activeSelf);
        }

        else if (myType == ToggleType.Size) 
        {
            if (transform.localScale.Equals(defaultSize))
            {
                transform.localScale = newSize;
            }
            else if (transform.localScale.Equals(newSize)) 
            {
                transform.localScale = defaultSize;
            }
        }
    }
}
