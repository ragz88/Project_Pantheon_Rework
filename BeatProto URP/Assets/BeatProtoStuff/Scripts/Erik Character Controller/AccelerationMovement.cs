using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerationMovement : MonoBehaviour
{
    [Tooltip("The base speed at which the player moves, before acceleration effects.")]
    /// <summary>
    /// The base speed at which the player moves, before acceleration effects.
    /// </summary>
    [SerializeField]
    float moveSpeed = 3f;



    [SerializeField]
    EnvironmentAcceleration[] accelerationSet;

    private float currentSpeedModifier = 1;  // This represents a percentage multiplier for the initial move speed - used to augment the speed on different terrains.
    private float currentAcceleration;       // Used for different terrains. How quickly the player accelerates to full speed.
    private float currentDecceleration;      // Used for different terrains. How quickly the player deccelerates to a standstill.


    /// <summary>
    /// The character controller for the player character that will be singing. This class should be a child of said character object.
    /// </summary>
    private PlayerCharacterController characterController;

    /// <summary>
    /// The rigidbody of our Player Character.
    /// </summary>
    private Rigidbody playerBody;


    private void OnEnable()
    {
        // We want to add the movement functionality of this class to the character controller.
        characterController = GetComponentInParent<PlayerCharacterController>();
        characterController.calculateXVelocity += CalculateMovementVector;

        playerBody = characterController.gameObject.GetComponent<Rigidbody>();
    }

    private void OnDisable()
    {
        // We want to remove the movement functionality of this class to the character controller.
        characterController.calculateXVelocity -= CalculateMovementVector;
    }

    Vector2 CalculateMovementVector(float xInput, TerrainType currentTerrain, MediumType currentMedium)
    {
        UpdateMovementAugmentations(currentTerrain, currentMedium);

        float finalXMovement = 0;

        // Player is trying to move
        if (xInput != 0)
        {
            finalXMovement = AccelerateMovement(xInput);
        }
        // Player has released controls
        else 
        {
            finalXMovement = DeccelerateMovement(xInput);
        }

        return new Vector2(finalXMovement,0);
    }


    /// <summary>
    /// Using the acceleration of the associated terrain, we calculate an appropriate new speed for our character.
    /// </summary>
    private float AccelerateMovement(float xInput)
    {
        // Player has not reached max speed, or is still moving in the opposite direction of the new movement input
        if ((Mathf.Abs(playerBody.velocity.x) < moveSpeed * currentSpeedModifier) || (Mathf.Sign(playerBody.velocity.x) != Mathf.Sign(xInput)))
        {
            float changingSpeed = playerBody.velocity.x + (currentAcceleration * Time.deltaTime * xInput);

            // We check if the new move speed is larger than the player's top speed, and if so, truncate it.
            if (Mathf.Abs(changingSpeed) > (moveSpeed * currentSpeedModifier))
            {
                return (xInput * moveSpeed * currentSpeedModifier);
            }
            else
            {
                return changingSpeed;
            }
        }
        // Player was already at top speed - so we'll maintain it.
        else
        {
            return (xInput * moveSpeed * currentSpeedModifier);
        }
    }

    /// <summary>
    /// Using the decceleration of the associated terrain, we calculate an appropriate new speed for our character.
    /// </summary>
    private float DeccelerateMovement(float xInput)
    {
        // Player has yet to stop
        if (playerBody.velocity.x != 0)
        {
            // Implies player is just about stopped, so we'll stop them
            if (Mathf.Abs(playerBody.velocity.x) < (currentDecceleration * Time.deltaTime))
            {
                return 0;
            }
            else // Player still has to slow down gradually.
            {
                // We know this won't result in them passing 0, due to if statement above.
                return  (playerBody.velocity.x - (currentDecceleration * Time.deltaTime * Mathf.Sign(playerBody.velocity.x)) );
            }
        }
        // maintain stopped state
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// Updates the accelerations and speed modifiers to those associated with the current terrain and/or medium the player is in.
    /// </summary>
    /// <param name="currentTerrain">The current Terrain the player is travelling on - if any.</param>
    /// <param name="currentMedium">The Current Medium the player is travelling through.</param>
    private void UpdateMovementAugmentations(TerrainType currentTerrain, MediumType currentMedium)
    {
        // Implies we aren't firmly standing on ground, and so the fluid we're in must be used for acceleration calculations.
        if (currentTerrain == TerrainType.None || currentMedium == MediumType.Water)
        {
            int mediumIndex = FindMediumIndex(currentMedium);

            if (mediumIndex != -1)
            {
                // Extract the relevant movement augmentation information from the correct acceleration set.
                currentSpeedModifier = accelerationSet[mediumIndex].moveSpeedModifier;
                currentAcceleration  = accelerationSet[mediumIndex].acceleration;
                currentDecceleration = accelerationSet[mediumIndex].decceleration;
            }
            else
            {
                Debug.LogWarning("Medium " + currentMedium.ToString() + " could not be found in accelerationSet array.");

                // Use Default movement augmentation information from the correct acceleration set.
                currentSpeedModifier = 1;
                currentAcceleration  = 1;
                currentDecceleration = 1;
            }
        }
        // implies we're firmly on some sort of ground
        else
        {
            int terrainIndex = FindTerrainIndex(currentTerrain);

            if (terrainIndex != -1)
            {
                // Extract the relevant movement augmentation information from the correct acceleration set.
                currentSpeedModifier = accelerationSet[terrainIndex].moveSpeedModifier;
                currentAcceleration  = accelerationSet[terrainIndex].acceleration;
                currentDecceleration = accelerationSet[terrainIndex].decceleration;
            }
            else
            {
                Debug.LogWarning("Terrain " + currentTerrain.ToString() + " could not be found in accelerationSet array.");

                // Use Default movement augmentation information from the correct acceleration set.
                currentSpeedModifier = 1;
                currentAcceleration  = 1;
                currentDecceleration = 1;
            }
        }
    }


    /// <summary>
    /// Finds the index of the accelerationSet element that corrolates to the given terrain.
    /// Returns -1 if nothing is found.
    /// </summary>
    /// <returns>The index of the accelerationSet element that corrolates to the given terrain. Returns -1 if nothing is found.</returns>
    private int FindTerrainIndex(TerrainType terrain)
    {
        int index = -1;

        // We loop through the accelerationSet array looking for a matching terrain, and store the index and break out if we find one
        for (int i = 0; i < accelerationSet.Length; i++)
        {
            if (accelerationSet[i].terrainType == terrain)
            {
                index = i;
                break;
            }
        }

        return index;
    }


    /// <summary>
    /// Finds the index of the accelerationSet element that corrolates to the given medium.
    /// Returns -1 if nothing is found.
    /// </summary>
    /// <returns>The index of the accelerationSet element that corrolates to the given medium. Returns -1 if nothing is found.</returns>
    private int FindMediumIndex(MediumType medium)
    {
        int index = -1;

        // We loop through the accelerationSet array looking for a matching terrain, and store the index and break out if we find one
        for (int i = 0; i < accelerationSet.Length; i++)
        {
            if (accelerationSet[i].mediumType == medium)
            {
                index = i;
                break;
            }
        }

        return index;
    }
}
