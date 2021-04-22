using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(SpriteRenderer))]
public class SingController_GroundReset : MonoBehaviour
{
    [Tooltip("Image used to show how much breath we have left.")]
    /// <summary>
    /// Image used to show how much breath we have left.
    /// </summary>
    [SerializeField]
    private Image singBar;

    [Tooltip("The maximum speed at which the player will fall while singing.")]
    /// <summary>
    /// The maximum speed at which the player will fall while singing.
    /// </summary>
    [SerializeField]
    private float singFallSpeed = -1f;
    
    [Tooltip("The maximum time our player can continually sing in seconds.")]
    /// <summary>
    /// The maximum time our player can continually sing in seconds.
    /// </summary>
    [SerializeField]
    private float maxSingTime = 4;

    [Tooltip("The amount of SingTime restored per second while the player is not singing. ")]
    /// <summary>
    /// The amount of SingTime restored per second while the player is not singing. 
    /// </summary>
    [SerializeField]
    private float singTimeResetSpeed = 2;

    [Tooltip("The localScale our aura should scale up to at full size.")]
    /// <summary>
    /// The localScale our aura should scale up to at full size
    /// </summary>
    [SerializeField]
    private Vector3 finalScale;

    [Tooltip("The speed at which our aura scales up.")]
    /// <summary>
    /// The speed at which our aura scales up
    /// </summary>
    [SerializeField]
    private float auraScaleSpeed = 2f;


    [Tooltip("How many seconds before our breath bar starts refilling again.")]
    /// <summary>
    /// How many seconds before our breath bar starts refilling again.
    /// </summary>
    [SerializeField]
    private float singResetDelay = 0.6f;

    /// <summary>
    /// Used to ensure singResetDelay seconds pass before our bar starts filling up again.
    /// </summary>
    private float singResetTimer = 0;

    /// <summary>
    /// The localScale our aura should scale down to at minimum size
    /// </summary>
    private Vector3 initialScale;

    /// <summary>
    /// The Color our aura starts off with.
    /// </summary>
    private Color initialAuraColour;

    /// <summary>
    /// Reduced as the player sings, and increases as the character takes a breath. Character will only take breath when on ground.
    /// </summary>
    private float currentSingTime;

    /// <summary>
    /// The character controller for the player character that will be singing. This class should be a child of said character object.
    /// </summary>
    private PlayerCharacterController characterController;

    /// <summary>
    /// Collider that represents sing-aura's bounds.
    /// </summary>
    private SphereCollider singCollider;

    /// <summary>
    /// Renders the visual representation of our sing ability's reach.
    /// </summary>
    private SpriteRenderer singAuraSpriteRenderer;

    
    private void OnEnable()
    {
        // We want to add the sing functionality of this class to the character controller.
        characterController = GetComponentInParent<PlayerCharacterController>();
        characterController.handleSing += HandleSing;

        characterController.SingFallVelocity = singFallSpeed;
    }

    private void OnDisable()
    {
        // We want to remove the sing functionality of this class to the character controller.
       characterController.handleSing -= HandleSing;
    }

    private void Start()
    {
        currentSingTime = maxSingTime;

        singCollider = GetComponent<SphereCollider>();
        singAuraSpriteRenderer = GetComponent<SpriteRenderer>();
        initialScale = transform.localScale;
        initialAuraColour = singAuraSpriteRenderer.color;
    }


    bool HandleSing(bool grounded)
    {
        singBar.fillAmount = (currentSingTime/maxSingTime);

        if (Input.GetButton("Sing"))
        {
            // Player still has breath to sing
            if (currentSingTime > 0)
            {
                singResetTimer = 0;
                singAuraSpriteRenderer.enabled = true;
                singCollider.enabled = true;
                singAuraSpriteRenderer.color = initialAuraColour;

                // Scale up sing aura
                if (transform.localScale.x < finalScale.x)
                {
                    transform.localScale = Vector3.Lerp(transform.localScale, finalScale, auraScaleSpeed * Time.deltaTime);
                }
                currentSingTime -= Time.deltaTime;

                return true;
            }
            else
            {
                currentSingTime = 0;

                // Fade out sing aura
                if (singAuraSpriteRenderer.color.a > 0)
                {
                    singAuraSpriteRenderer.color = singAuraSpriteRenderer.color - new Color(0, 0, 0, auraScaleSpeed * Time.deltaTime);
                }
                else
                {
                    transform.localScale = initialScale;
                    singAuraSpriteRenderer.enabled = false;
                    singCollider.enabled = false;
                }

                return false;
            }
        }
        else
        {
            // Fade out sing aura
            if (singAuraSpriteRenderer.color.a > 0)
            {
                singAuraSpriteRenderer.color = singAuraSpriteRenderer.color - new Color(0, 0, 0, auraScaleSpeed * Time.deltaTime);
            }
            else
            {
                transform.localScale = initialScale;
                singAuraSpriteRenderer.enabled = false;
                singCollider.enabled = false;
            }

            // Singing breath only resets once the ground is touched
            if (grounded)
            {
                if (singResetTimer >= singResetDelay)
                {
                    if (currentSingTime < maxSingTime)
                    {
                        currentSingTime += singTimeResetSpeed * Time.deltaTime;
                    }
                }
                else
                {
                    singResetTimer += Time.deltaTime;
                }
                
            }


            return false;
        }
        
    }


    /// <summary>
    /// Reduces currentSingTime to 0. SHould be used as player passes through waterfalls.
    /// </summary>
    public void ResetCurrentSingTime()
    {
        currentSingTime = 0;
        //singAuraSpriteRenderer.color = singAuraSpriteRenderer.color - new Color(0, 0, 0, 1);

        // BUG - PRESSING SING JUST AFTER WATERFALL SSTILL OPENS LOCK?
    }
}
