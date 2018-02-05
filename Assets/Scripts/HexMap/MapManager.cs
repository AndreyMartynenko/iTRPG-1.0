using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using PathFinding;
using Utility;

[AddComponentMenu("iTRPG/Managers/Hex map")]

public struct HighlightedMap {
	public Point position;
	public int distance;
	public GameObject hexagon;
	
	public HighlightedMap(Point pos, int dist, GameObject hex) {
		position = pos;
		distance = dist;
		hexagon = hex;
	}

	public HighlightedMap(int posX, int posY, int dist, GameObject hex) {
		position.x = posX;
		position.y = posY;
		distance = dist;
		hexagon = hex;
	}
}
	
public class MapManager : MonoBehaviour {
	
	#region =Variables=
	
	new public Camera camera;
	public GameObject hexagon;
	public Vector2 mapSize;
	public Material[] materials;
	public List<HighlightedMap> highlightedMap = new List<HighlightedMap>();
	public Vector2 hexExtents;
	
	public byte[,] hexBoard;
	private List<Point> obstacles;
	
	private GameObject currentHex = null;
	private GameObject selectedHex = null;
	
	private bool pcInDestination = true;
	
	//Path finder
	public HeuristicFormula heuristicFormula = HeuristicFormula.Manhattan;
	public int heuristicEstimate = 1;
	
	private IPathFinder pathFinder = null;
	private List<GameObject> pathToNewHex = new List<GameObject>();
	public List<Vector3> path = new List<Vector3>();
	
	private MobManager mobManager;
	
	//private bool togglePathFinderMode = false;
	
	#endregion
	
	void OnEnable() {
		Messenger<GameObject>.AddListener("HexSelected", OnSelectHex);
		Messenger.AddListener("PC in destination", PCInDestination);
		Messenger.AddListener("Start PC turn", StartPCTurn);
		Messenger.AddListener("End PC turn", EndPCTurn);
	}
	
	void OnDisable() {
		Messenger<GameObject>.RemoveListener("HexSelected", OnSelectHex);
		Messenger.RemoveListener("PC in destination", PCInDestination);
		Messenger.RemoveListener("Start PC turn", StartPCTurn);
		Messenger.RemoveListener("End PC turn", EndPCTurn);
	}
	
	void Awake() {
		GetHexProperties();
	}
	
	void Start() {
		GenerateHexBoard();
		
		//pathFinder = new PathFinder(hexBoard);
		pathFinder = new PathFinderFast(hexBoard);
		pathFinder.Formula = heuristicFormula;
		pathFinder.HeuristicEstimate = heuristicEstimate;
		
		StartCoroutine(HighlightMap(0, 0, PC.Instance.RemainingAP));
		//GenerateMap();
	}
	
	void Update() {

	}
	
	void OnGUI() {
		GUI.Label(new Rect(10, 60, 50, 21), "Range:");
		PC.Instance.RemainingAP = Convert.ToInt32(GUI.TextField(new Rect(70, 60, 30, 21), PC.Instance.RemainingAP.ToString()));
		
		//Path finder
		if(GUI.Button(new Rect(10, 90, 75, 25), "Path")) {
			pathFinder = new PathFinder(hexBoard);
			pathFinder.Formula = heuristicFormula;
			pathFinder.HeuristicEstimate = heuristicEstimate;
			
	        List<Vector3> path = pathFinder.FindPath(new Point(1, 4), new Point(0, 0));
			
			Debug.LogWarning(path.Count);
			for (int cnt = 0; cnt < path.Count; cnt++) {
				Debug.Log("path[x,y]: [" + path[cnt].x + ", " + path[cnt].y + "]");
			}
		}
	}
	
	private void GetHexProperties() {
		GameObject hex = Instantiate(hexagon, Vector3.zero, Quaternion.identity) as GameObject;
		
		hexExtents = new Vector2(hex.GetComponent<Collider>().bounds.extents.x, hex.GetComponent<Collider>().bounds.extents.z);
		Destroy(hex);
	}
	
	private void GenerateHexBoard() {
		hexBoard = new byte[(byte)mapSize.x, (byte)mapSize.y];
		
		for(byte x = 0; x < mapSize.x; x++)
			for(byte y = 0; y < mapSize.y; y++)
				hexBoard[x,y] = 1;
		
		GenerateObstacles();
	}
	
	private void GenerateObstacles() { //ToDo: obtener info de la db
		obstacles = new List<Point>();
		
		hexBoard[0,1] = 0;
		hexBoard[1,1] = 0;
		hexBoard[0,3] = 0;
		hexBoard[2,1] = 0;
		hexBoard[1,4] = 0;
		hexBoard[5,0] = 0;
		hexBoard[4,0] = 0;
		hexBoard[4,1] = 0;
		
		obstacles.Add(new Point(0, 1));
		obstacles.Add(new Point(1, 1));
		obstacles.Add(new Point(0, 3));
		obstacles.Add(new Point(2, 1));
		obstacles.Add(new Point(1, 4));
		obstacles.Add(new Point(5, 0));
		obstacles.Add(new Point(4, 0));
		obstacles.Add(new Point(4, 1));
	}
	
	private void OnSelectHex(GameObject hex) {
		if(mobManager.mobPositions.FindIndex(mobPos => mobPos == hex.GetComponent<HexagonTile>().Position) == -1) { //If there aren't any mob
			if(pcInDestination) {
				HexagonTile currentHexTile = currentHex.GetComponent<HexagonTile>();
				HexagonTile selectedHexTile = selectedHex != null ? selectedHex.GetComponent<HexagonTile>() : null;
				HexagonTile destinationHexTile = hex.GetComponent<HexagonTile>();
				
				if(destinationHexTile.IsDestination) {
					PC.Instance.Position = new Point(destinationHexTile.Position.x, destinationHexTile.Position.y);
					PC.Instance.RemainingAP -= path.Count - 1;
					
					currentHex = hex;
					currentHexTile = destinationHexTile;
					
					destinationHexTile.IsSelected = false;
					destinationHexTile.IsDestination = false;
					
					selectedHex = hex;
					
					//Move player character to the destination hex
					pcInDestination = false;
					Messenger<List<Vector3>>.Broadcast("MovePC", path);
					
					return;
				}
				
				if(selectedHex == null) {
					selectedHex = hex;
				}
				else {
					selectedHexTile.IsSelected = false;
					selectedHex = hex;
					
					foreach(GameObject p in pathToNewHex) {
						p.GetComponent<Renderer>().material = materials[0];
						p.GetComponent<Renderer>().material.color = Color.white;
					}
				}
				
				pathToNewHex.Clear();
				path = FindPath(currentHexTile.Position, destinationHexTile.Position);
		
				//Debug.Log("path.Count: " + path.Count + ". distance: " + path[0].z);
				for(int cnt = 0; cnt < path.Count; cnt++) {
					pathToNewHex.Add(highlightedMap.Find(hm => hm.position.x == path[cnt].x && hm.position.y == path[cnt].y).hexagon);
					pathToNewHex[cnt].GetComponent<Renderer>().material = materials[1];
					pathToNewHex[cnt].GetComponent<Renderer>().material.color = Color.red;
					
					path[cnt] = pathToNewHex[cnt].transform.position; //To get consistency with a mob AI script for Movement script
				}
				
				Resources.UnloadUnusedAssets();
			}
		}
	}
	
	public IEnumerator HighlightMap(int posX, int posY, int range) {
		//Lock this function
		System.Object locker = new System.Object();
		
		lock(locker) {
			//Destroy old map if it exists
			GameObject oldHexMap = GameObject.FindGameObjectWithTag("HexMap");
			
			if(oldHexMap)
				Destroy(oldHexMap);
			
			highlightedMap.Clear();
			
			//Create new map
			GameObject hexMap = new GameObject("HexMap");
			int xOffsetMin, xOffsetMax;
			int index;
			
			hexMap.tag = "HexMap";
			
			for(int y = -range + posY; y < range+1 + posY; y++) {
				
				//Get x-offsets
				xOffsetMin = Mathf.Abs((int)((y - posY) * 0.5f));
				xOffsetMax = xOffsetMin;
				
				if((y - posY) % 2 != 0) { //even
					xOffsetMin++;
					xOffsetMax = xOffsetMin - 1;
				}
				
				if((y % 2 != 0) && ((y - posY) % 2 != 0)) { //even
					xOffsetMin--;
					xOffsetMax++;
				}
				
				for(int x = -range + posX + xOffsetMin; x < range+1 + posX - xOffsetMax; x++) {
					if(x >= 0 && x < mapSize.x && y >= 0 && y < mapSize.y) {
						
						//Check if there is any obstacle; if no, continue
						index = obstacles.FindIndex(obst => obst.x == x && obst.y == y);
						if(index == -1) {
							
							//Check if distance to new point is less than the range; if so, instantiate a new hex tile
							List<Vector3> path = FindPath(new Point(posX, posY), new Point(x, y));
							if(path != null && path[0].z <= range) {
								
								//Instanciate all hexagon tiles
								GameObject hex = Instantiate(Resources.Load("Items/Mesh/Hex"), Vector3.zero, Quaternion.identity) as GameObject;
								
								hex.transform.position = Tools.GetGlobalPosition(x, y);
								hex.GetComponent<HexagonTile>().Position = new Point(x, y);
								hex.transform.parent = hexMap.transform;
								
								highlightedMap.Add(new HighlightedMap(x, y, 0, hex));
							}
						}
					}
				}
			}
			
			//Highlight hexs with mobs in hit range
			//Get reference to the MobManager script
			mobManager = GameObject.FindGameObjectWithTag("MobManager").GetComponent<MobManager>();
			
			for(int cnt = 0; cnt < mobManager.mobPositions.Count; cnt++) {
				Tools.ToggleMobPosition(mobManager.mobPositions[cnt], false);
				
				List<Vector3> path = FindPath(PC.Instance.Position, mobManager.mobPositions[cnt]);
				if(path[0].z <= PC.Instance.HitRange) {
					GameObject hex = Instantiate(Resources.Load("Items/Mesh/Hex"), Vector3.zero, Quaternion.identity) as GameObject;
					int x = (int)path[path.Count - 1].x;
					int y = (int)path[path.Count - 1].y;
					
					hex.transform.position = Tools.GetGlobalPosition(x, y);
					hex.GetComponent<HexagonTile>().Position = new Point(x, y);
					hex.transform.parent = hexMap.transform;
					
					hex.GetComponent<Renderer>().material = materials[2];
					hex.GetComponent<Renderer>().material.color = Color.red;
				}
				
				Tools.ToggleMobPosition(mobManager.mobPositions[cnt], true);
			}
			
			//Highlight hex on starting point
			currentHex = highlightedMap[highlightedMap.FindIndex(hex => hex.position.x == posX && hex.position.y == posY)].hexagon;
			currentHex.GetComponent<Renderer>().material = materials[1];
			currentHex.GetComponent<Renderer>().material.color = Color.blue;
			
			//Put a new map on the origin of coordinates
			hexMap.transform.position = Vector3.zero;
			
			//Auto unlock
		}
		
		yield return 1;
	}
	
	private void GenerateMap() { //Generate entire map
		GameObject hexMap = new GameObject("HexMap");
		int index;
		
		for(int y = 0; y < mapSize.y; y++) {
			for(int x = 0; x < mapSize.x; x++) {
				GameObject hex = Instantiate(hexagon, Vector3.zero, Quaternion.identity) as GameObject;
				
				hex.transform.position = Tools.GetGlobalPosition(x, y);
				hex.GetComponent<HexagonTile>().Position = new Point(x, y);
				hex.transform.parent = hexMap.transform;
				
				highlightedMap.Add(new HighlightedMap(x, y, 0, hex));
			}
		}
		
		foreach(Point obstacle in obstacles) {
			index = highlightedMap.FindIndex(hm => hm.position.x == obstacle.x && hm.position.y == obstacle.y);
			DestroyImmediate(highlightedMap[index].hexagon);
			highlightedMap.RemoveAt(index);
		}
		
		hexMap.transform.position = Vector3.zero;
		//Debug.Log(hexagons.Count.ToString());
	}
	
	public List<Vector3> FindPath(Point start, Point end) {
		return pathFinder.FindPath(start, end);
	}
	
	private void PCInDestination() {
		if(PC.Instance.IsPCTurn) {
			HexagonTile currentHexTile = currentHex.GetComponent<HexagonTile>();
			
			//PC.Instance.Position = new Point(currentHexTile.position.x, currentHexTile.position.y);
			
			pcInDestination = true;
			
			//Create a new highlighted map
			StartCoroutine(HighlightMap(currentHexTile.Position.x, currentHexTile.Position.y, PC.Instance.RemainingAP));
		}
	}
	
	private void StartPCTurn() {
		PC.Instance.IsPCTurn = true;
		PC.Instance.RemainingAP = 10; //ToDo: obtener en base a las props del PC
		
		StartCoroutine(HighlightMap(PC.Instance.Position.x, PC.Instance.Position.y, PC.Instance.RemainingAP));
	}

	private void EndPCTurn() {
		PC.Instance.IsPCTurn = false;
		
		//Destroy old map if it exists
		Destroy(GameObject.FindGameObjectWithTag("HexMap"));
		
		Messenger.Broadcast("Start mob turn");
	}
}
