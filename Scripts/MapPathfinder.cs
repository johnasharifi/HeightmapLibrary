using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Path = System.Collections.Generic.List<System.Tuple<int, int>>;

// TODO alias tuple generics
// using TupleInt2 = System.Tuple<int, int>;
// using TupleInt4 = System.Tuple<int,int,int,int>;

public static class MapPathfinder
{
    // TODO compute distance per each segment, cache
    // TODO cache search results between segments
    // TODO A* pathing

    // Contains info about map routes
    private static Dictionary<Vector2, MapRouteMagnet> routerTable = new Dictionary<Vector2, MapRouteMagnet>();

    private const int maxSegmentSplitDistance = 2;

    public static List<Tuple<int, int>> GetFastApproximateFullPathFrom(Heightmap map, Tuple<int,int> origxz, Tuple<int,int> targetxz)
    {
        // case: distance > maxSegmentSearchDistance
        if (Mathf.Abs(origxz.Item1 - targetxz.Item1) + Mathf.Abs(origxz.Item2 - targetxz.Item2) > maxSegmentSplitDistance)
        {
            Tuple<int, int> intermediate = new Tuple<int, int>((origxz.Item1 + targetxz.Item1) / 2, (origxz.Item2 + targetxz.Item2) / 2);

            List<Tuple<int, int>> pathP1 = GetFastApproximateFullPathFrom(map, origxz, intermediate);
            List<Tuple<int, int>> pathP2 = GetFastApproximateFullPathFrom(map, intermediate, targetxz);

            pathP1.AddRange(pathP2);
            return (pathP1);
        }
        
        return new List<Tuple<int, int>> { origxz };
    }

    // TODO cache adjacency data; currently we do n^2 iteration and new() each time, but could cache to avoid impact
    /// <summary>
    /// Defines adjacency for a cell i,j in a grid of integer-discretized cells
    /// </summary>
    /// <param name="maxDim">Cell grid size</param>
    /// <param name="p">A point i,j in grid</param>
    /// <returns>A collection of cells adjacent to cell i,j</returns>
    static IEnumerable<Tuple<int, int>> GetAdjacent (int maxDim, Tuple<int, int> p)
    {
        HashSet<Tuple<int, int>> items = new HashSet<Tuple<int, int>>();

        HashSet<Tuple<int, int>> collection = new HashSet<Tuple<int, int>>();
        for (int i = p.Item1 - 1; i >= 0 && i < maxDim && i < p.Item1 + 2; i++)
        {
            for (int j = p.Item2 - 1; j >= 0 && j < maxDim && j < p.Item2 + 2; j++)
            {
                // skip original point
                if (i == p.Item1 && j == p.Item2) { continue; };
                items.Add(new Tuple<int, int>(i, j));
            }
        }
        return items;
    }
}
