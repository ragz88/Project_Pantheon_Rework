using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatInput : MonoBehaviour
{


    // Defines whether the user's inputs (and lack thereof) affect the construction of a beat pattern or not.
    bool editMode = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            editMode = !editMode;
        }

        if (editMode)
        {

        }
    }
}
