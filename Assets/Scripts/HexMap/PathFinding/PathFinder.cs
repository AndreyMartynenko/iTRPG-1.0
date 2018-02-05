using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Utility;

namespace PathFinding {
	
    #region =Structs & Enums=
	
    public struct PathFinderNode {
        public int f;
        public int g;
        public int h; //f = gone + heuristic
        public int x;
        public int y;
        public int pX; //parent
        public int pY;
    }

    public enum HeuristicFormula {
        Manhattan,
        MaxDXDY,
        DiagonalShortCut,
        Euclidean,
        EuclideanNoSQR,
        Custom
    }
	
    #endregion

    public class PathFinder : IPathFinder {

        #region =Variables=
		
        private byte[,]                       	hexGrid     	= null;
        private PriorityQueue<PathFinderNode> 	openNodes   	= new PriorityQueue<PathFinderNode>(new ComparePFNode());
        private List<PathFinderNode>          	closeNodes  	= new List<PathFinderNode>();
        private HeuristicFormula              	hFormula    	= HeuristicFormula.MaxDXDY;
        private int                           	hEstimate   	= 1;
        private int                           	searchLimit 	= 2000; //ToDo: revisar
		
        #endregion

        public PathFinder(byte[,] grid) {
            if(grid == null)
				Debug.LogError("Grid cannot be null.");

            hexGrid = grid;
        }

        #region =Setters & Getters=
		
        public HeuristicFormula Formula {
            set { hFormula = value; }
            get { return hFormula; }
        }

        public int HeuristicEstimate {
            set { hEstimate = value; }
            get { return hEstimate; }
        }

        #endregion

        //public List<PathFinderNode> FindPath(Point start, Point end) {
        public List<Vector3> FindPath(Point start, Point end) {
			//Debug.Log("start: [" + start.x + ", " + start.y + "], end: [" + end.x + ", " + end.y + "]");
			
            PathFinderNode parentNode;
            bool found = false;
            int gridX = hexGrid.GetUpperBound(0) + 1;
            int gridY = hexGrid.GetUpperBound(1) + 1;
			int newG;
			sbyte[,] direction;
			
            openNodes.Clear();
            closeNodes.Clear();

            parentNode.g = 0;
            parentNode.h = hEstimate;
            parentNode.f = parentNode.g + parentNode.h;
            parentNode.x = start.x;
            parentNode.y = start.y;
            parentNode.pX = parentNode.x;
            parentNode.pY = parentNode.y;
            openNodes.Push(parentNode);
			
            while(openNodes.Count > 0) {
                parentNode = openNodes.Pop();

                if(parentNode.x == end.x && parentNode.y == end.y) { //camino encontrado
                    closeNodes.Add(parentNode);
                    found = true;
                    break;
                }

                if(closeNodes.Count > searchLimit) //ToDo: revisar mSearchLimit
                    return null;

				if(parentNode.y % 2 == 0)
					direction = new sbyte[6,2]{ {0,-1}, {1,0}, {0,1}, {-1,1}, {-1,0}, {-1,-1} }; //par
				else
					direction = new sbyte[6,2]{ {1,-1}, {1,0}, {1,1}, {0,1}, {-1,0}, {0,-1} }; //impar
				
				//Lets calculate each successors
                for(int cnt = 0; cnt < 6; cnt++) {
                    PathFinderNode newNode;
					
                    newNode.x = parentNode.x + direction[cnt,0];
                    newNode.y = parentNode.y + direction[cnt,1];
					
                    if(newNode.x < 0 || newNode.x >= gridX || newNode.y < 0 || newNode.y >= gridY)
                        continue;
					
                    newG = parentNode.g + hexGrid[newNode.x, newNode.y];

                    if(hexGrid[newNode.x, newNode.y] == 0) //Obstacle
                        continue;

                    if(hexGrid[newNode.x, newNode.y] == 2) //Mob
                        continue;

                    int foundInOpenIndex = -1;
                    for(int i = 0; i < openNodes.Count; i++) {
                        if(openNodes[i].x == newNode.x && openNodes[i].y == newNode.y) {
                            foundInOpenIndex = i;
                            break;
                        }
                    }
					
                    if(foundInOpenIndex != -1 && openNodes[foundInOpenIndex].g <= newG)
                        continue;

					int foundInCloseIndex = -1;
					for(int i = 0; i < closeNodes.Count; i++) {
						if(closeNodes[i].x == newNode.x && closeNodes[i].y == newNode.y) {
							foundInCloseIndex = i;
							break;
						}
					}
					
                    if(foundInCloseIndex != -1 && closeNodes[foundInCloseIndex].g <= newG)
                        continue;

                    newNode.pX = parentNode.x;
                    newNode.pY = parentNode.y;
                    newNode.g = newG;

                    switch(hFormula) {
                        default:
						
                        case HeuristicFormula.Manhattan:
                            newNode.h = hEstimate * (Math.Abs(newNode.x - end.x) + Math.Abs(newNode.y - end.y));
                            break;
						
                        case HeuristicFormula.MaxDXDY:
                            newNode.h = hEstimate * (Math.Max(Math.Abs(newNode.x - end.x), Math.Abs(newNode.y - end.y)));
                            break;
						
                        case HeuristicFormula.DiagonalShortCut:
                            int hDiagonal = Math.Min(Math.Abs(newNode.x - end.x), Math.Abs(newNode.y - end.y));
                            int hStraight = Math.Abs(newNode.x - end.x) + Math.Abs(newNode.y - end.y);
                            newNode.h = (hEstimate * 2) * hDiagonal + hEstimate * (hStraight - 2 * hDiagonal);
                            break;
						
                        case HeuristicFormula.Euclidean:
                            newNode.h = (int)(hEstimate * Math.Sqrt(Math.Pow((newNode.x - end.x) , 2) + Math.Pow((newNode.y - end.y), 2)));
                            break;
						
                        case HeuristicFormula.EuclideanNoSQR:
                            newNode.h = (int)(hEstimate * (Math.Pow((newNode.x - end.x) , 2) + Math.Pow((newNode.y - end.y), 2)));
                            break;
						
                        case HeuristicFormula.Custom:
                            Point dXY 		= new Point((byte)Math.Abs(end.x - newNode.x), (byte)Math.Abs(end.y - newNode.y));
                            int orthogonal 	= Math.Abs(dXY.x - dXY.y);
                            int diagonal 	= Math.Abs(((dXY.x + dXY.y) - orthogonal) / 2);
                            newNode.h 		= hEstimate * (diagonal + orthogonal + dXY.x + dXY.y);
                            break;
                    }
					
                    newNode.f = newNode.g + newNode.h;

                    openNodes.Push(newNode);
                }
				
                closeNodes.Add(parentNode);
            }

            if(found) {
                PathFinderNode fNode = closeNodes[closeNodes.Count - 1];
                for(int cnt = closeNodes.Count - 1; cnt >= 0; cnt--) {
                    if(fNode.pX == closeNodes[cnt].x && fNode.pY == closeNodes[cnt].y || cnt == closeNodes.Count - 1)
                        fNode = closeNodes[cnt];
                    else
                        closeNodes.RemoveAt(cnt);
                }
				
				//////////////////////////////////////////////
				List<Vector3> path = new List<Vector3>();
				
				for(int cnt = 0; cnt < closeNodes.Count; cnt++)
					path.Add(new Vector3(closeNodes[cnt].x, closeNodes[cnt].y, closeNodes.Count - 1));
				
                //return closeNodes;
				return path;
            }
			
            return null;
        }

        #region Inner Classes
        internal class ComparePFNode : IComparer<PathFinderNode> {
            public int Compare(PathFinderNode x, PathFinderNode y) {
				
                if(x.f > y.f)
                    return 1;
                else if(x.f < y.f)
                    return -1;
                return 0;
            }
        }
        #endregion
    }
}
