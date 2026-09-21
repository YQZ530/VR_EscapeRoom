
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace DCXR.Graph
{
    public enum GraphType
    {
        Bargraph = 0,
        LineGraph,
        Piechart,
        Boxplot,
        Histogram,
    }

    public enum PosesClusterFeature
    {
        WalkPath = 0,
        GameplayDur,
        NumCoinsCollected,
        NumBombHit,
        NumObstaclesHit,
        NumTreasureCollected,

        Tot,
    }

    public enum GroupFeature
    {
        GameplayDur,
        NumCoinsCollected,
        NumBombHit,
        NumObstaclesHit,
        NumTreasureCollected,
        GroupID,
        All,
    }





}
