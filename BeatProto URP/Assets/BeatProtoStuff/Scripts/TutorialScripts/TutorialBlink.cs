using UnityEngine;

public class TutorialBlink : MonoBehaviour
{

    bool opening;
    bool closing;

    public float targetSize;

    public float blinkDelay;

    float timer;

    void Start()
    {
        closing = true;
    }


    void Update()
    {
        if (closing)
        {
            transform.localScale -= new Vector3(0, 0.005f, 0);
            if (transform.localScale.y <= 0)
            {
                closing = false;
                opening = true;
            }
        }

        if (opening)
        {
            if (transform.localScale.y < targetSize)
                transform.localScale += new Vector3(0, 0.005f, 0);
        }


        if (transform.localScale.y >= targetSize && opening)
        {
            timer += Time.deltaTime;

            if (timer >= blinkDelay)
            {
                closing = true;
                opening = false;
                timer = 0;
            }
        }

    }
}
