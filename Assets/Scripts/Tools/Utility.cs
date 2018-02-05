using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Utility {
	
	#region =Point struct=
	
    public struct Point {
        public int x, y;

        public Point(int x, int y) {
            this.x = x;
            this.y = y;
        }
		
		public static Point operator -(Point p1, Point p2) {
			if((object)p1 == null || (object)p2 == null)
				return new Point(-1, -1);
			
			return new Point(p1.x - p2.x, p1.y - p2.y);
		}

		public static bool operator ==(Point p1, Point p2) {
			if((object)p1 == null || (object)p2 == null)
				return false;
			
			return (p1.x == p2.x) && (p1.y == p2.y);
		}

		public static bool operator !=(Point p1, Point p2) {
			return !(p1 == p2);
		}
		
	    public override bool Equals(object obj) {
	        if(obj == null)
	            return false;
	
	        //If parameter cannot be cast to Point return false
	        Point p = (Point)obj;
	        if((object)p == null)
	            return false;
	
	        return (x == p.x) && (y == p.y);
	    }
	
	    public bool Equals(Point p) {
	        if((object)p == null)
	            return false;
	
	        return (x == p.x) && (y == p.y);
	    }
	
	    public override int GetHashCode() {
	        return x ^ y;
	    }	
	}
	
	#endregion
	
	public enum MovementType {
		Walk,
		Run,
		Crouch
	}
	
	public class Tools {
		private static MapManager map = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
		
		public static Vector3 GetGlobalPosition(int x, int y) {
			float w = (y % 2 == 0) ? (x * map.hexExtents.x * 2) : (x * map.hexExtents.x * 2 + map.hexExtents.x);
			float h = y * map.hexExtents.y * 1.5f;
			
			return new Vector3(w, 0, h);
		}

		public static Point GetLocalPosition(float x, float y) {
			//Debug.Log("x: " + x + ", y: " + y);
			float h =  y / map.hexExtents.y / 1.5f;
			float w = (h % 2 == 0) ? (x / map.hexExtents.x / 2 + 0.1f) : ((x - map.hexExtents.x) / map.hexExtents.x / 2 + 0.1f);
			//Debug.Log("w: " + w + ", h: " + h);
			//Debug.Log("w: " + (int)Mathf.Round(w) + ", h: " + (int)Mathf.Round(h));
			
			return new Point((int)Mathf.Round(w), (int)Mathf.Round(h));
		}
		
		public static List<Vector3> FindPath(Point start, Point end) {
			return map.FindPath(start, end);
		}
		
		public static void ToggleMobPosition(int x, int y, bool hasMob) {
			ToggleMobPosition(new Point(x, y), hasMob);
		}
		
		public static void ToggleMobPosition(Point position, bool hasMob) {
			if(hasMob)
				map.hexBoard[position.x, position.y] = 2;
			else
				map.hexBoard[position.x, position.y] = 1;
		}
	}
}