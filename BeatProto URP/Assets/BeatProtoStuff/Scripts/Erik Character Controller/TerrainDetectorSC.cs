using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDetectorSC : MonoBehaviour
{
    [Tooltip("The distance the spherecast will travel in search of ground.")]
    /// <summary>
    /// The distance the spherecast will travel in search of ground
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
        characterController.detectTerrain += DetectTerrain;

        characterCollider = characterController.gameObject.GetComponent<CapsuleCollider>();
    }

    private void OnDisable()
    {
        characterController.detectTerrain -= DetectTerrain;
    }

    void OnDrawGizmos()
    {
        characterCollider = GetComponentInParent<CapsuleCollider>();

        // Shows Spherecast in the inspector
        if (visualiseDebugGizmos)
        {
            Gizmos.color = new Color(1, 0.8f, 0.4f, 0.6f);
            Gizmos.DrawSphere(characterCollider.transform.position - (characterCollider.transform.up * checkDistance), characterCollider.bounds.size.x/2);
        }
    }

    TerrainType DetectTerrain()
    {
        // Stores information about what our spherecast hit.
        RaycastHit hit;

        bool hitSomething = Physics.SphereCast(characterCollider.transform.position, characterCollider.bounds.size.x / 2,
            -1 * characterCollider.transform.up, out hit, checkDistance, checkMask);

        print(hitSomething);

        // Cast a sphere wrapping character controller 10 meters forward
        // to see if it is about to hit anything.
        if (hitSomething)
        {
            if (hit.collider.gameObject.CompareTag("Ice"))
            {
                return TerrainType.Ice;
            }
            else
            {
                return TerrainType.Standard;
            }
        }
        else
        {
            return TerrainType.None;
        }
    }
}
