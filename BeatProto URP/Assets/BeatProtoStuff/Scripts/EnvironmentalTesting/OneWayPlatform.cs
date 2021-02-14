using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    public GameObject platform;
    private GameObject player;



    private void Start()
    {
        if (PlayerController3D.pController3D.player != null) 
        {
            player = PlayerController3D.pController3D.player;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            Physics.IgnoreCollision(platform.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Physics.IgnoreCollision(platform.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        }

    }
}
