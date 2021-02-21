using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMovement : BeatToggle
{
	public Transform positionA, positionB;

	private Vector3 pos1, pos2;
	private Vector3 startPos, targetPos;

	private bool directionSwitch = false;

	private bool moving = false;

	float time;
	private float beatTime;

	public override void BeatEvent()
	{
		directionSwitch = !directionSwitch;
		moving = true;

		if (!directionSwitch)
		{
			startPos = transform.position;
			targetPos = pos1;
			time = 0f;
		}
		else if (directionSwitch)
		{
			startPos = transform.position;
			targetPos = pos2;
			time = 0f;
		}
	}

	private void Start()
	{
		pos1 = positionA.transform.position;
		pos2 = positionB.transform.position;
		transform.position = pos1;
	}

	private void Update()
	{
		beatTime = 60 / BeatTimingManager.btmInstance.currentSong.BPM;

		if (moving) 
		{
			time += Time.deltaTime / beatTime;
			toggleObject.transform.position = Vector3.Lerp(startPos, targetPos, time);

			if (transform.position == targetPos) 
			{
				moving = false;
			}
		}


		/*if (moving) 
		{
			toggleObject.transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.005f);
			if (transform.position == targetPos) 
			{
				moving = false;
			}
		}*/
	}
}
