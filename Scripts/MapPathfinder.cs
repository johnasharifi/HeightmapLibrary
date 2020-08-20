using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Point = System.Tuple<int, int>;
using PointPair = System.Tuple<int, int, int, int>;
using Path = System.Collections.Generic.List<System.Tuple<int,int>>;

public class MapPathfinder
{
    private static bool featurePathfindingDebugDraw = true;

    private Heightmap map;
    public MapPathfinder (Heightmap _map)
    {
        map = _map;
    }

    const float maxPointToPointMag = 2500f;
    private static float Magnitude(Point p1, Point p2)
    {
        return Vector2.SqrMagnitude(new Vector2(p1.Item1, p1.Item2) - new Vector2(p2.Item1, p2.Item2));
    }

    public List<Point> GetFastApproximateFullPathFrom(Point origxz, Point targetxz)
    {
        if (Magnitude(origxz, targetxz) < maxPointToPointMag)
        {
            return Astar(origxz, targetxz, searchSpan: 1);
        }
        return Astar(origxz, targetxz, searchSpan: 2);
    }
    
    private List<Point> Astar(Point origxz, Point targetxz, int searchSpan)
    {
        SortedDictionary<float, Path> paths = new SortedDictionary<float, Path>();
        paths[0f] = new Path(new Point[] { origxz });
        
        Path pShort = GetPathRecommendation(origxz, targetxz);
        pShort = null;
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
            Point priorityTerminus = priorityPath[priorityPath.Count - 1];

            foreach (Point adj in GetAdjacent(priorityTerminus, searchSpan))
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
                    
                    float distanceAdj = 0.2f * approachBonus + 0.1f * fleeBonus + 0.2f * traversalCost + 0.5f * priorityDistance;
                    paths[distanceAdj] = pathAdj;

                    // if we reached the target by any path, return that path
                    if (adj.Item1 == targetxz.Item1 && adj.Item2 == targetxz.Item2)
                    {
                        if(featurePathfindingDebugDraw)
                        {
                            // assume first key -> path to target
                            DrawDebugPath(added, paths[distanceAdj].Count);
                        }

                        AddPathRecommendation(added[adj.Item1, adj.Item2]);

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
    
    /// <summary>
    /// Defines adjacency for a cell i,j in a grid of integer-discretized cells
    /// </summary>
    /// <param name="maxDim">Cell grid size</param>
    /// <param name="p">A point i,j in grid</param>
    /// <returns>A collection of cells adjacent to cell i,j</returns>
    private Dictionary<Tuple<int,int,int>, HashSet<Point>> adjCache = new Dictionary<Tuple<int, int, int>, HashSet<Point>>();
    IEnumerable<Point> GetAdjacent(Point p, int searchSpan = 1)
    {
        int maxDim = map.getMaxDim();
        if (!adjCache.ContainsKey(new Tuple<int,int,int>(p.Item1, p.Item2, searchSpan)))
        {
            adjCache[new Tuple<int, int, int>(p.Item1, p.Item2, searchSpan)] = new HashSet<Point>();

            for (int i = Mathf.Max(0, p.Item1 - searchSpan); i < Mathf.Min(maxDim, p.Item1 + searchSpan + 1); i++)
            {
                for (int j = Mathf.Max(0, p.Item2 - searchSpan); j < Mathf.Min(maxDim, p.Item2 + searchSpan + 1); j++)
                {
                    if (i == j && j == p.Item2) continue; // ij is not adjacent to itself
                    adjCache[new Tuple<int,int,int>(p.Item1, p.Item2, searchSpan)].Add(new Point(i,j));
                }
            }
        }
        
        return adjCache[new Tuple<int,int,int>(p.Item1, p.Item2, searchSpan)];
    }
    
    private Dictionary<PointPair, Path> pathCache = new Dictionary<PointPair, Path>();
    private void AddPathRecommendation(Path path)
    {
        //cache only full length path
        pathCache[new Tuple<int, int, int, int>(path[0].Item1, path[0].Item2, path[path.Count - 1].Item1, path[path.Count - 1].Item2)] = path;
    }
    
    private Path GetPathRecommendation(Point orig, Point target)
    {
        Tuple<int, int, int, int> t = new PointPair(orig.Item1, orig.Item2, target.Item1, target.Item2);
        if (pathCache.ContainsKey(t))
        {
            return pathCache[t];
        }

        return null;
    }
}
