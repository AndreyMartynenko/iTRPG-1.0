using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using Utility;

namespace PathFinding {
    interface IPathFinder {
		
        #region Properties
		
        HeuristicFormula Formula {
            set;
            get;
        }

        int HeuristicEstimate {
            set;
            get;
        }

        #endregion

        #region Methods
        List<Vector3> FindPath(Point start, Point end);
        #endregion
    }
}
