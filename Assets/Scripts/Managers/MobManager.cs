using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utility;

[AddComponentMenu("iTRPG/Managers/Mob generator")]

public class MobManager : MonoBehaviour {
	
	#region =Variables=
	
	private enum State {
		Idle,
		Init,
		Setup,
		Spawn
	}

	public GameObject[] mobs;
	public GameObject[] spawnPoints;
	public List<Point> mobPositions = new List<Point>();
	
	private State state;
	
	#endregion
	
	void OnEnable() {
		Messenger.AddListener("Start mob turn", StartMobTurn);
		Messenger<int>.AddListener("End mob turn", EndMobTurn);
	}
	
	void OnDisable() {
		Messenger.RemoveListener("Start mob turn", StartMobTurn);
		Messenger<int>.RemoveListener("End mob turn", EndMobTurn);
	}
	
	void Awake() {
		state = State.Init;
	}
	
	IEnumerator Start () {
		while(state != State.Idle) {
			switch(state) {
			case State.Init:
				Init();
				break;
			case State.Setup:
				Setup();
				break;
			case State.Spawn:
				Spawn();
				break;
			}
			
			yield return 0;
		}
	}
	
	private void Init() {
		//Debug.Log("**Initialize mob**");
		
		if(mobs.Length == 0 || spawnPoints.Length == 0) {
			Debug.LogError("There aren't any mob or spawn point assigned.");
			
			return;
		}
		
		state = State.Setup;
	}

	private void Setup() {
		//Debug.Log("**Setup mob**");
		
		state = State.Spawn;
	}

	private void Spawn() {
		//Debug.Log("**Spawn mob**");
		
		GameObject[] availableSpawnPoints = GetAvailableSpawnPoints();
		
		for(int cnt = 0; cnt < availableSpawnPoints.Length; cnt++) {
			GameObject mob = Instantiate(mobs[Random.Range(0, mobs.Length)], //ToDo: revisar el random
			                             availableSpawnPoints[cnt].transform.position, 
			                             Quaternion.identity) as GameObject;
			
			mob.transform.parent = availableSpawnPoints[cnt].transform;
			mob.GetComponent<Mob>().Id = cnt + 1;
			
			availableSpawnPoints[cnt].GetComponent<SpawnPoint>().isAvailable = false;
		}
		
		state = State.Idle;
	}
	
	private GameObject[] GetAvailableSpawnPoints() {
		List<GameObject> availableSpawnPoints = new List<GameObject>();

		foreach(GameObject spawnPoint in spawnPoints) {
			if(spawnPoint.GetComponent<SpawnPoint>().isAvailable)
				availableSpawnPoints.Add(spawnPoint);
		}
		
		return availableSpawnPoints.ToArray();
	}
	
	private void StartMobTurn() {
		Messenger<int>.Broadcast("Exec mob turn", 1);
	}

	private void EndMobTurn(int id) {
		id++;
		
		if(id <= spawnPoints.Length)
			Messenger<int>.Broadcast("Exec mob turn", id);
		else
			Messenger.Broadcast("Start PC turn");
	}
}
