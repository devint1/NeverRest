﻿using UnityEngine;
using System.Collections;

public class Persistence : MonoBehaviour {
	public int currentLevel = 1;

	public AudioSource backGroundMusic;

	public static Persistence i;
	public  bool isTutorial;

	public TutorialStates.State currentState = TutorialStates.State.Commence;
		
	void Awake () {
		if(!i) {
			i = this;
			DontDestroyOnLoad(gameObject);

			if (backGroundMusic) {
				backGroundMusic.Play();
			}
		}
		else {
			Destroy(gameObject);
		}
	}
}
