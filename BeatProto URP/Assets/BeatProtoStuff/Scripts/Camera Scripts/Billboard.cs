using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 2D Objects with this script will always tilt to face the camera.
/// </summary>
public class Billboard : MonoBehaviour
{
    Transform camTrans;


    // Start is called before the first frame update
    void Start()
    {
        camTrans = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(camTrans);
        transform.eulerAngles = new Vector3(-transform.eulerAngles.x, 0, 0);
    }
}
