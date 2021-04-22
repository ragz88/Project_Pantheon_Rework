using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDetectorSC : MonoBehaviour
{
    [Tooltip("The distance the spherecast will travel in search of ground.")]
    /// <summary>
    /// The distance the spherecasts will travel in search of walls
    /// </summary>
    [SerializeField]
    private float checkDistance = 1f;

    [Tooltip("Describes the layers the spherecast will interact with.")]
    /// <summary>
    /// Describes the layers the spherecast will interact with.
    /// </summary>
    [SerializeField]
    private LayerMask checkMask;


    [Tooltip("Shows a visualisation of where the spherecast will appear if true.")]
    /// <summary>
    /// Shows a visualisation of where the spherecast will appear if true.
    /// </summary>
    [SerializeField]
    private bool visualiseDebugGizmos = false;

    /// <summary>
    /// The character controller for the player character that will be singing. This class should be a child of said character object.
    /// </summary>
    private PlayerCharacterController characterController;

    /// <summary>
    /// The Capsule Collider component of our character
    /// </summary>
    private CapsuleCollider characterCollider;

    private void OnEnable()
    {
        characterController = GetComponentInParent<PlayerCharacterController>();
        characterController.detectWalls += DetectWalls;

        characterCollider = characterController.gameObject.GetComponent<CapsuleCollider>();
    }

    private void OnDisable()
    {
        characterController.detectWalls -= DetectWalls;
    }

    void OnDrawGizmos()
    {
        characterCollider = GetComponentInParent<CapsuleCollider>();

        // Shows Spherecast in the inspector
        if (visualiseDebugGizmos)
        {
            Gizmos.color = new Color(0.3f, 0.9f, 0.4f, 0.6f);
            Gizmos.DrawSphere(characterCollider.transform.position - (characterCollider.transform.right * checkDistance), characterCollider.bounds.size.y / 4);
            Gizmos.DrawSphere(characterCollider.transform.position + (characterCollider.transform.right * checkDistance), characterCollider.bounds.size.y / 4);
        }
    }

    WallDirection DetectWalls()
    {
        // Stores information about what our spherecast hit.
        RaycastHit hit;

        // Cast a sphere, half the character's height in diameter, out to the right of the character's centre in search of walls
        bool hitSomething = Physics.SphereCast(characterCollider.transform.position, characterCollider.bounds.size.y / 4,
            characterCollider.transform.right, out hit, checkDistance, checkMask);

        if (hitSomething)
        {
            return WallDirection.Right;
        }
        else
        {
            // If nothing was found, we check the other direction
            // Cast a sphere, half the character's height in diameter, out to the left of the character's centre in search of walls
            hitSomething = Physics.SphereCast(characterCollider.transform.position, characterCollider.bounds.size.y / 4,
                -1 * characterCollider.transform.right, out hit, checkDistance, checkMask);

            if (hitSomething)
            {
                return WallDirection.Left;
            }
            else
            {
                return WallDirection.None;
            }
        }
    }
}
