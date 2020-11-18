using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalingObject : MonoBehaviour
{
    public float[] scaleFactors;

    public float scaleAccuracy = 0.05f;

    public float scalingSpeed = 2f;

    public int currentScaleFactor = 0;

    Vector3 initScale;
    bool scaling = false;
    
    
    // Start is called before the first frame update
    void Start()
    {
        initScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (scaling)
        {
            float deltaX = (transform.localScale.x - (scaleFactors[currentScaleFactor] * initScale).x);
            float deltaY = (transform.localScale.y - (scaleFactors[currentScaleFactor] * initScale).y);

            if ((deltaX < scaleAccuracy && deltaX > -scaleAccuracy) && (deltaY < scaleAccuracy && deltaY > -scaleAccuracy))
            {
                scaling = false;
            }
            else
            {
                transform.localScale = Vector3.Lerp(transform.localScale, initScale * scaleFactors[currentScaleFactor], scalingSpeed * Time.deltaTime);
            }
        }
    }


    public void OnBeat()
    {
        currentScaleFactor = (currentScaleFactor + 1) % scaleFactors.Length;
        scaling = true;
    }
}
