using UnityEngine;

public class BeatToggleHolder : MonoBehaviour
{
    //public GameObject[] toggleObjects;
    [SerializeField]
    private BeatToggle[] beatEvents;

    private void Start()
    {
        // As multiple toggle holders may be linked to different collections of toggle objects, we unfortunately can't find them all in one shot.
        //beatEvents = FindObjectsOfType<BeatToggle>();

        /*beatEvents = new BeatToggle[toggleObjects.Length];

        for (int i = 0; i < toggleObjects.Length; i++)
        {
            beatEvents[i] = toggleObjects[i].GetComponent<BeatToggle>();
        }*/

    }
    public void OnBeat()
    {
        if (beatEvents != null)
        {
            foreach (BeatToggle beatevent in beatEvents)
            {
                beatevent.BeatEvent();
            }
        }
    }

}
