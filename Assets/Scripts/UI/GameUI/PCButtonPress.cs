using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PCButtonPress : MonoBehaviour, IPointerClickHandler {
	public GameUI gameui;
	
	public PCButtonPress ()
	{
		/*this.onClick.AddListener( () => {
			Debug.Log("PC Button press");
			gameui.incrPCQueue();
		});*/
	}
	
	public void OnPointerClick(PointerEventData eventData) {
		if (eventData.button == PointerEventData.InputButton.Left) {
			gameui.incrPCQueue ();
		} else if (eventData.button == PointerEventData.InputButton.Right) {
			gameui.decrPCQueue ();
		}
	}
}


