using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeColourChanger : MonoBehaviour
{
    public float alphaChange = 0.3f;
    
    float initialAlpha;

    MeshRenderer meshRend;

    WaterController waterController;

    // Start is called before the first frame update
    void Start()
    {
        waterController = GetComponentInParent<WaterController>();

        meshRend = GetComponent<MeshRenderer>();

        initialAlpha = meshRend.material.color.a;
    }

    // Update is called once per frame
    void Update()
    {
        if (waterController.waterState == WaterState.Frozen)
        {
            meshRend.material.color = meshRend.material.color + new Color(0, 0, 0, alphaChange);
        }
        else
        {
            meshRend.material.color = new Color(meshRend.material.color.r, meshRend.material.color.g, meshRend.material.color.b, initialAlpha);
        }
    }
}
