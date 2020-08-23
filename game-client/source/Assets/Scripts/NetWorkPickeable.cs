using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetWorkPickeable : MonoBehaviour
{
	public string id;


	public void OnCollisionEnter(Collision collision)
	{
		NetworkEntity player = collision.gameObject.GetComponent<NetworkEntity>();

		if (player)
		{
			Network.Pick(id, player.id);
		}
	}
}
