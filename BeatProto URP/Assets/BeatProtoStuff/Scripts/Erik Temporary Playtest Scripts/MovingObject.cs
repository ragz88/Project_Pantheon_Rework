using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public Transform[] movementPoints;

    public float moveSpeed = 5;

    public float moveAccuracy = 0.1f;

    [HideInInspector]
    public int currentMovePoint = 0;
    bool moving = false;
    
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < movementPoints.Length; i++)
        {
            if (movementPoints[i].parent == transform)
            {
                movementPoints[i].parent = transform.parent;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            if (   (transform.position.x - movementPoints[currentMovePoint].position.x < moveAccuracy && transform.position.x - movementPoints[currentMovePoint].position.x > -moveAccuracy)
                && (transform.position.y - movementPoints[currentMovePoint].position.y < moveAccuracy && transform.position.y - movementPoints[currentMovePoint].position.y > -moveAccuracy))
            {
                moving = false;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, movementPoints[currentMovePoint].position, moveSpeed * Time.deltaTime);
            }
        }
    }

    public void OnBeat()
    {
        currentMovePoint = (currentMovePoint + 1) % movementPoints.Length;
        moving = true;
    }
}
