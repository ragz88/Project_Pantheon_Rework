using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialToggle : MonoBehaviour
{
    
    void Start()
    {
        InvokeRepeating("OnOff", 2, 1);
    }

    
    void Update()
    {
        
    }

    void OnOff() 
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

}
