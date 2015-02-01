using UnityEngine;
using System.Collections;

public class HealthBarCript : MonoBehaviour {

	public Transform target;
	
	void Update ()
	{
		Vector3 wantedPos = Camera.main.WorldToViewportPoint (target.position);
		transform.position = wantedPos;
	}
}
