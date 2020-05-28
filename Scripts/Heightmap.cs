﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Heightmap
{
    private int dim1;
    private int dim2;

    private Dictionary<int, HashSet<Tuple<int, int>>> points = new Dictionary<int, HashSet<Tuple<int, int>>>();

    public Heightmap(int _dim1, int _dim2)
    {
        dim1 = _dim1;
        dim2 = _dim2;
        
        points[-1] = new HashSet<Tuple<int, int>>();

        for (int i = 0; i < dim1; i++)
        {
            for (int j = 0; j < dim2; j++)
            {
                points[-1].Add(new Tuple<int, int>(i, j));
            }
        }
    }

    public void MapFromTo(int originClass, int targetClass, Func<int, int, bool> predicate)
    {
        MapFromTo(originClass, targetClass, originClass, predicate);
    }
    
    public void MapFromTo(int originClass, int targetClass, int failClass, Func<int, int, bool> predicate)
    {
        if (!points.ContainsKey(originClass))
        {
            points[originClass] = new HashSet<Tuple<int, int>>();
        }
        if (!points.ContainsKey(targetClass))
        {
            points[targetClass] = new HashSet<Tuple<int, int>>();
        }
        if (!points.ContainsKey(failClass))
        {
            points[failClass] = new HashSet<Tuple<int, int>>();
        }
         
        // temp set collection for recording who should be mapped to a new value
        HashSet<Tuple<int, int>> filtered = new HashSet<Tuple<int, int>>();
        HashSet<Tuple<int, int>> rejected = new HashSet<Tuple<int, int>>();

        // iteration to determine which points need to be filtered from set originClass to targetClass
        foreach (Tuple<int,int> point in points[originClass])
        {
            bool needsRemap = predicate(point.Item1, point.Item2);
            if (needsRemap)
            {
                filtered.Add(point);
            }
            else
            {
                rejected.Add(point);
            }
        }

        // remove mapped points from origin class
        points[originClass].ExceptWith(filtered);
        points[originClass].ExceptWith(rejected);
        // add matching points to target class
        points[targetClass].UnionWith(filtered);
        // add failed points to fail class
        points[failClass].UnionWith(rejected);
    }

    /// <summary>
    /// Gets all points in a class.
    /// </summary>
    /// <param name="originClass">Class to match points to</param>
    /// <returns>A collection of points which are of a class</returns>
    public HashSet<Tuple<int, int>> this[int originClass]
    {
        get
        {
            if (!points.ContainsKey(originClass))
            {
                points[originClass] = new HashSet<Tuple<int, int>>();
            }

            return (points[originClass]);
        }
    }

    /// <summary>
    /// If index i,j has a known class, gets it.
    /// Else return default value.
    /// </summary>
    /// <param name="i">First dimension map index</param>
    /// <param name="j">Second dimension map index</param>
    /// <returns>Class of index i,j if known.</returns>
    public int this[int i, int j]
    {
        get
        {
            foreach (KeyValuePair<int, HashSet<Tuple<int, int>>> kvp in points)
            {
                if (kvp.Value.Contains(new Tuple<int, int>(i, j)))
                {
                    return kvp.Key;
                }
            }

            return default(int);
        }
    }
}
