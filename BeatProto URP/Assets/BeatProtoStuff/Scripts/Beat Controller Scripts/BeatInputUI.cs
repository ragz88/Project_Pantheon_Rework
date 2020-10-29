using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatInputUI : MonoBehaviour
{
    /// <summary>
    /// The images used to display the beat blocks for a specific beat controller
    /// </summary>
    public Image[] beatBlockImages;

    /// <summary>
    /// Sprite to be show if a beat in a beat pattern is filled - ie. the OnBeat event for this beat WILL fire
    /// </summary>
    public Sprite activeBeatSprite;

    /// <summary>
    /// Sprite to be show if a beat in a beat pattern is empty - ie. the OnBeat event for this beat WON'T fire
    /// </summary>
    public Sprite inactiveBeatSprite;


    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
