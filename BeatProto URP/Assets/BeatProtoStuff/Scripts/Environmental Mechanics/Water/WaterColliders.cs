using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterColliders : MonoBehaviour
{
    /// <summary>
    /// Representation of 4 reference-based directions - Up, Down, Left, Right.
    /// </summary>
    public enum FreezeDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    /// <summary>
    /// The direction the player should be pushed if they're within the waterfall as it freezes.
    /// </summary>
    [SerializeField]
    private FreezeDirection freezeDirection = FreezeDirection.Up;


    
    /// <summary>
    /// These are the physical colliders to be activated when the body of water freezes.
    /// </summary>
    [SerializeField]
    private BoxCollider[] internalColliders;

    /*/// <summary>
    /// The persistent trigger aligning to the water's shape - used to apply momentum effects and outline the shape of the water.
    /// </summary>
    Collider waterTrigger;*/

    /// <summary>
    /// The speed at which the physical colliders expand to full size when the water freezes.
    /// </summary>
    [SerializeField]
    private float freezeSpeed = 1f;

    /// <summary>
    /// Used to change waterfall tags when freezing to allow the player to jump off of them like walls.
    /// </summary>
    [SerializeField]
    private bool isWaterfall = false;

    /// <summary>
    /// The full width/height that the solid ice collider will grow to.
    /// </summary>
    float[] colliderInitialSizes;

    bool freezingComplete = false;

    bool meltingComplete = true;

    WaterController waterController;

    // Start is called before the first frame update
    void Start()
    {
        waterController = GetComponentInParent<WaterController>();
        
        // We don't want to be switching our Trigger collider off - so we'll temporarily store ALL of the collides in this body of water,
        // then loop through them, only adding colliders to our final internalColliders array if they aren't triggers.
        BoxCollider[] tempInternalColliders = GetComponents<BoxCollider>();

        // As water bodies only need one trigger collider, we know the final size of this array is one less than the total collider count.
        internalColliders = new BoxCollider[tempInternalColliders.Length - 1];

        int indexShift = 0;
        for (int i = 0; i < tempInternalColliders.Length; i++)
        {
            if (tempInternalColliders[i].isTrigger)
            {
                // This marks the point where we skip over storing the trigger collider
                indexShift = -1;
            }
            else
            {
                internalColliders[i + indexShift] = tempInternalColliders[i];
            }
        }

        colliderInitialSizes = new float[internalColliders.Length];

        if (freezeDirection == FreezeDirection.Left || freezeDirection == FreezeDirection.Right)
        {
            for (int i = 0; i < internalColliders.Length; i++)
            {
                colliderInitialSizes[i] = internalColliders[i].size.x;
            }
        }
        else
        {
            for (int i = 0; i < internalColliders.Length; i++)
            {
                colliderInitialSizes[i] = internalColliders[i].size.y;
            }
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        if (waterController.waterState == WaterState.Frozen)
        {
            // As at least some freezing has happened, we need to prepare for the potential need to melt again. We also need to solidify the ice that already
            // exists by turning the colliders back on.
            if (meltingComplete)
            {
                for (int i = 0; i < internalColliders.Length; i++)
                {
                    internalColliders[i].enabled = true;
                }

                meltingComplete = false;

                if (isWaterfall)
                {
                    gameObject.tag = "Wall";
                    gameObject.layer = LayerMask.NameToLayer("Walls");
                }
            }

            if (!freezingComplete)
            {
                // We reinitialise this before each loop through the list of colliders. If we find a collider while 
                // looping that hasn't expanded all the way, we'll set it back to false.
                freezingComplete = true;

                for (int i = 0; i < internalColliders.Length; i++)
                {
                    switch(freezeDirection)
                    {
                        case FreezeDirection.Up:
                            
                            if (colliderInitialSizes[i] > internalColliders[i].size.y)
                            {
                                // This ensures we don't expand too far
                                if (internalColliders[i].size.y + (Time.deltaTime * freezeSpeed) > colliderInitialSizes[i])
                                {
                                    internalColliders[i].center
                                        = new Vector3(internalColliders[i].center.x, internalColliders[i].center.y + (colliderInitialSizes[i] - internalColliders[i].size.y) * 0.5f, internalColliders[i].center.z);
                                    internalColliders[i].size
                                        = new Vector3(internalColliders[i].size.x, colliderInitialSizes[i], internalColliders[i].size.z);
                                }
                                else
                                {
                                    internalColliders[i].size
                                        = new Vector3(internalColliders[i].size.x, internalColliders[i].size.y + (Time.deltaTime * freezeSpeed), internalColliders[i].size.z);
                                    internalColliders[i].center
                                        = new Vector3(internalColliders[i].center.x, internalColliders[i].center.y + (Time.deltaTime * freezeSpeed * 0.5f), internalColliders[i].center.z);

                                    freezingComplete = false;
                                }

                            }
                            break;

                        case FreezeDirection.Down:

                            if (colliderInitialSizes[i] > internalColliders[i].size.y)
                            {
                                // Ensure we don't expand too far
                                if (internalColliders[i].size.y + (Time.deltaTime * freezeSpeed) > colliderInitialSizes[i])
                                {
                                    internalColliders[i].center
                                        = new Vector3(internalColliders[i].center.x, internalColliders[i].center.y - (colliderInitialSizes[i] - internalColliders[i].size.y) * 0.5f, internalColliders[i].center.z);
                                    internalColliders[i].size
                                        = new Vector3(internalColliders[i].size.x, colliderInitialSizes[i], internalColliders[i].size.z);
                                }
                                else
                                {
                                    internalColliders[i].size
                                        = new Vector3(internalColliders[i].size.x, internalColliders[i].size.y + (Time.deltaTime * freezeSpeed), internalColliders[i].size.z);
                                    internalColliders[i].center
                                        = new Vector3(internalColliders[i].center.x, internalColliders[i].center.y - (Time.deltaTime * freezeSpeed * 0.5f), internalColliders[i].center.z);

                                    freezingComplete = false;
                                }
                            }
                            break;

                        case FreezeDirection.Left:

                            if (colliderInitialSizes[i] > internalColliders[i].size.x)
                            {
                                // Ensure we don't expand too far
                                if (internalColliders[i].size.x + (Time.deltaTime * freezeSpeed) > colliderInitialSizes[i])
                                {
                                    internalColliders[i].center
                                        = new Vector3(internalColliders[i].center.x - ((colliderInitialSizes[i] - internalColliders[i].size.x) * 0.5f), internalColliders[i].center.y, internalColliders[i].center.z);
                                    internalColliders[i].size
                                        = new Vector3(colliderInitialSizes[i], internalColliders[i].size.y , internalColliders[i].size.z);
                                }
                                else
                                {
                                    internalColliders[i].size
                                        = new Vector3(internalColliders[i].size.x + (Time.deltaTime * freezeSpeed), internalColliders[i].size.y, internalColliders[i].size.z);
                                    internalColliders[i].center
                                        = new Vector3(internalColliders[i].center.x - (Time.deltaTime * freezeSpeed * 0.5f), internalColliders[i].center.y, internalColliders[i].center.z);

                                    freezingComplete = false;
                                }
                            }
                            break;

                        case FreezeDirection.Right:

                            if (colliderInitialSizes[i] > internalColliders[i].size.x)
                            {
                                // Ensure we don't expand too far
                                if (internalColliders[i].size.x + (Time.deltaTime * freezeSpeed) > colliderInitialSizes[i])
                                {
                                    internalColliders[i].center
                                        = new Vector3(internalColliders[i].center.x + ((colliderInitialSizes[i] - internalColliders[i].size.x) * 0.5f), internalColliders[i].center.y, internalColliders[i].center.z);
                                    internalColliders[i].size
                                        = new Vector3(colliderInitialSizes[i], internalColliders[i].size.y, internalColliders[i].size.z);
                                }
                                else
                                {
                                    internalColliders[i].size
                                        = new Vector3(internalColliders[i].size.x + (Time.deltaTime * freezeSpeed), internalColliders[i].size.y, internalColliders[i].size.z);
                                    internalColliders[i].center
                                        = new Vector3(internalColliders[i].center.x + (Time.deltaTime * freezeSpeed * 0.5f), internalColliders[i].center.y, internalColliders[i].center.z);

                                    freezingComplete = false;
                                }
                            }
                            break;
                    }
                }
            }
        }
        else
        {
            // As at least some melting has happened, we need to prepare for the potential need to freeze again.
            freezingComplete = false;

            if (!meltingComplete)
            {
                // We reinitialise this before each loop through the list of colliders. If we find a collider while 
                // looping that hasn't contracted all the way, we'll set it back to false.
                meltingComplete = true;

                for (int i = 0; i < internalColliders.Length; i++)
                {
                    switch (freezeDirection)
                    {
                        case FreezeDirection.Up:

                            if (internalColliders[i].size.y > 0)
                            {
                                // This ensures we don't contract too far
                                if (internalColliders[i].size.y - (Time.deltaTime * freezeSpeed) < 0)
                                {
                                    internalColliders[i].center
                                        = new Vector3(internalColliders[i].center.x, internalColliders[i].center.y - (internalColliders[i].size.y) * 0.5f, internalColliders[i].center.z);
                                    internalColliders[i].size
                                        = new Vector3(internalColliders[i].size.x, 0, internalColliders[i].size.z);
                                }
                                else
                                {
                                    internalColliders[i].size
                                    = new Vector3(internalColliders[i].size.x, internalColliders[i].size.y - (Time.deltaTime * freezeSpeed), internalColliders[i].size.z);
                                    internalColliders[i].center
                                        = new Vector3(internalColliders[i].center.x, internalColliders[i].center.y - (Time.deltaTime * freezeSpeed * 0.5f), internalColliders[i].center.z);

                                    meltingComplete = false;
                                }
                            }
                            break;

                        case FreezeDirection.Down:

                            if (internalColliders[i].size.y > 0)
                            {
                                // This ensures we don't contract too far
                                if (internalColliders[i].size.y - (Time.deltaTime * freezeSpeed) < 0)
                                {
                                    internalColliders[i].center
                                        = new Vector3(internalColliders[i].center.x, internalColliders[i].center.y + (internalColliders[i].size.y) * 0.5f, internalColliders[i].center.z);
                                    internalColliders[i].size
                                        = new Vector3(internalColliders[i].size.x, 0, internalColliders[i].size.z);
                                }
                                else
                                {
                                    internalColliders[i].size
                                    = new Vector3(internalColliders[i].size.x, internalColliders[i].size.y - (Time.deltaTime * freezeSpeed), internalColliders[i].size.z);
                                    internalColliders[i].center
                                        = new Vector3(internalColliders[i].center.x, internalColliders[i].center.y + (Time.deltaTime * freezeSpeed * 0.5f), internalColliders[i].center.z);

                                    meltingComplete = false;
                                }
                            }
                            break;

                        case FreezeDirection.Left:

                            if (internalColliders[i].size.x > 0)
                            {
                                // This ensures we don't contract too far
                                if (internalColliders[i].size.x - (Time.deltaTime * freezeSpeed) < 0)
                                {
                                    internalColliders[i].center
                                        = new Vector3(internalColliders[i].center.x + (internalColliders[i].size.x) * 0.5f, internalColliders[i].center.y, internalColliders[i].center.z);
                                    internalColliders[i].size
                                        = new Vector3(0, internalColliders[i].size.y, internalColliders[i].size.z);
                                }
                                else
                                {
                                    internalColliders[i].size
                                    = new Vector3(internalColliders[i].size.x - (Time.deltaTime * freezeSpeed), internalColliders[i].size.y, internalColliders[i].size.z);
                                    internalColliders[i].center
                                        = new Vector3(internalColliders[i].center.x + (Time.deltaTime * freezeSpeed * 0.5f), internalColliders[i].center.y, internalColliders[i].center.z);

                                    meltingComplete = false;
                                }
                            }
                            break;

                        case FreezeDirection.Right:

                            if (internalColliders[i].size.x > 0)
                            {
                                // This ensures we don't contract too far
                                if (internalColliders[i].size.x - (Time.deltaTime * freezeSpeed) < 0)
                                {
                                    internalColliders[i].center
                                        = new Vector3(internalColliders[i].center.x - (internalColliders[i].size.x) * 0.5f, internalColliders[i].center.y, internalColliders[i].center.z);
                                    internalColliders[i].size
                                        = new Vector3(0, internalColliders[i].size.y, internalColliders[i].size.z);
                                }
                                else
                                {
                                    internalColliders[i].size
                                    = new Vector3(internalColliders[i].size.x - (Time.deltaTime * freezeSpeed), internalColliders[i].size.y, internalColliders[i].size.z);
                                    internalColliders[i].center
                                        = new Vector3(internalColliders[i].center.x - (Time.deltaTime * freezeSpeed * 0.5f), internalColliders[i].center.y, internalColliders[i].center.z);

                                    meltingComplete = false;
                                }
                            }
                            break;
                    }
                }

                // If the meltingComplete bool is still true, all colliders have contracted to a width of 0. Thus we can make them intangible.
                if (meltingComplete)
                {
                    for (int i = 0; i < internalColliders.Length; i++)
                    {
                        internalColliders[i].enabled = false;
                    }

                    if (isWaterfall)
                    {
                        gameObject.tag = "Water";
                        gameObject.layer = LayerMask.NameToLayer("Water");
                    }
                }
            }
        }
    }

}
