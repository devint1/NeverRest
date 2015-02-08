using UnityEngine;
using System.Collections;

public class HealthBarCript : MonoBehaviour {

	public GameControl gameControl;
	public Transform target;
	
	void Update ()
	{
		if( gameControl.CheckIfPaused() ){
			return;
		}
		Vector3 wantedPos = Camera.main.WorldToViewportPoint (target.position);
		transform.position = wantedPos;
	}
}
