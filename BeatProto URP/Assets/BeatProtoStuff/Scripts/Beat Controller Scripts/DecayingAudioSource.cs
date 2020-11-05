using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach this class to an AudioSource to make it destroy itself once it's finished playing it's clip.
/// </summary>
public class DecayingAudioSource : MonoBehaviour
{
    [HideInInspector]
    /// <summary>
    /// To be assigned upon instantiation, but before initialisation.
    /// </summary>
    public AudioClip sourceClip;


    [HideInInspector]
    /// <summary>
    /// To be assigned upon instantiation, but before initialisation. Adjusts the starting volume of the decaying source.
    /// </summary>
    public float volume = -1;


    [HideInInspector]
    /// <summary>
    /// To be assigned upon instantiation, but before initialisation. Adjusts the time 
    /// the decaying source will start playing from within the given clip.
    /// </summary>
    public float startTime = 0;

    [HideInInspector]
    /// <summary>
    /// To be assigned upon instantiation, but before initialisation. If true, the source will become softer until it disappears.
    /// At volume == 0, the source will destroy itself.
    /// </summary>
    public bool fadeVolume = false;

    [HideInInspector]
    /// <summary>
    /// To be assigned upon instantiation, but before initialisation. The source's volume will fade out at this speed if fadeVolume is true.
    /// </summary>
    public float fadeOutSpeed = 0.5f;

    // The audioSource that will play the individual sound.
    private AudioSource source;


    private void Start()
    {
        source = GetComponent<AudioSource>();

        if (sourceClip != null)
        {
            // assign the decaying source an AudioClip
            source.clip = sourceClip;

            // Update the volume of our Source to the given value (if a value was given)
            if (volume > 0)
            {
                source.volume = volume;
            }

            // Update the point at which this source should start if a non-zero time was given.
            if (startTime > 0)
            {
                source.time = startTime;
            }

            // and finally start playing the source
            source.Play();
        }
        else
        {
            Debug.LogError("No sourceClip assigned to Decaying Audio Source. Source destroyed to prevent it from persisting indefinitely.",gameObject);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!source.isPlaying)
        {
            Destroy(gameObject);
        }
        else
        {
            // Source will become perpetually softer, destroying itself once its volume is 0
            if (fadeVolume)
            {
                source.volume -= fadeOutSpeed * Time.deltaTime;

                if (source.volume <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

}
