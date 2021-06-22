using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Takes the sprite of the character's image component (updated through the animator), and applies it as the character's
/// Albedo map as well. This way, we can force the sprite to interact with 3D lighting.
/// </summary>
public class ImageToTextureConverter : MonoBehaviour
{
    Image characterImage;
    Texture tempTexture;
    
    // Start is called before the first frame update
    void Start()
    {
        characterImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        tempTexture = characterImage.sprite.texture;
        characterImage.material.SetTexture("_BaseMap", tempTexture);
    }
}
