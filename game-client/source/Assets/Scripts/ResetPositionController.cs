﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPositionController : MonoBehaviour
{
	public void OnCollisionEnter(Collision collision)
	{
		GameObject player = collision.gameObject;
		
		var backToPosition = Vector3.zero;
		
		backToPosition.y = 0.5f;

		player.transform.position = backToPosition;
	}
}
