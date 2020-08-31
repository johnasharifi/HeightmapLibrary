using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Point = System.Tuple<int, int>;

[RequireComponent(typeof(LineRenderer))]
public class PathfindingTotem : MonoBehaviour
{
    [SerializeField] private Renderer rend;
    [SerializeField] private LineRenderer lrend;
    [SerializeField] private Transform target;
    [SerializeField] private Heightmap map;

    [Range(64, 512)]
    [SerializeField] private int dims;

    public Heightmap GetMap { get { return map; } }

    [SerializeField] HeightmapColorLookupTable lut;
    public HeightmapColorLookupTable GetLut { get { return lut; } }

    // Start is called before the first frame update
    void Start()
    {
        map = new Heightmap(dims, dims);
        var filter_water = MapFilterFactory.GetPerlinBand(dims, dims, 1.0f, 0.4f, 0.43f);
        var filter_exterior = MapFilterFactory.GetBlendedExteriorWeight(dims, dims, 0.5f);
        var filter_lattice = MapFilterFactory.GetPerlinBand(dims, dims, 15.0f, 0.3f, 0.4f);
        var filter_mountains = MapFilterFactory.GetPerlinBand(dims, dims, 5.0f, 0.4f, 0.5f);
        var filter_forests = MapFilterFactory.GetPerlinBand(dims, dims, 10.0f, 0.4f, 0.5f);
        var filter_plains = MapFilterFactory.GetPerlinBand(dims, dims, 5.0f, 0.4f, 0.5f);

        map.MapFromTo(-1, 5, -1, filter_water);
        map.MapFromTo(-1, 10, 1, filter_lattice);
        map.MapFromTo(1, 0, 1, filter_exterior);

        map.MapFromTo(1, 9, filter_mountains);
        map.MapFromTo(1, 8, filter_forests);
        map.MapFromTo(1, 7, filter_plains);

        HeightmapSpeedLookupTable speedTable = new HeightmapSpeedLookupTable
        {
            {0, 0.1f },
            {5, 0.1f },
            {7, 1f },
            {8, 0.5f },
            {9, 0.1f },
            {10, 1f },
            {-1, 0.001f },
            {1, 1f }
        };

        map.speedTable = speedTable;

        Texture2D tex = map.AsTexture2D(lut);
        tex.filterMode = FilterMode.Point;
        tex.Apply();

        rend.material.mainTexture = tex;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            Point origxz = new Point(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));
            Point targetxz = new Point(Mathf.FloorToInt(target.position.x), Mathf.FloorToInt(target.position.z));

            List<Point> path = map.PathFromTo(origxz, targetxz);

            if (path == null || path.Count == 0)
            {
                lrend.enabled = false;
            }
            else
            {
                lrend.enabled = true;
                Vector3[] points = new Vector3[path.Count];
                for (int i = 0; i < path.Count; i++)
                {
                    points[i] = new Vector3(path[i].Item1, 0f, path[i].Item2);
                }
                lrend.positionCount = path.Count;
                lrend.SetPositions(points);
            }
            
        }
    }
}
