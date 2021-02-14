using UnityEngine;

public class BeatToggleHolder : MonoBehaviour
{
    //public GameObject[] toggleObjects;

    private BeatToggle[] beatEvents;

    private void Start()
    {

        beatEvents = FindObjectsOfType<BeatToggle>();

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
