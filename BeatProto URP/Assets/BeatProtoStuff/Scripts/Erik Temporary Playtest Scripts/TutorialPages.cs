using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPages : MonoBehaviour
{

    public Sprite[] tutPages;

    public GameObject player;

    public GameObject surveyButton;

    public int[] surveyButtonPages;

    Image tutPageImage;

    int currentPageNumber = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        tutPageImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPageNumber == 0 && player != null)
        {
            player.SetActive(false);
        }
        
        if (Input.GetButtonDown("Jump"))
        {
            if (currentPageNumber != tutPages.Length - 1)
            {
                currentPageNumber++;

                tutPageImage.sprite = tutPages[currentPageNumber];

                if (surveyButton != null)
                {
                    bool onSurveyPage = false;

                    for (int i = 0; i < surveyButtonPages.Length; i++)
                    {
                        if (currentPageNumber == surveyButtonPages[i])
                        {
                            onSurveyPage = true;
                        }
                    }

                    if (onSurveyPage)
                    {
                        surveyButton.SetActive(true);
                    }
                    else
                    {
                        surveyButton.SetActive(false);
                    }
                }
            }
            else
            {
                if (player != null)
                {
                    player.SetActive(true);
                }

                Destroy(gameObject);
            }
        }
    }
}
