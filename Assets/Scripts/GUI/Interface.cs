using UnityEngine;
using System.Collections;
using Utility;

public class Interface : MonoBehaviour {
	
	public GUISkin skin;
	
	void Start() {
		PC.Instance.Initialize();
	}
	
	void Update() {
		
	}
	
	void OnGUI() {
		GUI.skin = skin;
		
		//test
		if(GUI.Button(new Rect(10, 10, 75, 25), "Test")) {
			Point pos = Tools.GetLocalPosition(PC.Instance.transform.position.x, PC.Instance.transform.position.z);
			Debug.Log("globalPosition: [" + PC.Instance.transform.position.x + "," + PC.Instance.transform.position.z + "]. localPosition: [" + pos.x + "," + pos.y + "]");
		}
		
		if(GUI.Button(new Rect(Screen.width - 70, 10, 60, 80), "", "Running")) {
			Messenger<MovementType>.Broadcast("ToggleMovementType", MovementType.Run);
		}
		
		if(GUI.Button(new Rect(Screen.width - 140, 10, 60, 80), "", "Standing")) {
			Messenger<MovementType>.Broadcast("ToggleMovementType", MovementType.Walk);
		}

		if(GUI.Button(new Rect(Screen.width - 210, 10, 60, 80), "", "Crouching")) {
			Messenger<MovementType>.Broadcast("ToggleMovementType", MovementType.Crouch);
		}

		if(GUI.Button(new Rect(Screen.width - 60, Screen.height - 60, 50, 50), "", "EndTurn")) {
			Messenger.Broadcast("End PC turn");
		}
	}
}
