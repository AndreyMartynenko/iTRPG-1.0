using UnityEngine;
using System.Collections;
using Utility;

public class Mob : BaseCharacter {
	
	private int id;
	public MobManager mobManager;
	
	public int Id {
		get { return id; }
		set { id = value; }
	}
	
	void Start () {
		Point position = Tools.GetLocalPosition(transform.position.x, transform.position.z);
		Tools.ToggleMobPosition(position, true);

		RemainingAP = 5; //ToDo: obtener en base a las props del mob
		
		mobManager = GameObject.FindGameObjectWithTag("MobManager").GetComponent<MobManager>();
		mobManager.mobPositions.Add(position);
	}
	
	void Update () {
	
	}
}
