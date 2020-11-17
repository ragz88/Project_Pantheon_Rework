using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class Flowers : MonoBehaviour
{
    public int numPetals;

    public float petalRadius = 1;

    public GameObject petalPrefab;

    public float colourChangeSpeed = 5;

    SpriteRenderer[] petals;

    Color[] currentColours;

    public BeatController redController;

    public BeatController yellowController;

    public BeatController blueController;

    [Header("Response Colours")]

    public Color redColour;
    public Color yellowColour;
    public Color orangeColour;
    public Color blueColour;
    public Color purpleColour;
    public Color greenColour;
    public Color mixColour;


    private void Start()
    {
        petals = new SpriteRenderer[numPetals];

        currentColours = new Color[numPetals];

        for (int i = 0; i < numPetals; i++)
        {
            float currentAngle = (( (numPetals - i) / ((float)numPetals)) * 2 * Mathf.PI) + (0.5f * Mathf.PI);

            float xPos = petalRadius * Mathf.Cos(currentAngle);
            float yPos = petalRadius * Mathf.Sin(currentAngle);

            GameObject newPetalObj = 
            Instantiate(petalPrefab, transform.position + new Vector3(xPos, yPos, 0), Quaternion.identity, transform) as GameObject;

            newPetalObj.transform.eulerAngles = new Vector3(0,0, currentAngle * (180/Mathf.PI) - 90);
            petals[i] = newPetalObj.GetComponent<SpriteRenderer>();
            currentColours[i] = Color.white;
        }
    }

    private void Update()
    {
        for (int i = 0; i < petals.Length; i++)
        {
            petals[i].color = Color.Lerp(petals[i].color, currentColours[i], colourChangeSpeed * Time.deltaTime);
        }
    }

    public void OnBeat()
    {
        for (int i = 0; i < petals.Length; i++)
        {
            bool redPresent = false;
            bool bluePresent = false;
            bool yellowPresent = false;

            if (redController != null)
            {
                if (redController.activeBeats[i])
                {
                    redPresent = true;
                }
            }

            if (yellowController != null)
            {
                if (yellowController.activeBeats[i])
                {
                    yellowPresent = true;
                }
            }

            if (blueController != null)
            {
                if (blueController.activeBeats[i])
                {
                    bluePresent = true;
                }
            }


            if (redPresent)
            {
                if (bluePresent && yellowPresent)
                {
                    currentColours[i] = mixColour;
                }
                else if (bluePresent)
                {
                    currentColours[i] = purpleColour;
                }
                else if (yellowPresent)
                {
                    currentColours[i] = orangeColour;
                }
                else
                {
                    currentColours[i] = redColour;
                }
            }
            else if (bluePresent)
            {
                if (yellowPresent)
                {
                    currentColours[i] = greenColour;
                }
                else
                {
                    currentColours[i] = blueColour;
                }
            }
            else if (yellowPresent)
            {
                currentColours[i] = yellowColour;
            }
            else
            {
                currentColours[i] = Color.white;
            }
        }
        
    }
}
