using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardSingController : MonoBehaviour
{
    /// <summary>
    /// The maximum time our player can continually sing in seconds.
    /// </summary>
    [SerializeField]
    private float maxSingTime = 4;

    /// <summary>
    /// The amount of SingTime restored per second while the player is not singing. 
    /// </summary>
    [SerializeField]
    private float SingTimeResetSpeed = 1;

    /// <summary>
    /// The character controller for the player character that will be singing. This class should be a child of said character object.
    /// </summary>
    private PlayerCharacterController characterController;

    
    private void OnEnable()
    {
        // We want to add the sing functionality of this class to the character controller.
        characterController = GetComponentInParent<PlayerCharacterController>();
        
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
