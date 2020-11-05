using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSingControl : MonoBehaviour
{
    private SpriteRenderer singSprite;

    public Vector3 targetSize;

    private void Start()
    {
        singSprite = GetComponent<SpriteRenderer>();
    }

    public void ActiveSing() 
    {
        singSprite.enabled = true;

        if (!singSprite.transform.localScale.Equals(targetSize))
            singSprite.transform.localScale += new Vector3(0.01f, 0.01f, 0);
        else if (singSprite.transform.localScale.x > targetSize.x)
            singSprite.transform.localScale = targetSize;

    }


}
