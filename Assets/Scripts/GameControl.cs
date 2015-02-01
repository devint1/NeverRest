using UnityEngine;
using System.Collections;

public class GameControl : MonoBehaviour {
	public ArrayList selected;
	public ArrayList whiteBloodCells;
	public GameObject white_blood_spawn_point;
	public GameObject white_Blood_Cell_Prefab;
	public Texture2D food_Bar_Full;
	public Texture2D food_Bar_Empty;


	int white_blood_production = 0;
	int mousePressStart = -1;
	Vector3 mousePositionStart;
	float time_of_last_spawn;
	float food_Level = 1f;

	void Start() {
		time_of_last_spawn = Time.time;
		selected = new ArrayList ();
		whiteBloodCells = new ArrayList ();
	}

	void OnGUI() {
		white_blood_production = (int)GUI.HorizontalSlider(new Rect(25, 50, 100, 30), white_blood_production, 0.0F, 10.0F);
		GUI.TextArea (new Rect(130, 45, 20, 20), "" + white_blood_production);

		GUI.BeginGroup (new Rect (20, 10, 125, 30));
		GUI.Box (new Rect (0,0, 125, 30),food_Bar_Empty);
		// draw the filled-in part:
		{
			GUI.BeginGroup (new Rect (0, 0, 125 * food_Level, 30));
			GUI.Box (new Rect (0,0, 125, 30),food_Bar_Full);
			GUI.EndGroup ();
		}
		GUI.EndGroup ();

		if (Input.GetMouseButton (0) == true) {
			if( mousePressStart == -1 ){
				mousePressStart = Time.frameCount;
				mousePositionStart = Event.current.mousePosition;
			}
			else if(Time.frameCount - mousePressStart > 4){
				//TODO - only need to make this texture once, rather than every time
				Texture2D text = new Texture2D(1,1);
				Color col = Color.green;
				col.a = .15f;
				text.SetPixel(1, 1, col);
				text.Apply();
				Rect box = new Rect(mousePositionStart.x, mousePositionStart.y,
				                    Event.current.mousePosition.x - mousePositionStart.x, 
				                    Event.current.mousePosition.y - mousePositionStart.y);
				GUI.DrawTexture(box, text);
			}
		}
		else{
			if(mousePressStart != -1) {
				Rect box = new Rect(mousePositionStart.x, mousePositionStart.y,
				                    Event.current.mousePosition.x - mousePositionStart.x, 
				                    Event.current.mousePosition.y - mousePositionStart.y);
				foreach(WhiteBloodCell cell in whiteBloodCells){
					Vector2 pos = Camera.main.WorldToScreenPoint(cell.transform.position);
					pos.y = Camera.main.pixelHeight - pos.y;
					if(box.Contains(pos, true)){
						cell.Select();
					}

				}
			}
			mousePressStart = -1;
		}
	}

	void Update() {
		if (white_blood_production > 0 && (Time.time - time_of_last_spawn) > 30 / white_blood_production) {
			Spawn_White_Blood_Cell ();
			food_Level -= 0.05f;
		}

		if (food_Level <= 0f) {
			Application.LoadLevel ("MenuScene"); 
		}
	}

	void Spawn_White_Blood_Cell() {
		GameObject new_White = (GameObject)Instantiate (white_Blood_Cell_Prefab, white_blood_spawn_point.transform.position, this.transform.rotation);
		new_White.GetComponent<WhiteBloodCell> ().current_Block = white_blood_spawn_point;
		new_White.GetComponent<WhiteBloodCell> ().game_Control = this;
		whiteBloodCells.Add (new_White.GetComponent<WhiteBloodCell>());
		time_of_last_spawn = Time.time;
	}
}
