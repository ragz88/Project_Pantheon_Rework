using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterController : MonoBehaviour
{
    public WaterState waterState = WaterState.Liquid;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            OnActiveBeat();
        }
    }

    public void OnActiveBeat()
    {
        if (waterState == WaterState.Liquid)
        {
            Freeze();
        }
        else
        {
            Melt();
        }
    }


    void Freeze()
    {
        waterState = WaterState.Frozen;
    }


    void Melt()
    {
        waterState = WaterState.Liquid;
    }
}
