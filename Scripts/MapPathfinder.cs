using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public static class MapPathfinder
{
    // TODO stub; implement faster more general function later
    public static List<Tuple<int,int>> GetSlowFullPathFrom(Heightmap map, Tuple<int,int> orig, Tuple<int, int> target, int cellSearchLimit = 128)
    {
        // inline function for getting cells adjacent to cell i,j in a grid
        Func < Tuple<int, int>, IEnumerable <Tuple<int, int>>> GetAdjacent = (Tuple<int, int> p) =>
        {
            HashSet<Tuple<int, int>> items = new HashSet<Tuple<int, int>>();

            // TODO convert to param later. need to retrieve from $map
            const int maxDim = 128;

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
        };


        // general objective:
        // use cell grid search. BFS from $orig to $target.
        // this pathfinding process will not work if we start from target and work backwards.

        // a point can serve as both a lookup for whether a point has been visited, and a map to shortest known path
        Dictionary<Tuple<int, int>, List<Tuple<int, int>>> shortestPath = new Dictionary<Tuple<int, int>, List<Tuple<int, int>>>();
        // shortest path from $orig to $orig is empty list
        shortestPath.Add(orig, new List<Tuple<int, int>>());
        
        Queue<Tuple<int, int>> pending = new Queue<Tuple<int, int>>();
        pending.Enqueue(orig);
        int iters = 0;
        while (pending.Count > 0 && iters++ < cellSearchLimit)
        {
            Tuple<int,int> top = pending.Dequeue();
            foreach (Tuple<int,int> adj in GetAdjacent(top))
            {
                if (shortestPath.ContainsKey(adj)) continue; // -> we have already encountered this path. assume we already have the shortest path
                if (!shortestPath.ContainsKey(top)) continue; // -> we do not actually know a path to this cell

                // the path to $adj is <path from orig to top> + <top>
                // presumably we need to copy the list's elements using GetRange
                shortestPath[adj] = shortestPath[top].GetRange(0, shortestPath[top].Count);
                shortestPath[adj].Add(top);

                if (shortestPath.ContainsKey(target))
                {
                    // if adj == target, then we already built the path
                    return shortestPath[target];
                }

                // else add it to the stack of things to investigate
                pending.Enqueue(adj);
            }
        }

        // either no path, or no path found within time available
        return null;
    }
}
