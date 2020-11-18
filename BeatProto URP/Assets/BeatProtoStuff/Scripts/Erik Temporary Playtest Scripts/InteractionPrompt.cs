using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionPrompt : MonoBehaviour
{
    public Image promptImage;

    bool showPrompt = false;

    public float fadeSpeed = 3f;
    public float moveSpeed = 3f;

    public Transform loopPosition;
    Vector3 initialPos;

    bool moving = false;
    
    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (showPrompt && promptImage.color.a < 1)
        {
            promptImage.color = promptImage.color + new Color(0, 0, 0, fadeSpeed * Time.deltaTime);
        }
        else if (!showPrompt && promptImage.color.a > 0)
        {
            promptImage.color = promptImage.color - new Color(0, 0, 0, fadeSpeed * Time.deltaTime);
        }
        else if (!showPrompt && promptImage.color.a <= 0)
        {
            promptImage.transform.position = initialPos;
            moving = false;
        }

        if (showPrompt)
        {
            if (Input.GetButtonDown("Sing"))
            {
                moving = true;
            }
        }

        if (moving)
        {
            promptImage.transform.position = Vector3.Lerp(promptImage.transform.position, loopPosition.position, moveSpeed * Time.deltaTime);
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            showPrompt = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            showPrompt = false;
        }
    }
}
