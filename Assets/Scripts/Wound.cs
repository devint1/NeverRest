﻿using UnityEngine;
using System.Collections;

public class Wound : MonoBehaviour {
	public float health = 1f;
	public Block block;
	bool closed = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!closed) {
			renderer.material.color = new Color(1f, 1f, 1f, health);
		}

		if(!closed && health <= 0) {
			block.wounds.Remove(this);
			closed = true;
			Animator animator = GetComponent<Animator>();

			// Put in background
			SpriteRenderer sprite = GetComponent<SpriteRenderer>();
			sprite.sortingOrder = 3;

			// Fade to closed animation
			animator.CrossFade("Closed", 0f);
			renderer.material.color = new Color(1f, 1f, 1f, 0f);
			StartCoroutine(Fade (1f, 0.5f));
		}
	}

	IEnumerator Fade(float value, float time) {
		float alpha = renderer.material.color.a;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / time) {
			Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, value, t));
			renderer.material.color = newColor;
			yield return null;
		}
	}
}
