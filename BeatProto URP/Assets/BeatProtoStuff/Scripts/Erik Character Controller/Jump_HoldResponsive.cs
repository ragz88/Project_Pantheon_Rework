﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump_HoldResponsive : MonoBehaviour
{
    [Tooltip("The jump's height in response to a tap - measured as multiples of the player's height.")]
    /// <summary>
    /// The jump's height in response to a tap - measured as multiples of the player's height.
    /// </summary>
    [SerializeField]
    private float minJumpHeight = 1.5f;

    [Tooltip("The jump's height in response to a full hold - measured as multiples of the player's height.")]
    /// <summary>
    /// The jump's height in response to a full hold - measured as multiples of the player's height.
    /// </summary>
    [SerializeField]
    private float maxJumpHeight = 3.5f;

    /// <summary>
    /// The amount of time the shortest possible jump should last for on flat ground.
    /// </summary>
    [SerializeField]
    private float minJumpTime = 1.5f;

    /// <summary>
    /// The maximum speed the player can fall at
    /// </summary>
    [SerializeField]
    private float maxFallVelocity = -2f;


    /// <summary>
    /// The number of g's experienced while jumping upwards. Gravity will reset once you begin falling.
    /// </summary>
    [SerializeField]
    private float gravityAugment = 0.4f;

    /// <summary>
    /// Calculated based on the minJumpTime, jumpVelocity, minJumpHeight and maxJumpHeight.
    /// Represents the maximum amount of time the game will increase the Y velocity in response to a held jump button.
    /// </summary>
    private float maxHoldTime = 0;

    /// <summary>
    /// The speed at which the player rises while successfully holding Jump.
    /// </summary>
    private float jumpVelocity = 0;

    /// <summary>
    /// The amount of time the player's been holding Jump for
    /// </summary>
    private float currentHoldTime = 0;

    /// <summary>
    /// Monitors if the player has released the jump button - and doesn't allow them to jump again until they've touched ground.
    /// </summary>
    private bool lastJumpReleased = true;

    /// <summary>
    /// The character controller for the player character that will be singing. This class should be a child of said character object.
    /// </summary>
    private PlayerCharacterController characterController;

    /// <summary>
    /// The rigidbody of our Player Character.
    /// </summary>
    private Rigidbody playerBody;

    /// <summary>
    /// The Capsule Collider component of our character
    /// </summary>
    private CapsuleCollider characterCollider;

    /// <summary>
    /// The standard value of Unity gravity
    /// </summary>
    private const float gravity = -9.8f;

    private void OnEnable()
    {
        // We want to add the jump functionality of this class to the character controller.
        characterController = GetComponentInParent<PlayerCharacterController>();
        characterController.calculateJumpVelocity += CalculateJumpVelocity;

        playerBody = characterController.gameObject.GetComponent<Rigidbody>();

        characterCollider = characterController.gameObject.GetComponent<CapsuleCollider>();

        CalculateInitialJumpVelocity();

        CalculateMaxHoldTime();

        characterController.MaxFallVelocity = maxFallVelocity;
    }

    private void OnDisable()
    {
        // We want to remove the jump functionality of this class to the character controller.
        characterController.calculateJumpVelocity -= CalculateJumpVelocity;
    }

    
    Vector2 CalculateJumpVelocity(bool grounded)
    {
        // Player started on ground, and intentionally pressed jump again when on the ground
        if (grounded && Input.GetButtonDown("Jump"))
        {
            currentHoldTime = 0;
            lastJumpReleased = false;
            Physics.gravity = new Vector3(0,gravity * gravityAugment,0);
        }

        if (Input.GetButtonUp("Jump"))
        {
            lastJumpReleased = true;
        }
        
        // Player still has some airtime left, and they're still holding Jump down
        if (currentHoldTime < maxHoldTime && !lastJumpReleased)
        {
            currentHoldTime += Time.deltaTime;
            return new Vector2(0, jumpVelocity);
        }
        // Player has exceeded max hold time - so we no longer increase the Y Velocity
        else
        {
            Physics.gravity = new Vector3(0, gravity, 0); 
            return new Vector2(0, playerBody.velocity.y);
        }

        
    }


    /// <summary>
    /// Calculates the Y velocity the jump should use during a hold using the equations of motion.
    /// </summary>
    private void CalculateInitialJumpVelocity()
    {
        // EQ:         s = ut + 0.5 * a * t^2
        // Therefore:  u = (s - 0.5*a*t^2)/t

        jumpVelocity = ((minJumpHeight * characterCollider.bounds.size.y) - (0.5f * Physics.gravity.y * Mathf.Pow( (minJumpTime/2), 2)) / (minJumpTime/2));
    }


    /// <summary>
    /// Calculates the maximum amount of time the Y velocity must be increased in response to a held Jump Button
    /// in order to finish at maxJumpHeight. <br></br>
    /// Calculation based on the minJumpTime, jumpVelocity, minJumpHeight and maxJumpHeight.
    /// </summary>
    private void CalculateMaxHoldTime()
    {
        // We can no longer use EQ of Motion - as the jump is designed to maintain a constant velocity while the 
        // player holds jump. So instead, we use the originally calculated values in CalculateInitialJumpVelocity
        // to do distance, speed and time calculations - in conjunction with the original parabolic form from CalculateInitialJumpVelocity.

        // We only need to maintain the jumpVelocity for this portion of the jump - the natural parabolic nature resulting from downward acceleration
        // will carry it the rest of the way. Imagine it as starting our minJump calculation at a higher point.
        float holdDistance = (maxJumpHeight - minJumpHeight) * characterCollider.bounds.size.y;

        // Now, we just need to know how long it takes to travel that distance at the jumpVelocity - and we have our max hold time!
        // We can find it with EQ t = d/s
        maxHoldTime = holdDistance / jumpVelocity;
    }
}
