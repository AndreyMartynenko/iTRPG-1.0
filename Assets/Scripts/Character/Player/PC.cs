using UnityEngine;
using System.Collections;
using Utility;

public class PC : BaseCharacter {

	#region =Variables=
	
	private static PC _instance = null;
	private Point _position;
	private bool _isPCTurn = true;
	
	private bool initialized = false;
	
	#endregion
	
	public override void Awake() {
		base.Awake();
		
		_instance = this;
	}
	
	#region =Getters & Setters=
	
	public static PC Instance {
		get {
			if(_instance == null) {
				Debug.Log("Instancing a new PC.");
				GameObject player = Instantiate(Resources.Load("PC/Mesh/Soldier PC"), 
				                                Vector3.zero, 
				                                Quaternion.identity) as GameObject;
				player.name = "PC";
				player.tag = "Player";
				player.transform.Rotate(Vector3.up, 90);
				player.transform.position = Vector3.zero;
			}
			
			_instance.HitRange = 10; //ToDo: sacar de aqui!!!
			
			return _instance;
		}
	}
	
	public Point Position {
		get { return _position; }
		set { _position = value; }
	}
	
	public bool IsPCTurn {
		get { return _isPCTurn; }
		set { _isPCTurn = value; }
	}
	
	#endregion
	
	public void Initialize() {
		Debug.Log("PlayerCharacter: Initialize");
		
		if(!initialized)
			SetupCharacter();
	}
	
	public void SetupCharacter() { //ToDo: add setups for all patrs of character: head, torso...
		GameSettings.LoadAttributes();
		//PC.Instance.ClearModifiers();
		GameSettings.LoadVitals();
		GameSettings.LoadSkills();
		
		initialized = true;
	}
}
