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
    
    // The audioSource that will play the individual sound.
    private AudioSource source;


    private void Start()
    {
        source = GetComponent<AudioSource>();

        if (sourceClip != null)
        {
            source.clip = sourceClip;
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
    }

}
