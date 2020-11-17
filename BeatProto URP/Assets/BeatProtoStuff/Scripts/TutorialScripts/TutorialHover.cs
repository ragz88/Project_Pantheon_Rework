using UnityEngine;

public class TutorialHover : MonoBehaviour
{
    Vector3 startpos;

    [Tooltip("Higher number makes it take longer")]
    public float hoverTime;

    public float hoverDistance;

    void Start()
    {
        startpos = transform.position;
    }


    void Update()
    {

        //*** THIS INTERFERES WITH GRABBING A WALL. IM SO CONFUSED
        // IT SEEMS LIKE ANYTHING MOVING IN THE SCENE STOPS THE PLAYER RB FROM SLEEPING. NEED WORKAROUND
        transform.position = new Vector3(transform.position.x, (Mathf.PingPong(Time.time / hoverTime, hoverDistance) + startpos.y), transform.position.z);


    }
}
