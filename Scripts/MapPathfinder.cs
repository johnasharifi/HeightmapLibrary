using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Path = System.Collections.Generic.List<System.Tuple<int, int>>;

// TODO alias tuple generics
// using TupleInt2 = System.Tuple<int, int>;
// using TupleInt4 = System.Tuple<int,int,int,int>;

// TODO ensure tuples cannot trigger index-out-of-bounds errors when looking up vectorRecommendedPaths

// TODO make a non-static method of a Heightmap

public static class MapPathfinder
{
    private static bool featurePathfindingDebugDraw = true;
    // TODO cache search results between segments
    
    // Contains info about map routes
    private static Dictionary<Vector2, MapRouteMagnet> routerTable = new Dictionary<Vector2, MapRouteMagnet>();
    
    public static List<Tuple<int, int>> GetFastApproximateFullPathFrom(Heightmap map, Tuple<int, int> origxz, Tuple<int, int> targetxz)
    {
        return Astar(map, origxz, targetxz);
    }
    
    private static List<Tuple<int,int>> Astar(Heightmap map, Tuple<int,int> origxz, Tuple<int,int> targetxz)
    {
        SortedDictionary<float, Path> paths = new SortedDictionary<float, Path>();
        paths[0f] = new Path(new Tuple<int, int>[] { origxz });
        
        Path pShort = GetPathRecommendation(map, origxz, targetxz);
        if (pShort != null)
        {
            return pShort;
        }

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
                    
                    float traversalCost = (Mathf.Abs(adj.Item1 - priorityTerminus.Item1) + Mathf.Abs(adj.Item2 - priorityTerminus.Item2)) / map.speedTable[map[adj.Item1, adj.Item2]];

                    float approachBonus = Vector2.Distance(new Vector2(targetxz.Item1, targetxz.Item2), new Vector2(adj.Item1, adj.Item2));
                    float fleeBonus = Vector2.Distance(new Vector2(origxz.Item1, origxz.Item2), new Vector2(adj.Item1, adj.Item2));

                    // float distanceAdj = priorityDistance + approachBonus + traversalCost;
                    float distanceAdj = approachBonus + 0.1f * fleeBonus + 0.1f * traversalCost + 0.1f * priorityDistance;
                    paths[distanceAdj] = pathAdj;

                    // if we reached the target by any path, return that path
                    if (adj.Item1 == targetxz.Item1 && adj.Item2 == targetxz.Item2)
                    {
                        if(featurePathfindingDebugDraw)
                        {
                            // assume first key -> path to target
                            DrawDebugPath(added, paths[distanceAdj].Count);
                        }

                        AddPathRecommendation(map, added[adj.Item1, adj.Item2]);

                        return added[adj.Item1, adj.Item2];
                    }
                    
                }
            }

            // remove first element from sorted dictionary
            paths.Remove(paths.Keys.First());
        }
        
        return new Path();
    }

    private static void DrawDebugPath(Path[,] pathRecords, int longestPath)
    {
        GameObject go = GameObject.Find("PathfindingDebugQuad");
        if (go == null) return;
        int maxDim = pathRecords.GetLength(0);

        Texture2D pathLogger = new Texture2D(maxDim, maxDim);
        for (int i = 0; i < maxDim; i++)
        {
            for (int j = 0; j < maxDim; j++)
            {
                if (pathRecords[i,j] != null)
                    pathLogger.SetPixel(i, j, Color.Lerp(Color.white, Color.blue, pathRecords[i,j].Count * 1.0f / longestPath));
            }
        }
        pathLogger.Apply();

        go.GetComponent<Renderer>().material.mainTexture = pathLogger;
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
    
    /// <summary>
    /// Defines adjacency for a cell i,j in a grid of integer-discretized cells
    /// </summary>
    /// <param name="maxDim">Cell grid size</param>
    /// <param name="p">A point i,j in grid</param>
    /// <returns>A collection of cells adjacent to cell i,j</returns>
    private static IEnumerable<Tuple<int, int>>[,] adjCache;
    static IEnumerable<Tuple<int, int>> GetAdjacent(int maxDim, Tuple<int, int> p)
    {
        if (adjCache == null)
        {
            adjCache = new IEnumerable<Tuple<int, int>>[maxDim, maxDim];
        }

        const int span = 2;
        if (adjCache[p.Item1, p.Item2] == null)
        {
            HashSet<Tuple<int, int>> items = new HashSet<Tuple<int, int>>();

            HashSet<Tuple<int, int>> collection = new HashSet<Tuple<int, int>>();
            for (int i = p.Item1 - span; i >= 0 && i < maxDim && i < p.Item1 + span + 1; i++)
            {
                for (int j = p.Item2 - span; j >= 0 && j < maxDim && j < p.Item2 + span + 1; j++)
                {
                    // skip original point
                    if (i == p.Item1 && j == p.Item2) { continue; };
                    items.Add(new Tuple<int, int>(i, j));
                }
            }

            adjCache[p.Item1, p.Item2] = items;
        }

        return adjCache[p.Item1, p.Item2];
    }
    
    private static Dictionary<Tuple<int, int, int, int>, Path> pathCache = new Dictionary<Tuple<int, int, int, int>, Path>();
    private static void AddPathRecommendation(Heightmap map, Path path)
    {
        //cache only full length path
        pathCache[new Tuple<int, int, int, int>(path[0].Item1, path[0].Item2, path[path.Count - 1].Item1, path[path.Count - 1].Item2)] = path;
    }
    
    private static Path GetPathRecommendation(Heightmap map, Tuple<int,int> orig, Tuple<int,int> target)
    {
        Tuple<int, int, int, int> t = new Tuple<int, int, int, int>(orig.Item1, orig.Item2, target.Item1, target.Item2);
        if (pathCache.ContainsKey(t))
        {
            return pathCache[t];
        }

        return null;
    }
}
