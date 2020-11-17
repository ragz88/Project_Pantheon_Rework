using UnityEngine;

public class BeatCursor : MonoBehaviour
{
    [Tooltip("The Beat Input UI controller for the set of blocks we want this cursor to be linked to.")]
    /// <summary>
    /// The Beat Input UI controller for the set of blocks we want this cursor to be linked to
    /// </summary>
    public BeatUI beatUI;


    [Tooltip("The offset of the cursor from the centre of the beat blocks it points at.")]
    /// <summary>
    /// The offset of the cursor from the centre of the beat blocks it points at
    /// </summary>
    public Vector3 cursorOffset;

    // Used to fade cursor in and out depending on the visibility settings of the UI
    SpriteRenderer cursorSprite;

    // Start is called before the first frame update
    void Start()
    {
        cursorSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // We want our cursor to highlight the currently firing beat when the beat blocks are visible
        if (beatUI.UIVisible && beatUI.UIReady)
        {
            transform.position =
                beatUI.beatBlockSprites[BeatTimingManager.btmInstance.GetBeatNumber() % beatUI.beatBlockSprites.Length].transform.position
                + cursorOffset;

            if (cursorSprite.color.a < 1)
            {
                cursorSprite.color = cursorSprite.color + new Color(0, 0, 0, 5 * Time.deltaTime);
            }
        }
        else
        {
            if (cursorSprite.color.a > 0)
            {
                cursorSprite.color = cursorSprite.color - new Color(0, 0, 0, 5 * Time.deltaTime);
            }
        }
    }
}
