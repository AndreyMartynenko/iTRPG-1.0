using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utility;

public class Movement : MonoBehaviour {
	
	#region =Variables=
	
	public bool isPlayerCharacter = false;
	public float walkSpeed = 1;
	public float runSpeed = 2;
	public float crouchSpeed = 0.7f;
	public float rotationSpeed = 0.33f; //crouch: x0.5, walk: x1, run: x2
	
	private List<Vector3> path;
	private Transform _transform;
	
	private Animation _animation;
	private string animationName = "";
	
	private int index = 1;
	private MovementType movementType;
	private float movementSpeed = 0;
	private float rotationTime = 0;
	
	#endregion
	
	void OnEnable() {
		Messenger<MovementType>.AddListener("ToggleMovementType", ToggleMovementType);
		Messenger<List<Vector3>>.AddListener("MovePC", MovePCManager);
		Messenger<List<Vector3>, int>.AddListener("MoveMob", MoveMobManager);
	}
	
	void OnDisable() {
		Messenger<MovementType>.RemoveListener("ToggleMovementType", ToggleMovementType);
		Messenger<List<Vector3>>.RemoveListener("MovePC", MovePCManager);
		Messenger<List<Vector3>, int>.RemoveListener("MoveMob", MoveMobManager);
	}
	
	void Start() {
		if(isPlayerCharacter) {
			_transform = PC.Instance.transform;
			_animation = PC.Instance.GetComponent<Animation>();
		}
		else {
			_transform = transform;
			_animation = GetComponent<Animation>();
		}
		
		movementType = MovementType.Walk;
		
		_animation.wrapMode = WrapMode.Loop;
		_animation.Play("idle");
	}
	
	void Update() {
		
	}
	
	private void ToggleMovementType(MovementType moveType) {
		if(isPlayerCharacter) {
			movementType = moveType;
			
			switch(movementType) {
			case MovementType.Walk:
				_animation.CrossFade("stand"); //idle ???
				break;
				
			case MovementType.Run:
				_animation.CrossFade("stand");
				break;
				
			case MovementType.Crouch:
				_animation.CrossFade("crouch"); //crouchIdle ???
				break;
			}
		}
	}
	
	private void MovePCManager(List<Vector3> newPath) {
		if(isPlayerCharacter) {
			switch(movementType) {
			case MovementType.Walk:
				animationName = "walk";
				movementSpeed = 1 / walkSpeed;
				//rotationSpeed = rotationSpeed;
				break;
				
			case MovementType.Run:
				animationName = "run";
				movementSpeed = 1 / runSpeed;
				rotationSpeed = rotationSpeed / 2;
				break;
				
			case MovementType.Crouch:
				animationName = "crouchWalk";
				movementSpeed = 1 / crouchSpeed;
				rotationSpeed = rotationSpeed * 2;
				break;
			}
			
			path = newPath;
			
			_animation.CrossFade(animationName);
			
			StartCoroutine(MoveCharacter(0));
		}
	}
	
	private void MoveMobManager(List<Vector3> newPath, int id) {
		if(!isPlayerCharacter) {
			if(_transform.GetComponent<Mob>().Id == id) {
				switch(movementType) {
				case MovementType.Walk:
					animationName = "walk";
					movementSpeed = 1 / walkSpeed;
					//rotationSpeed = rotationSpeed;
					break;
					
				case MovementType.Run:
					animationName = "run";
					movementSpeed = 1 / runSpeed;
					rotationSpeed = rotationSpeed / 2;
					break;
					
				case MovementType.Crouch:
					animationName = "crouchWalk";
					movementSpeed = 1 / crouchSpeed;
					rotationSpeed = rotationSpeed * 2;
					break;
				}
				
				path = newPath;
				
				_animation.CrossFade(animationName);
				
				StartCoroutine(MoveCharacter(id));
			}
		}
	}
	
	private IEnumerator MoveCharacter(int id) {
		Vector3 currentHex = Vector3.zero;
		Vector3 nextHex = Vector3.zero;
		float angleToRotate = 0;
		
		if(path.Count > 1 && index < path.Count) {
			currentHex = path[index - 1];
			nextHex = path[index];
			
			angleToRotate = Vector3.Angle(nextHex - currentHex, _transform.forward);
			
			switch((int)Mathf.Round(angleToRotate)) {
			case 0:
				rotationTime = 0;
				break;
				
			case 60:
				rotationTime = rotationSpeed;
				break;
				
			case 120:
				rotationTime = rotationSpeed * 2;
				break;
				
			case 180:
				rotationTime = rotationSpeed * 3;
				break;
				
			default:
				rotationTime = rotationSpeed;
				break;
			}
			
			if(angleToRotate > 10)
				yield return StartCoroutine(Rotate(_transform, _transform.rotation, (nextHex - currentHex), rotationTime));
		}
		
		if(index == path.Count) {
			index = 1;
			
			switch(movementType) {
			case MovementType.Walk:
				_animation.CrossFade("stand"); //idle ???
				break;
				
			case MovementType.Run:
				_animation.CrossFade("stand");
				break;
				
			case MovementType.Crouch:
				_animation.CrossFade("crouch"); //crouchIdle ???
				break;
			}
			
			if(isPlayerCharacter)
				Messenger.Broadcast("PC in destination");
			else
				Messenger<int>.Broadcast("Mob in destination", id);
		}
		else {
			index++;
			
			StartCoroutine(Move(_transform, currentHex, nextHex, movementSpeed, id));
		}
	}

	private IEnumerator Move(Transform transform, Vector3 source, Vector3 target, float time, int id)
	{
	    float startTime = Time.time;
		
	    while(Time.time < startTime + time)
	    {
	        transform.position = Vector3.Lerp(source, target, (Time.time - startTime)/time);
	        yield return null;
	    }
		
	    transform.position = target;
		
		StartCoroutine(MoveCharacter(id));
	}
	
	private IEnumerator Rotate(Transform transform, Quaternion source, Vector3 rotation, float time)
	{
		Quaternion target = Quaternion.LookRotation(rotation);
	    float startTime = Time.time;
		
	    while(Time.time < startTime + time)
	    {
	        transform.rotation = Quaternion.Slerp(source, target, (Time.time - startTime)/time);
	        yield return 0;
	    }
		
	    transform.rotation = target;
		
		yield return (startTime + time);
	}
}
