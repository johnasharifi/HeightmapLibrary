using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Heightmap : MonoBehaviour
{
    [SerializeField, Range(64, 512)] private int dim1;
    [SerializeField, Range(64, 512)] private int dim2;

    private Renderer rend;
    
    [SerializeField] private HeightmapBiomeFilterLookupTable m_biomeFilterLookupTable = new HeightmapBiomeFilterLookupTable();
    [SerializeField] private HeightmapColorLookupTable m_colorLookupTable = new HeightmapColorLookupTable();
    [SerializeField] private HeightmapSpeedLookupTable m_speedLookupTable = new HeightmapSpeedLookupTable();

    public delegate void BiomesGenerated(int biome, int x, int z, Color c);
    public event BiomesGenerated onBiomesGenerated;

    /// <summary>
    /// Gets dimension of the map.
    /// </summary>
    /// <param name="dimension"></param>
    /// <returns></returns>
    public int getDim(int dimension)
    {
        if (dimension == 0)
        {
            return dim1;
        }
        // easier to expand to n dimensions in future
        return dim2;
    }

    /// <summary>
    /// Gets max dimension of map.
    /// </summary>
    /// <returns>Max of dim1, dim2 args which are provided on construction.</returns>
    public int getMaxDim()
    {
        return Mathf.Max(dim1, dim2);
    }

    /// <summary>
    /// Getter for a speed lookup table.
    /// </summary>
    public HeightmapSpeedLookupTable speedLookupTable
    {
        get
        {
            return m_speedLookupTable;
        }
    }

    public HeightmapColorLookupTable colorLookupTable
    {
        get
        {
            return m_colorLookupTable;
        }
    }

    public IEnumerable<int> biomes
    {
        get
        {
            return points.Keys;
        }
    }

    private Dictionary<int, HashSet<Tuple<int, int>>> points = new Dictionary<int, HashSet<Tuple<int, int>>>();
    
    private void OnEnable()
    {
        points[-1] = new HashSet<Tuple<int, int>>();

        for (int i = 0; i < dim1; i++)
        {
            for (int j = 0; j < dim2; j++)
            {
                points[-1].Add(new Tuple<int, int>(i, j));
            }
        }

        foreach (HeightmapBiomeFilter filter in m_biomeFilterLookupTable)
        {
            if (HeightmapBiomeFilter.IsBlendedExterior(filter.predicateType)) {
                Func<int, int, bool> mapFunc = MapFilterFactory.GetBlendedExteriorWeight(dim1, dim2, filter.predicateThresholdA);

                this.MapFromTo(filter.originClass, filter.targetClass, filter.failClass, mapFunc);
            }
            else
            {
                Func<int, int, bool> mapFunc = MapFilterFactory.GetPerlinBand(dim1, dim2, filter.predicatePerlinScale, filter.predicateThresholdA, filter.predicateThresholdB);
                this.MapFromTo(filter.originClass, filter.targetClass, filter.failClass, mapFunc);
            }
        }
        
        for (int i = 0; i < dim1; i++)
        {
            for (int j = 0; j < dim2; j++)
            {
                int biome = this[i, j];
                Color color = m_colorLookupTable[biome];
                onBiomesGenerated?.Invoke(biome, i, j, color);
            }
        }

        // DrawMapOnRenderer();
    }

    public void ApplyFunctionTo(int originClass, Action<int, int> function)
    {
        foreach (Tuple<int, int> point in points[originClass])
        {
            function(point.Item1, point.Item2);
        }
    }

    private MapPathfinder pathfinder;
    public List<Tuple<int,int>> PathFromTo(Tuple<int,int> origin, Tuple<int,int> target)
    {
        if (pathfinder == null)
        {
            pathfinder = new MapPathfinder(this);
        }

        return pathfinder.GetFastApproximateFullPathFrom(origin, target);
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

    /// <summary>
    /// Convert to Texture2D using parameter - mapping
    /// </summary>
    /// <param name="mapping">Specifies a mapping from integer to Color</param>
    /// <returns>A Texture2D on GPU</returns>
    [Obsolete("Heightmap component should have a color table")]
    public Texture2D AsTexture2D(HeightmapColorLookupTable mapping)
    {
        Texture2D tex = new Texture2D(dim1, dim2);
        Color[] colors = new Color[dim1 * dim2];
        
        foreach (KeyValuePair<int, HashSet<Tuple<int,int>>> kvp in points)
        {
            foreach (Tuple<int,int> point in kvp.Value)
            {
                colors[point.Item2 * dim1 + point.Item1] = mapping[kvp.Key];
            }
        }

        tex.SetPixels(colors);
        tex.filterMode = FilterMode.Point;
        tex.Apply();

        if (rend == null)
        {
            rend = GetComponent<Renderer>();
        }
        if (rend != null)
        {
            rend.material.mainTexture = tex;
        }

        return tex;
    }

    /// <summary>
    /// Given heightmap color lookup table,
    /// 
    /// Creates a Texture2D from the heightmap's height and heightmap color lookup table.
    /// 
    /// Sets rend's main texture to be the heightmap's texture.
    /// </summary>
    public void DrawMapOnRenderer()
    {
        Texture2D tex = new Texture2D(dim1, dim2);
        Color[] colors = new Color[dim1 * dim2];

        foreach (KeyValuePair<int, HashSet<Tuple<int, int>>> kvp in points)
        {
            foreach (Tuple<int, int> point in kvp.Value)
            {
                colors[point.Item2 * dim1 + point.Item1] = m_colorLookupTable[kvp.Key];
            }
        }

        tex.SetPixels(colors);
        tex.filterMode = FilterMode.Point;
        tex.Apply();

        if (rend == null)
        {
            rend = GetComponent<Renderer>();
        }
        if (rend != null)
        {
            rend.material.mainTexture = tex;
        }
    }
}
