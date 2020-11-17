using UnityEngine;

public class HUDCursor : MonoBehaviour
{
    [Tooltip("The Beat HUD we want this cursor to be linked to.")]
    /// <summary>
    /// The Beat HUD we want this cursor to be linked to.
    /// </summary>
    public BeatHUD beatHUD;

    [Tooltip("The offset of the cursor from the centre of the beat blocks it points at.")]
    /// <summary>
    /// The offset of the cursor from the centre of the beat blocks it points at
    /// </summary>
    public Vector3 cursorOffset;

    // Update is called once per frame
    void Update()
    {
        // We move our cursor to the position of the currently firing beat.
        transform.position =
            beatHUD.beatBlockImages[BeatTimingManager.btmInstance.GetBeatNumber() % beatHUD.beatBlockImages.Length].transform.position
            + cursorOffset;
    }
}
