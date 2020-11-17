using UnityEngine;

public class PlayerSingControl : MonoBehaviour
{
    private SpriteRenderer singSprite;

    private GameObject singCircle;

    public Vector3 targetSize;
    public Vector3 defaultSize;

    private void Start()
    {
        singSprite = GetComponent<SpriteRenderer>();

        singCircle = this.gameObject;

        defaultSize = transform.localScale;
    }

    public void ActiveSing()
    {
        singSprite.enabled = true;

        singCircle.SetActive(true);

        //if (singSprite.transform.localScale.x < targetSize.x)
        //    singSprite.transform.localScale += new Vector3(0.01f, 0f, 0);

        //if (singSprite.transform.localScale.y < targetSize.y)
        //    singSprite.transform.localScale += new Vector3(0f, 0.01f, 0);

        if (singCircle.transform.localScale.x < targetSize.x)
            singCircle.transform.localScale += new Vector3(0.01f, 0f, 0);

        if (singCircle.transform.localScale.y < targetSize.y)
            singCircle.transform.localScale += new Vector3(0f, 0.01f, 0);

    }

    public void StopSing()
    {
        //if (singSprite.transform.localScale.x > defaultSize.x)
        //    singSprite.transform.localScale -= new Vector3(0.01f, 0f, 0);

        //if (singSprite.transform.localScale.y > defaultSize.y)
        //    singSprite.transform.localScale -= new Vector3(0f, 0.01f, 0);

        singCircle.SetActive(false);

        if (singCircle.transform.localScale.x > defaultSize.x)
            singCircle.transform.localScale -= new Vector3(0.01f, 0f, 0);

        if (singCircle.transform.localScale.y > defaultSize.y)
            singCircle.transform.localScale -= new Vector3(0f, 0.01f, 0);
    }


}
