using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetWorkPickeable : MonoBehaviour
{
	public string id;


	public void OnCollisionEnter(Collision collision)
	{
		NetworkEntity player = collision.gameObject.GetComponent<NetworkEntity>();

		if (player && !player.isBoss())
		{
			Network.Pick(id, player.id);
		}
	}

	public void FixedUpdate()
	{
		transform.Rotate(0, Time.deltaTime * 40f, 0, Space.Self);
	}
}
