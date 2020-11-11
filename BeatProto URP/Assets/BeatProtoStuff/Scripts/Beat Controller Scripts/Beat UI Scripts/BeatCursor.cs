using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatCursor : MonoBehaviour
{
    /// <summary>
    /// The Beat Input UI controller for the set of blocks we want this cursor to be linked to
    /// </summary>
    public BeatUI beatUI;

    /// <summary>
    /// The offset of the cursor from the centre of the beat blocks it points at
    /// </summary>
    public Vector3 cursorOffset;

    // Used to fade cursor in and out depending on the visibility settings of the UI
    Image cursorImage;
    
    // Start is called before the first frame update
    void Start()
    {
        cursorImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    { 
        // We want our cursor to highlight the currently firing beat when the beat blocks are visible

        if (beatUI.UIVisible)
        {
            transform.position =
                beatUI.beatBlockImages[BeatTimingManager.btmInstance.GetBeatNumber() % beatUI.beatBlockImages.Length].transform.position
                + cursorOffset;

            if (cursorImage.color.a < 1)
            {
                cursorImage.color = cursorImage.color + new Color(0, 0, 0, 5 * Time.deltaTime);
            }
        }
        else
        {
            if (cursorImage.color.a > 0)
            {
                cursorImage.color = cursorImage.color - new Color(0, 0, 0, 5 * Time.deltaTime);
            }
        }
    }
}
