using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utility;

[RequireComponent(typeof(Movement))]

public class AI : MonoBehaviour {
	
	#region =Variables and enums=
	
	private enum State {
		Idle,
		Init,
		Setup,
		Search,
		Decide,
		Approach,
		Attack,
		Retreat
	}
	
	public float meleeRange = 18f;
	//public float rangedRange = 18f;
	public float perceptionRadius = 15f;
	
	private Transform _transform;
	private Transform spawnPoint;
	private Transform target;
	private Mob mob;
	private State state;
	public bool canAttack;
	
	#endregion
	
	void OnEnable() {
		Messenger<int>.AddListener("Exec mob turn", ExecMobTurn);
		Messenger<int>.AddListener("Mob in destination", MobInDestination);
	}
	
	void OnDisable() {
		Messenger<int>.RemoveListener("Exec mob turn", ExecMobTurn);
		Messenger<int>.RemoveListener("Mob in destination", MobInDestination);
	}
	
	void Awake() {
		mob = gameObject.GetComponent<Mob>();
	}
	
	void Start() {
		//Debug.Log("+Start mob AI+");
		
		_transform = transform;
		//state = AI.State.Init;
		
		//StartCoroutine("AILogic");
	}
	
	private IEnumerator AILogic() {
		
		yield return 0;
	}
	
	private void ExecMobTurn(int id) {
		if(id == mob.Id) { //To restrict the execution of this code only to the current mob
			Point start = Tools.GetLocalPosition(_transform.position.x, _transform.position.z);
			Point end = new Point(PC.Instance.Position.x, PC.Instance.Position.y);
			List<Vector3> pathToTarget = new List<Vector3>();
			
			pathToTarget = Tools.FindPath(start, end); //ToDo: ver como resolver q no haya camino
			
			//Debug.Log("mob.RemainingAP: " + mob.RemainingAP + ". pathToTarget.Count: " + pathToTarget.Count);
			if(pathToTarget.Count > mob.RemainingAP)
				pathToTarget.RemoveRange(mob.RemainingAP, pathToTarget.Count - mob.RemainingAP);
			pathToTarget.RemoveAt(pathToTarget.Count - 1); //To not transgress hex where PC is
			
			Tools.ToggleMobPosition(mob.mobManager.mobPositions[id - 1], false);
			mob.mobManager.mobPositions[id - 1] = new Point((int)pathToTarget[pathToTarget.Count - 1].x, (int)pathToTarget[pathToTarget.Count - 1].y);
			Tools.ToggleMobPosition(mob.mobManager.mobPositions[id - 1], true);
			
			for(int cnt = 0; cnt < pathToTarget.Count; cnt++)
				pathToTarget[cnt] = Tools.GetGlobalPosition((int)pathToTarget[cnt].x, (int)pathToTarget[cnt].y);
			
			Messenger<List<Vector3>, int>.Broadcast("MoveMob", pathToTarget, id);
		}
	}
	
	private void MobInDestination(int id) {
		if(id == mob.Id) {
			//Debug.LogWarning("MobInDestination: " + id);
			Messenger<int>.Broadcast("End mob turn", id);
		}
	}
	
	#region =Types of attack=
	
	private void MeeleAttack() {
		Debug.Log("**Meele attack**");
		
		SendMessage("PlayMeeleAttack");
		
		if(true) { //ToDo: logica de acierto
			Debug.Log("Hit");
		}
		//else
	}
	
	private void RangedAttack() {
		
	}

	private void PsyAttack() {
		
	}
	
	#endregion
}
