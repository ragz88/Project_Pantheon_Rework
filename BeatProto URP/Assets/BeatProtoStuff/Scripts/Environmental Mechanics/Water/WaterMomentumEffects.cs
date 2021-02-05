using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMomentumEffects : MonoBehaviour
{

    [Header("Water Settings")]
    
    [Tooltip("The percent of the player's normal speed that they will travel in water.")]
    /// <summary>
    /// The percent of the player's normal speed that they will travel in water.
    /// </summary>
    public float relativeWaterSpeed = 0.4f;

    [Header("Ice Settings")]

    [Tooltip("The percent of the player's normal speed that they will travel on ice.")]
    /// <summary>
    /// The percent of the player's normal speed that they will travel on ice.
    /// </summary>
    public float relativeIceSpeed = 1.2f;

    public float iceAcceleration = 15f;

    public float iceDecceleration = 12f;

    private WaterController waterController;

    private 

    // Start is called before the first frame update
    void Start()
    {
        waterController = GetComponentInParent<WaterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && waterController.waterState == WaterState.Liquid)
        {
            PlayerController3D.pController3D.SetMoveSpeedModifier(relativeWaterSpeed);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController3D.pController3D.SetMoveSpeedModifier(1);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && waterController.waterState == WaterState.Frozen)
        {
            PlayerController3D.pController3D.SetMoveSpeedModifier(relativeIceSpeed);
            PlayerController3D.pController3D.MoveWithAcceleration(iceAcceleration, iceDecceleration);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && waterController.waterState == WaterState.Frozen)
        {
            PlayerController3D.pController3D.SetMoveSpeedModifier(1);
            PlayerController3D.pController3D.MoveWithoutAcceleration();
        }
    }
}
