using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisappearingPrompt : MonoBehaviour
{
    public GameObject prompt;

    public Image promptImage;

    public SpriteRenderer promptSpriteRend;

    public KeyCode[] ClosingKeys;

    public float closeDelay = 2f;

    public float fadeSpeed = 3f;

    bool closing = false;

    float closeTimer = 0;

    // Update is called once per frame
    void Update()
    {
        if (!closing)
        {
            for (int i = 0; i < ClosingKeys.Length; i++)
            {
                if (Input.GetKeyDown(ClosingKeys[i]))
                {
                    closing = true;
                }
            }
        }
        else
        {
            if (closeTimer > closeDelay)
            {
                if (promptImage != null)
                {
                    promptImage.color = promptImage.color - new Color(0, 0, 0, fadeSpeed * Time.deltaTime);

                    if (promptImage.color.a <= 0)
                    {
                        Destroy(gameObject);
                    }
                }

                if (promptSpriteRend != null)
                {
                    promptSpriteRend.color = promptSpriteRend.color - new Color(0, 0, 0, fadeSpeed * Time.deltaTime);

                    if (promptSpriteRend.color.a <= 0)
                    {
                        Destroy(gameObject);
                    }
                }
            }
            else
            {
                closeTimer += Time.deltaTime;
            }
        }
    }
}
