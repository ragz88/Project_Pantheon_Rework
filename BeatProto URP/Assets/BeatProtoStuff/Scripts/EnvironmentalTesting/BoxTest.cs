using UnityEngine;

public class BoxTest : MonoBehaviour
{
    public void OnBeat()
    {
        if (transform.localScale.x == 1)
        {
            transform.localScale = new Vector2(2, 2);
        }
        else if (transform.localScale.x == 2)
        {
            transform.localScale = new Vector2(1, 1);
        }
    }
}
