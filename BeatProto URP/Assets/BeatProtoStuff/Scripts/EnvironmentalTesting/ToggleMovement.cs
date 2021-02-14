using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMovement : BeatToggle
{

	public Vector3 moveDirection;
	private Vector3 targetPos;

	private bool directionSwitch = true;

	private bool moving = false;

	public override void BeatEvent()
	{
		directionSwitch = !directionSwitch;
		moving = true;

		if (!directionSwitch)
			targetPos = transform.position + moveDirection;
		else if (directionSwitch)
		{
			targetPos = transform.position - moveDirection;
		}
	}


	private void Update()
	{
		
		if (moving) 
		{
			toggleObject.transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.005f);
			if (transform.position == targetPos) 
			{
				moving = false;
			}
		}
	}
}
