using UnityEngine;

public class CameraController : MonoBehaviour
{

    private GameObject target;

    public float dampTime;

    private Vector3 velocity = Vector3.zero;

    public Vector2 minBounds, maxBounds;

    void Start()
    {
        if (PlayerController.pController.playerFound)
            target = PlayerController.pController.player;
    }


    void FixedUpdate()
    {
        SmoothFollow();
    }

    void SmoothFollow()
    {
        Vector3 targetPos = new Vector3(target.transform.position.x, target.transform.position.y, -10f);
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, dampTime);

        float minX = minBounds.x;
        float minY = minBounds.y;
        float maxX = maxBounds.x;
        float maxY = maxBounds.y;

        float xClamp = Mathf.Clamp(transform.position.x, minX, maxX);
        float yClamp = Mathf.Clamp(transform.position.y, minY, maxY);

        transform.position = new Vector3(xClamp, yClamp, -10);
    }
}
