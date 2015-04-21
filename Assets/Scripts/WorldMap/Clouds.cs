using UnityEngine;
using System.Collections;

public class Clouds : MonoBehaviour {
	public Transform end;
	public Transform start;

	private float moveSpeed = 0.5f;

	// Update is called once per frame
	void Update () {
		if(this.transform.position.x > end.position.x) {
			this.transform.position = start.position;
		}

		this.transform.position = new Vector3(this.transform.position.x + (moveSpeed * Time.deltaTime), 0, 0);
	}
}
