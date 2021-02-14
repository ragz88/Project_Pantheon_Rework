using UnityEngine;

public class BeatToggle : MonoBehaviour
{

    public GameObject toggleObject;

    /* public void OnBeat() 
     {
         toggleObject.SetActive(!toggleObject.activeSelf);
     }*/

    public virtual void BeatEvent(){}
}
