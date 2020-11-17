using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class ShowMessage : MonoBehaviour
{
    public Text text;

    public float fadeSpeed = 2;

    public float pauseTime = 2;

    float pauseTimer = 0;
    bool showing = false;
    bool fadingAway = false;

    Color initialColour;
    Color showingColour;


    void Start()
    {
        initialColour = text.color;
        showingColour = new Color(text.color.r, text.color.g, text.color.b, 1);
    }

    void Update()
    {
        if (showing)
        {
            if (text.color != showingColour)
            {
                text.color = Color.Lerp(text.color, showingColour, fadeSpeed * Time.deltaTime);
            }
            else
            {
                if (pauseTimer >= pauseTime)
                {
                    showing = false;
                    fadingAway = true;
                }
                else
                {
                    pauseTimer += Time.deltaTime;
                }
            }
        }
        else if (fadingAway)
        {
            if (text.color != initialColour)
            {
                text.color = Color.Lerp(text.color, initialColour, fadeSpeed * Time.deltaTime);
            }
            else
            {
                fadingAway = false;
            }
        }
    }

    public void DisplayMessage (string message)
    {
        text.text = message;
        showing = true;
    }

}
