using UnityEngine;

public class BeatToggleHolder : MonoBehaviour
{
    public GameObject[] toggleObjects;

    private GameObject[] toggleParts;

    private void Start()
    {
        toggleParts = new GameObject[toggleObjects.Length];

        for (int i = 0; i < toggleObjects.Length; i++)
        {
            toggleParts[i] = toggleObjects[i].GetComponent<BeatToggle>().toggleObject;
        }

    }
    public void OnBeat()
    {
        foreach (GameObject togPart in toggleParts)
        {
            togPart.SetActive(!togPart.activeSelf);
        }
    }

}
