using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionPrompt : MonoBehaviour
{
    public Text promptText;

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
        if (showPrompt && promptText.color.a < 1)
        {
            promptText.color = promptText.color + new Color(0, 0, 0, fadeSpeed * Time.deltaTime);
        }
        else if (!showPrompt && promptText.color.a > 0)
        {
            promptText.color = promptText.color - new Color(0, 0, 0, fadeSpeed * Time.deltaTime);
        }
        else if (!showPrompt && promptText.color.a <= 0)
        {
            promptText.transform.position = initialPos;
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
            promptText.transform.position = Vector3.Lerp(promptText.transform.position, loopPosition.position, moveSpeed * Time.deltaTime);
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
