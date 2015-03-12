using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WBCButtonPress : MonoBehaviour, IPointerClickHandler {
	public GameUI gameui;

	public WBCButtonPress () {
		/*this.onClick.AddListener( () => {
			Debug.Log("WBC Button Press");
			gameui.incrWBCQueue();
		});*/
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (eventData.button == PointerEventData.InputButton.Left) {
			gameui.incrWBCQueue ();
		} else if (eventData.button == PointerEventData.InputButton.Right) {
			gameui.decrWBCQueue ();
		}
	}
}

