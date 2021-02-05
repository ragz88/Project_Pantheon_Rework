using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Add this to any body of water deep enough to trigger swimming. It will simulate the feeling of floating in water.
/// </summary>
public class BuoyancyForceController : MonoBehaviour
{
    [Tooltip("The rate at which the player will accelerate upwards the first time they enter the water.")]
    /// <summary>
    /// The rate at which the player will accelerate upwards the first time they enter the water.
    /// </summary>
    public float buoyancyAcceleration = 2f;

    [Tooltip("The depth at which the buoyancyAcceleration will begin affecting the player and where the invisible floor will appear - represents a percent of the player's total height." +
     "\n I.e. if it were 0.75f, the force would be applied when the player is 75% submerged.")]
    /// <summary>
    /// The depth at which the buoyancyAcceleration will begin affecting the player and where the invisible floor will appear - 
    /// represents a percent of the player's total height.
    /// <br>I.e. if it were 0.75f, the force would be applied when the player is 75% submerged.</br>
    /// </summary>
    public float buoyancyDepth = 0.75f;

    [Tooltip("We don't want the invisible floor to spawn in if the player is moving quikly - them bouncing up and down a little feels more natural. Thus, the floorwill only spawn if the player is moving slower than this threshhold and in shallower water than buoyancyDepth.")]
    /// <summary>
    /// We don't want the invisible floor to spawn in if the player is moving quikly - them bouncing up and down a little feels more natural. Thus, the floor
    /// will only spawn if the player is moving slower than this threshhold and in shallower water than buoyancyDepth.
    /// </summary>
    public float velocityThreshold = 2f;


    [Tooltip("This is the collider that gets turned on and off within the water to prevent sinking. Not the same as the water's main trigger collider - rather a collider on a child object of the same size.")]
    /// <summary>
    /// This is the collider that gets turned on and off within the water to prevent sinking.
    /// </summary>
    [SerializeField]
    private BoxCollider dynamicCollider;



    [Tooltip("The maximum speed the player will be pushed vertically by the buoyancy acceleration.")]
    /// <summary>
    /// The maximum speed the player will be pushed vertically by the buoyancy force.
    /// </summary>
    public float maxYVelocity = 3;

    /// <summary>
    /// Stores the position representing the top of the water source.
    /// </summary>
    private float waterSurfaceYPos;


    /// <summary>
    /// The Capsule COllider attached to our player.
    /// </summary>
    private CapsuleCollider playerCollider;

    /// <summary>
    /// The Rigidbdy attached to the player.
    /// </summary>
    private Rigidbody playerRB;



    private void Start()
    {
        // Here, we search through all the box colliders in this body of water until we find the one that's a trigger, then cache a reference to it.
        BoxCollider[] allColliders = GetComponents<BoxCollider>();

        BoxCollider waterTrigger = null;

        for (int i = 0; i < allColliders.Length; i++)
        {
            if (allColliders[i].isTrigger)
            {
                waterTrigger = allColliders[i];
                break;
            }
        }

        // We use this trigger collider to identify the Y position of the water's surface.
        if (waterTrigger != null)
        {
            waterSurfaceYPos = waterTrigger.bounds.max.y;
        }
        else
        {
            Debug.LogWarning("Trigger Collider not found in water source " + gameObject.name);
        }

        
    }


    private void OnTriggerStay(Collider other)
    {        
        if (other.CompareTag("Player"))
        {
            // We need to cache a reference to the player's collider the first time we interact with it.
            if (playerCollider == null)
            {
                playerCollider = other.gameObject.GetComponent<CapsuleCollider>();

                // Adjust the size of the internal collider to reflect the buoyancyDepth.
                dynamicCollider.size = dynamicCollider.size - (dynamicCollider.transform.InverseTransformVector(Vector3.up) * buoyancyDepth * playerCollider.height);
                dynamicCollider.center = dynamicCollider.center - (dynamicCollider.transform.InverseTransformVector(Vector3.up) * buoyancyDepth * playerCollider.height * 0.5f);
            }

            // We aso want to cache a reference to the player's rigidbody.
            if (playerRB == null)
            {
                playerRB = other.gameObject.GetComponent<Rigidbody>();
            }

            // Will place more accurately once all animations are decided on.
            PlayerController3D.pController3D.SetSwimming(true);

            // Visualises the 'Dead Zone'
            /*Debug.DrawLine(new Vector3(transform.position.x, waterSurfaceYPos, transform.position.z), 
                new Vector3(transform.position.x, waterSurfaceYPos - (buoyancyDepth * playerCollider.height), transform.position.z),
                Color.magenta);*/
            
            // Player is below 'dead zone' and must swim upwards
            if ((waterSurfaceYPos - other.transform.position.y) > (buoyancyDepth * playerCollider.height * 0.55f) && playerRB.velocity.y < maxYVelocity)
            {
                // This makes our player accelerate upwards.
                playerRB.velocity = (playerRB.velocity + (Vector3.up * buoyancyAcceleration * Time.deltaTime));
            }
            else
            {
                // Velocity is both positive and lower than our threshhold. Thus, the player is moving slowly upwards, and is just peaking above the water surface.
                if (playerRB.velocity.y > 0 && playerRB.velocity.y < velocityThreshold)
                {
                    // So we stop the wobble up-and-down movement by activating an invisible floor.
                    dynamicCollider.enabled = true;
                }
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        // If the player leaves the water, we'll deactivate our invisible floor and tell them to stop swimming.
        if (other.CompareTag("Player"))
        {
            PlayerController3D.pController3D.SetSwimming(false);
            dynamicCollider.enabled = false;
        }
    }
}
