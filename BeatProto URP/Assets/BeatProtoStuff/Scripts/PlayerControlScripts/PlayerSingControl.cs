using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSingControl : MonoBehaviour
{
    private SpriteRenderer singSprite;

    public Vector3 targetSize;
    public Vector3 defaultSize;

    private void Start()
    {
        singSprite = GetComponent<SpriteRenderer>();

        defaultSize = transform.localScale;
    }

    public void ActiveSing() 
    {
        singSprite.enabled = true;

        if (singSprite.transform.localScale.x < targetSize.x)
            singSprite.transform.localScale += new Vector3(0.01f, 0f, 0);

        if (singSprite.transform.localScale.y < targetSize.y)
            singSprite.transform.localScale += new Vector3(0f, 0.01f, 0);

    }

    public void StopSing() 
    {
        if (singSprite.transform.localScale.x > defaultSize.x)
            singSprite.transform.localScale -= new Vector3(0.01f, 0f, 0);

        if (singSprite.transform.localScale.y > defaultSize.y)
            singSprite.transform.localScale -= new Vector3(0f, 0.01f, 0);
    }


}
