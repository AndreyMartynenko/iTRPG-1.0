using UnityEngine;
using System.Collections;
using Utility;

public class HexagonTile : MonoBehaviour {
	
	#region =Variables=
	
	private Transform _transform;

	private Point _position;
	private bool _isSelected;
	private bool _isDestination;
	
	#endregion
	
	#region =Getters & Setters=
	
	public Point Position {
		get { return _position; }
		set { _position = value; }
	}
	
	public bool IsSelected {
		get { return _isSelected; }
		set { _isSelected = value; }
	}
	
	public bool IsDestination {
		get { return _isDestination; }
		set { _isDestination = value; }
	}
	
	#endregion
	
	void Start() {
		_transform = transform;
	}
	
	private void OnMouseUp() {
		Debug.Log("Hex selected: [" + _position.x + ", " + _position.y + "]");
		
		if(_isSelected)
			_isDestination = true;
				
		_isSelected = true;
		
		//if(Input.GetMouseButtonDown(0))
		Messenger<GameObject>.Broadcast("HexSelected", _transform.gameObject);
	}
}