
using UnityEngine;
using System.Collections;

public class CamControl : MonoBehaviour {
	
	public GameObject target;  // The player
	
	
	private Vector3  offset;
	
	void Start () {
		
		// Change the z value of the offset
		// to something below the sprites.
		// Otherwise you can't see anything:
		offset = new Vector3(0f, 0f, -10f);
	}
	
	void Update () {
		
	}
	
	// Let's put the movement in LateUpdate (called after Update function)
	void LateUpdate() {
		// Change camera's position to the same as the player (with the z-value of -10)
		transform.position = target.transform.position + offset;
	}
}