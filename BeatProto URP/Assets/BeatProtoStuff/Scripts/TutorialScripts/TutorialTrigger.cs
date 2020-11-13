using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialTrigger : MonoBehaviour
{
    [TextArea (1,3)]
    public string tutorialMessage;

    public Text tutorialText;

    public bool turnOffAfterOneUse;
    private bool alreadyTriggered = false;

    public GameObject speechBubble;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (turnOffAfterOneUse)
            {
                if (!alreadyTriggered)
                {
                    alreadyTriggered = true;
                    speechBubble.SetActive(true);
                    tutorialText.text = tutorialMessage;
                }
            }
            else if (!turnOffAfterOneUse) 
            {
                speechBubble.SetActive(true);
                tutorialText.text = tutorialMessage;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") 
        {
            speechBubble.SetActive(false);
            tutorialText.text = "";
        }
    }
}
