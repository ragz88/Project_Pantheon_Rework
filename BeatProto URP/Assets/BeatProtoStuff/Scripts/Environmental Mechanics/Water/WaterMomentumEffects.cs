using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMomentumEffects : MonoBehaviour
{

    [Header("Water Settings")]

    public float speedReductionFactor = 0.5f;

    [Header("Ice Settings")]

    public float maxSpeedIncreaseFactor = 1.2f;

    public float accelerationChangeFactor = 0.75f;

    public float deccelerationChangeFactor = 0.75f;

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


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

        }
    }
}
