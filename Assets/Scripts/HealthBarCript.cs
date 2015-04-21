using UnityEngine;
using System.Collections;

public class HealthBarCript : MonoBehaviour {

	public GameControl gameControl;
	public Sprite phase0;
	public Sprite phase1;
	public Sprite phase2;
	public Sprite phase3;
	public Sprite phase4;
	public Sprite phase5;
	public Sprite phase6;
	public SpriteRenderer spriteRend;

	int deadLimbs = 0;

	void Update ()
	{
		if( gameControl.IsPaused() ){
			return;
		}

		if (gameControl.deadBlocks != deadLimbs) {
			deadLimbs = gameControl.deadBlocks;
			switch(deadLimbs) {
			case 0:
				spriteRend.sprite = phase0;
				break;
			case 1:
				spriteRend.sprite = phase1;
				break;
			case 2:
				spriteRend.sprite = phase2;
				break;
			case 3:
				spriteRend.sprite = phase3;
				break;
			case 4:
				spriteRend.sprite = phase4;
				break;
			case 5:
				spriteRend.sprite = phase5;
				break;
			default:
				spriteRend.sprite = phase6;
				break;
			}
		}
		//Vector3 wantedPos = Camera.main.WorldToViewportPoint (target.position);
		//transform.position = wantedPos;
	}
}
