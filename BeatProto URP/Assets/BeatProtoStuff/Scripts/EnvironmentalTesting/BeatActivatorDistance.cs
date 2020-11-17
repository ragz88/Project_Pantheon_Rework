using UnityEngine;

public class BeatActivatorDistance : MonoBehaviour
{

    private float distanceFromPlayer;

    public float distanceForInput;

    private BeatInput myBeatInput;

    void Start()
    {
        myBeatInput = GetComponent<BeatInput>();
    }


    void Update()
    {
        distanceFromPlayer = Vector2.Distance(PlayerController.pController.player.transform.position, transform.position);

        if (distanceFromPlayer < distanceForInput)
        {
            myBeatInput.SetPlayerInRange(true);
            myBeatInput.TogglePossibleInputImage(true);
        }

        else if (distanceFromPlayer > distanceForInput)
        {
            myBeatInput.SetPlayerInRange(false);
            myBeatInput.TogglePossibleInputImage(false);
        }


    }
}
