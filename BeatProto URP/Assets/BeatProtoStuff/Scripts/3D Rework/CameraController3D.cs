using UnityEngine;

public class CameraController3D : MonoBehaviour
{

    private GameObject target;

    public float dampTime;

    private Vector3 velocity = Vector3.zero;

    public Vector2 minBounds, maxBounds;

    public float zDistance;

    void Start()
    {
        if (PlayerController3D.pController3D.playerFound)
            target = PlayerController3D.pController3D.player;
    }


    // MAKE SURE PLAYER RB IS SET TO INTERPOLATE!!!!!!

    void LateUpdate() 
    {
        SmoothFollow();
    }
    

    void SmoothFollow()
    {
        Vector3 targetPos = new Vector3(target.transform.position.x, target.transform.position.y, -zDistance);
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, dampTime);

        float minX = minBounds.x;
        float minY = minBounds.y;
        float maxX = maxBounds.x;
        float maxY = maxBounds.y;

        float xClamp = Mathf.Clamp(transform.position.x, minX, maxX);
        float yClamp = Mathf.Clamp(transform.position.y, minY, maxY);

        transform.position = new Vector3(xClamp, yClamp, -zDistance);
    }
}
