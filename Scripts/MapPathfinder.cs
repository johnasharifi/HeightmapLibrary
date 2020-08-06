using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Path = System.Collections.Generic.List<System.Tuple<int, int>>;

// TODO alias tuple generics
// using TupleInt2 = System.Tuple<int, int>;
// using TupleInt4 = System.Tuple<int,int,int,int>;

// TODO make a non-static method of a Heightmap

public static class MapPathfinder
{
    // TODO cache search results between segments
    
    // Contains info about map routes
    private static Dictionary<Vector2, MapRouteMagnet> routerTable = new Dictionary<Vector2, MapRouteMagnet>();

    private const int maxSegmentSplitDistance = 2;

    public static List<Tuple<int, int>> GetFastApproximateFullPathFrom(Heightmap map, Tuple<int, int> origxz, Tuple<int, int> targetxz)
    {
        return Astar(map, origxz, targetxz);
    }
    
    private static List<Tuple<int,int>> Astar(Heightmap map, Tuple<int,int> origxz, Tuple<int,int> targetxz)
    {
        SortedDictionary<float, Path> paths = new SortedDictionary<float, Path>();
        paths[0f] = new Path(new Tuple<int, int>[] { origxz });

        int maxDim = map.getMaxDim();

        // array of nulls
        Path[,] added = new Path[maxDim, maxDim];

        int counter = 0;
        while (paths.Count > 0 && counter++ < 4096)
        {
            float priorityDistance = paths.Keys.First();
            Path priorityPath = paths[priorityDistance];
            Tuple<int, int> priorityTerminus = priorityPath[priorityPath.Count - 1];

            foreach (Tuple<int, int> adj in GetAdjacent(maxDim, priorityTerminus))
            {
                if (added[adj.Item1, adj.Item2] == null)
                {
                    // add to array of paths so we know sequence of steps from origxz to adj
                    Path pathAdj = priorityPath.GetRange(0, priorityPath.Count);
                    pathAdj.Add(adj);
                    added[adj.Item1, adj.Item2] = pathAdj;

                    // inverse of speed = cost approximation. diagonal moves cost 1.4x 
                    float traversalDistance = 1.0f / map.speedTable[map[adj.Item1, adj.Item2]] * Vector2.SqrMagnitude(new Vector2(origxz.Item1, origxz.Item2) - new Vector2(adj.Item1, adj.Item2));
                    float distanceAdj = priorityDistance + traversalDistance;
                    paths[distanceAdj] = pathAdj;

                    // if we reached the target by any path, return that path
                    if (adj.Item1 == targetxz.Item1 && adj.Item2 == targetxz.Item2)
                    {
                        return added[adj.Item1, adj.Item2];
                    }
                }
            }
            
            // remove first element from sorted dictionary
            paths.Remove(paths.Keys.First());
        }
        
        return new Path();
    }

    private static List<Tuple<int,int>> BFS(Heightmap map, Tuple<int,int> origxz, Tuple<int,int> targetxz)
    {
        int maxDim = map.getMaxDim();
        
        Path[,] paths = new Path[maxDim, maxDim];
        paths[origxz.Item1, origxz.Item2] = new Path();

        Queue<Tuple<int, int>> queue = new Queue<Tuple<int, int>>();
        queue.Enqueue(origxz);

        // origin point has a zero-len non-null path to itself
        paths[origxz.Item1, origxz.Item2] = new Path();

        int counter = 0;
        while (queue.Count > 0 && counter++ < 4096)
        {
            Tuple<int, int> curr = queue.Dequeue();

            foreach (Tuple<int,int> adj in GetAdjacent(maxDim, curr))
            {
                // if no path, we need to define a path
                if (paths[adj.Item1, adj.Item2] == null)
                {
                    Path pathAdj = paths[curr.Item1, curr.Item2].GetRange(0, paths[curr.Item1, curr.Item2].Count);
                    pathAdj.Add(adj);
                    paths[adj.Item1, adj.Item2] = pathAdj;

                    queue.Enqueue(adj);

                    if (adj.Item1 == targetxz.Item1 && adj.Item2 == targetxz.Item2)
                    {
                        return paths[adj.Item1, adj.Item2];
                    }
                }
            }

        }
        
        return new Path();
    }

    // TODO cache adjacency data; currently we do n^2 iteration and new() each time, but could cache to avoid impact
    /// <summary>
    /// Defines adjacency for a cell i,j in a grid of integer-discretized cells
    /// </summary>
    /// <param name="maxDim">Cell grid size</param>
    /// <param name="p">A point i,j in grid</param>
    /// <returns>A collection of cells adjacent to cell i,j</returns>
    static IEnumerable<Tuple<int, int>> GetAdjacent(int maxDim, Tuple<int, int> p)
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
