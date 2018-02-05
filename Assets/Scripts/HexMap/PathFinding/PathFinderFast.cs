using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Utility;

namespace PathFinding {
    public class PathFinderFast : IPathFinder {
		
        #region =Structs=
		
        internal struct PathFinderNodeFast {
            public int    f; //f = gone + heuristic
            public int    g;
            public ushort pX; //Parent
            public ushort pY;
            public byte   status;
        }
		
        #endregion

        #region =Variables=
		
        // Heap variables are initializated to default, but I like to do it anyway
        private byte[,]              hexGrid        = null;
        private PriorityQueue<int>   openNodes      = null;
        private List<PathFinderNode> closeNodes     = new List<PathFinderNode>();
        private HeuristicFormula     hFormula       = HeuristicFormula.Manhattan;
        private int                  hEstimate      = 2;
        private int                  searchLimit    = 2000;
        private PathFinderNodeFast[] calcGrid       = null;
        private byte                 openNodeValue  = 1;
        private byte                 closeNodeValue = 2;
        
        //Promoted local variables to member variables to avoid recreation between calls
        private int      newH             = 0;
        private int      location         = 0;
        private int      newLocation      = 0;
        private ushort   locationX        = 0;
        private ushort   locationY        = 0;
        private ushort   newLocationX     = 0;
        private ushort   newLocationY     = 0;
        private int      closeNodeCounter = 0;
        private ushort   gridX            = 0;
        private ushort   gridY            = 0;
        private ushort   gridXMinus1      = 0;
        private ushort   gridYLog2        = 0;
        private bool     found            = false;
        private sbyte[,] direction        = null;
        private int      endLocation      = 0;
        private int      newG             = 0;
		
        #endregion

        public PathFinderFast(byte[,] grid) {
            if(grid == null)
                throw new Exception("Grid cannot be null");

            hexGrid = grid;
            gridX = (ushort)(hexGrid.GetUpperBound(0) + 1);
            gridY = (ushort)(hexGrid.GetUpperBound(1) + 1);
            gridXMinus1 = (ushort)(gridX - 1);
            gridYLog2 = (ushort)Math.Log(gridY, 2);

            // This should be done at the constructor, for now we leave it here.
            if(grid == null || Math.Log(gridX, 2) != (int)Math.Log(gridX, 2) || Math.Log(gridY, 2) != (int)Math.Log(gridY, 2))
				Debug.LogError("Invalid Grid, size in X and Y must be power of 2.");

            if(calcGrid == null || calcGrid.Length != (gridX * gridY))
                calcGrid = new PathFinderNodeFast[gridX * gridY];

            openNodes = new PriorityQueue<int>(new ComparePFNodeMatrix(calcGrid));
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

        #region Methods

        public List<Vector3> FindPath(Point start, Point end) {
            lock(this) {
                // Is faster if we don't clear the matrix, just assign different values for open and close and ignore the rest
                // I could have user Array.Clear() but using unsafe code is faster, no much but it is.

                found             = false;
                closeNodeCounter  = 0;
                openNodeValue     += 2;
                closeNodeValue    += 2;
                openNodes.Clear();
                closeNodes.Clear();

                location                  = (start.y << gridYLog2) + start.x;
                endLocation               = (end.y << gridYLog2) + end.x;
                calcGrid[location].g      = 0;
                calcGrid[location].f      = hEstimate;
                calcGrid[location].pX     = (ushort)start.x;
                calcGrid[location].pY     = (ushort)start.y;
                calcGrid[location].status = openNodeValue;

                openNodes.Push(location);
                while(openNodes.Count > 0) {
                    location = openNodes.Pop();

                    //Is it in closed list? means this node was already processed
                    if(calcGrid[location].status == closeNodeValue)
                        continue;

                    locationX = (ushort)(location & gridXMinus1);
                    locationY = (ushort)(location >> gridYLog2);
                    
                    if(location == endLocation) {
                        calcGrid[location].status = closeNodeValue;
                        found = true;
                        break;
                    }

                    if(closeNodeCounter > searchLimit)
                        return null;
					
					//////////////////////////////////////////////////////////////////////////////////////
					//////////////////////////////////////////////////////////////////////////////////////
					
					if(locationY % 2 == 0)
						direction = new sbyte[6,2]{ {0,-1}, {1,0}, {0,1}, {-1,1}, {-1,0}, {-1,-1} }; //par
					else
						direction = new sbyte[6,2]{ {1,-1}, {1,0}, {1,1}, {0,1}, {-1,0}, {0,-1} }; //impar

                    //Lets calculate each successors
                    for(int cnt = 0; cnt < 6; cnt++) {
                        newLocationX = (ushort)(locationX + direction[cnt,0]);
                        newLocationY = (ushort)(locationY + direction[cnt,1]);
                        newLocation = (newLocationY << gridYLog2) + newLocationX;

                        if(newLocationX >= gridX || newLocationY >= gridY)
                            continue;

                        if(hexGrid[newLocationX, newLocationY] == 0) //Obstacle
                            continue;

                        if(hexGrid[newLocationX, newLocationY] == 2) //Mob
                            continue;

                        newG = calcGrid[location].g + hexGrid[newLocationX, newLocationY];

                        //Is it open or closed?
                        if(calcGrid[newLocation].status == openNodeValue || calcGrid[newLocation].status == closeNodeValue) {
                            // The current node has less code than the previous? then skip this node
                            if(calcGrid[newLocation].g <= newG)
                                continue;
                        }

                        calcGrid[newLocation].pX = locationX;
                        calcGrid[newLocation].pY = locationY;
                        calcGrid[newLocation].g = newG;

                        switch(hFormula) {
                            default:
							
                            case HeuristicFormula.Manhattan:
                                newH = hEstimate * (Math.Abs(newLocationX - end.x) + Math.Abs(newLocationY - end.y));
                                break;
							
                            case HeuristicFormula.MaxDXDY:
                                newH = hEstimate * (Math.Max(Math.Abs(newLocationX - end.x), Math.Abs(newLocationY - end.y)));
                                break;
							
                            case HeuristicFormula.DiagonalShortCut:
                                int hDiagonal  = Math.Min(Math.Abs(newLocationX - end.x), Math.Abs(newLocationY - end.y));
                                int hStraight  = (Math.Abs(newLocationX - end.x) + Math.Abs(newLocationY - end.y));
                                newH = (hEstimate * 2) * hDiagonal + hEstimate * (hStraight - 2 * hDiagonal);
                                break;
							
                            case HeuristicFormula.Euclidean:
                                newH = (int)(hEstimate * Math.Sqrt(Math.Pow((newLocationY - end.x), 2) + Math.Pow((newLocationY - end.y), 2)));
                                break;
							
                            case HeuristicFormula.EuclideanNoSQR:
                                newH = (int)(hEstimate * (Math.Pow((newLocationX - end.x), 2) + Math.Pow((newLocationY - end.y), 2)));
                                break;
							
                            case HeuristicFormula.Custom:
                                Point dXY 		= new Point(Math.Abs(end.x - newLocationX), Math.Abs(end.y - newLocationY));
                                int orthogonal 	= Math.Abs(dXY.x - dXY.y);
                                int diagonal 	= Math.Abs(((dXY.x + dXY.y) - orthogonal) / 2);
                                newH 			= hEstimate * (diagonal + orthogonal + dXY.x + dXY.y);
                                break;
                        }
						
                        calcGrid[newLocation].f = newG + newH;

                        //It is faster if we leave the open node in the priority queue
                        //When it is removed, it will be already closed, it will be ignored automatically
                        //if (tmpGrid[newLocation].Status == 1)
                        //{
                        //    //int removeX   = newLocation & gridXMinus1;
                        //    //int removeY   = newLocation >> gridYLog2;
                        //    mOpen.RemoveLocation(newLocation);
                        //}

                        //if (tmpGrid[newLocation].Status != 1)
                        //{
                            openNodes.Push(newLocation);
                        //}
                        calcGrid[newLocation].status = openNodeValue;
                    }

                    closeNodeCounter++;
                    calcGrid[location].status = closeNodeValue;
                }

                if(found) {
                    closeNodes.Clear();
                    int posX = end.x;
                    int posY = end.y;

                    PathFinderNodeFast fNodeTmp = calcGrid[(end.y << gridYLog2) + end.x];
                    PathFinderNode fNode;
                    fNode.f  = fNodeTmp.f;
                    fNode.g  = fNodeTmp.g;
                    fNode.h  = 0;
                    fNode.pX = fNodeTmp.pX;
                    fNode.pY = fNodeTmp.pY;
                    fNode.x  = end.x;
                    fNode.y  = end.y;

                    while(fNode.x != fNode.pX || fNode.y != fNode.pY) {
                        closeNodes.Add(fNode);
						
                        posX 	 = fNode.pX;
                        posY 	 = fNode.pY;
                        fNodeTmp = calcGrid[(posY << gridYLog2) + posX];
                        fNode.f  = fNodeTmp.f;
                        fNode.g  = fNodeTmp.g;
                        fNode.h  = 0;
                        fNode.pX = fNodeTmp.pX;
                        fNode.pY = fNodeTmp.pY;
                        fNode.x  = posX;
                        fNode.y  = posY;
                    } 

                    closeNodes.Add(fNode);

					//////////////////////////////////////////////
					List<Vector3> path = new List<Vector3>();
					
					//for(int cnt = 0; cnt < closeNodes.Count; cnt++)
					for(int cnt = closeNodes.Count - 1; cnt >= 0; cnt--)
						path.Add(new Vector3(closeNodes[cnt].x, closeNodes[cnt].y, closeNodes.Count - 1));
					
                    //return mClose;
					return path;
                }
				
                return null;
            }
        }
        #endregion

        #region Inner Classes
        internal class ComparePFNodeMatrix : IComparer<int> {
            #region Variables Declaration
            PathFinderNodeFast[] mMatrix;
            #endregion

            #region Constructors
            public ComparePFNodeMatrix(PathFinderNodeFast[] matrix) {
                mMatrix = matrix;
            }
            #endregion

            #region IComparer Members
            public int Compare(int a, int b) {
                if(mMatrix[a].f > mMatrix[b].f)
                    return 1;
                else if (mMatrix[a].f < mMatrix[b].f)
                    return -1;
                return 0;
            }
            #endregion
        }
        #endregion
    }
}
