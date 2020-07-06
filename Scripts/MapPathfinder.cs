using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
// TODO alias tuple generics
// using TupleInt2 = System.Tuple<int, int>;
// using TupleInt4 = System.Tuple<int,int,int,int>;

public static class MapPathfinder
{
    // approximately ideal path from cell i,j to cell n, m
    // assumes only one heightmap, see param $map of Get*PathFrom
    private static Dictionary<Tuple<int, int, int, int>, List<Tuple<int, int>>> recPathCache = new Dictionary<Tuple<int, int, int, int>, List<Tuple<int, int>>>();

    public static List<Tuple<int,int>> GetFastApproximateFullPathFrom(Heightmap map, Tuple<int,int> orig, Tuple<int,int> target)
    {
        int maxDim = map.getMaxDim();

        // if already know path, return path
        Tuple<int, int, int, int> p4 = new Tuple<int, int, int, int>(orig.Item1, orig.Item2, target.Item1, target.Item2);
        if (recPathCache.ContainsKey(p4))
        {
            return recPathCache[p4];
        }

        int dist = Mathf.Abs(orig.Item1 - target.Item1) + Mathf.Abs(orig.Item2 - target.Item2);

        // assume {fast && approximate} tolerates us returning literally any known path from orig -> intermediate, + intermediate -> target

        if (dist <= 2)
        {
            // cases:
            // i,j to (i + 1),j: dist = 1
            // i,j to (i + 1),(j + 1): dist = 2
            return GetFastShortDistanceFrom(map, orig, target);
        }
        else
        {
            // if distance longer than 2, split and recurse. 
            Tuple<int, int> p_intermediate = new Tuple<int, int>((orig.Item1 + target.Item1) / 2, (orig.Item2 + target.Item2) / 2);
            List<Tuple<int, int>> p1 = GetFastApproximateFullPathFrom(map, orig, p_intermediate);
            List<Tuple<int, int>> p2 = GetFastApproximateFullPathFrom(map, p_intermediate, target);

            // join two paths
            List<Tuple<int, int>> combined = new List<Tuple<int, int>>(p1);
            combined.AddRange(p2);

            // store in cache so we don't lose work. we only have to store path from ij to nm.
            recPathCache[p4] = combined;

            return combined;
        }

        // later we could modify distance check and cases
    }

    /// <summary>
    /// Gets path from cell i,j to cell n,m where dist(i,j to n,m) < 1
    /// </summary>
    /// <param name="map">A heightmap with a max dimension</param>
    /// <param name="orig">Origin cell i,j</param>
    /// <param name="target">Target cell n,m</param>
    /// <returns></returns>
    private static List<Tuple<int,int>> GetFastShortDistanceFrom(Heightmap map, Tuple<int,int> orig, Tuple<int,int> target)
    {
        // path from i,j to adjacent n,m is simply i,j
        return new List<Tuple<int, int>> { orig };

        // this function will be extended later to support caching even when distances > 1, and better solutions even when i,j is
        // within distance 1 but not adjacent (due to cells being unpathable)
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
