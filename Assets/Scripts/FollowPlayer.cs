using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

	public Transform player;

	public float zOffset;
	public float yOffset;

	private void LateUpdate()
	{
		Vector3 pos = player.position;
		pos.y += yOffset;
		pos.z += zOffset;
		transform.position = pos;
	}
}
