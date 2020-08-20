using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
	public Vector3 offset;
	private Transform target;
	[Range (0, 1)] public float lerpValue;

	
	void Start()
	{
		target = GameObject.Find("Player").transform;
	}

	void LateUpdate()
	{
	    transform.position = Vector3.Lerp(transform.position, target.position + offset, lerpValue);
	}
}
