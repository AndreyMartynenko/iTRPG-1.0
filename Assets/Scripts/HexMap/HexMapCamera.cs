using UnityEngine;
using System.Collections;

public class HexMapCamera : MonoBehaviour {
	
	#region =Variables=
	
	public float height = 10f;
	public float angle = 75f;
	public float speed = 10f;
	
	private Transform _camera;
	
	#endregion

	void Start () {
		_camera = transform;
		
		_camera.Translate(0, height, 0);
		_camera.Rotate(angle, 0, 0);
	}
	
	void Update () {
		if(Input.GetButton("CameraHorizontal")) {
			if(Input.GetAxis("CameraHorizontal") > 0)
				MoveCamera(Vector3.right);
			else
				MoveCamera(Vector3.left);
		}
		
		if(Input.GetButton("CameraVertical")) {
			if(Input.GetAxis("CameraVertical") > 0)
				MoveCamera(Vector3.forward);
			else
				MoveCamera(Vector3.back);
		}

		if(Input.GetButton("CameraZoom")) {
			if(Input.GetAxis("CameraZoom") > 0)
				MoveCamera(Vector3.down);
			else
				MoveCamera(Vector3.up);
		}
	}
	
	private void MoveCamera(Vector3 translation) {
		_camera.Translate(translation * speed * Time.deltaTime, Space.World);
	}
}
