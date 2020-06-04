﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightmapToTexture : MonoBehaviour
{
    [SerializeField] private Renderer rend;

    [Range(64, 512)]
    [SerializeField] private int dims;

    // Start is called before the first frame update
    void Start()
    {
        Heightmap map = new Heightmap(dims, dims);

        var filter_water = MapFilterFactory.GetPerlinBand(dims, dims, 1.0f, 0.4f, 0.43f);
        var filter_exterior = MapFilterFactory.GetBlendedExteriorWeight(dims, dims, 0.5f);
        var filter_lattice = MapFilterFactory.GetPerlinBand(dims, dims, 10.0f, 0.3f, 0.4f);
        var filter_mountains = MapFilterFactory.GetPerlinBand(dims, dims, 5.0f, 0.4f, 0.5f);
        var filter_forests = MapFilterFactory.GetPerlinBand(dims, dims, 10.0f, 0.4f, 0.5f);
        var filter_plains = MapFilterFactory.GetPerlinBand(dims, dims, 5.0f, 0.4f, 0.5f);

        map.MapFromTo(-1, 5, -1, filter_water);
        map.MapFromTo(-1, 10, 1, filter_lattice);
        map.MapFromTo(1, 0, 1, filter_exterior);

        map.MapFromTo(1, 9, filter_mountains);
        map.MapFromTo(1, 8, filter_forests);
        map.MapFromTo(1, 7, filter_plains);
        
        Dictionary<int, Color> mapping = new Dictionary<int, Color>()
        {
            { 0, Color.red },
            { 5, Color.blue },
            { 7, Color.yellow },
            { 8, Color.green},
            { 9, Color.red},
            { 10, Color.white},
            { -1, Color.white },
            {1, Color.white }
        };
        HeightmapColorLookupTable lut = new HeightmapColorLookupTable(mapping);

        Texture2D tex = map.AsTexture2D(lut);
        tex.filterMode = FilterMode.Point;
        tex.Apply();

        rend.material.mainTexture = tex;
    }
    
}