using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPlatformCheck3D : MonoBehaviour
{

	public enum wallToCheck { right, left};

	public wallToCheck WallToCheck;
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Platform" || other.gameObject.tag == "Wall" || other.gameObject.tag == "Door")
		{
			if (WallToCheck == wallToCheck.right)
			{
				PlayerController3D.pController3D.againstWallRight = true;
			}
			else 
			{
				PlayerController3D.pController3D.againstWallLeft = true;
			}

			//PlayerController3D.pController3D.againstWall = true;
		}
	}
	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag == "Platform" || other.gameObject.tag == "Wall" || other.gameObject.tag == "Door")
		{
			if (WallToCheck == wallToCheck.right)
			{
				PlayerController3D.pController3D.againstWallRight = true;
			}
			else
			{
				PlayerController3D.pController3D.againstWallLeft = true;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Platform" || other.gameObject.tag == "Wall" || other.gameObject.tag == "Door")
		{
			if (WallToCheck == wallToCheck.right)
			{
				PlayerController3D.pController3D.againstWallRight = false;
			}
			else
			{
				PlayerController3D.pController3D.againstWallLeft = false;
			}
		}
	}
}
