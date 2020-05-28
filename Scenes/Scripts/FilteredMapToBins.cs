using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilteredMapToBins : MonoBehaviour
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
        var filter_lattice = MapFilterFactory.GetPerlinBand(dims, dims, 5.0f, 0.4f, 0.5f);
        var filter_plains = MapFilterFactory.GetPerlinBand(dims, dims, 5.0f, 0.4f, 0.5f);
        var filter_forests = MapFilterFactory.GetPerlinBand(dims, dims, 5.0f, 0.4f, 0.5f);
        var filter_mountains = MapFilterFactory.GetPerlinBand(dims, dims, 5.0f, 0.4f, 0.5f);

        map.MapFromTo(-1, 5, -1, filter_water);
        map.MapFromTo(-1, 10, 1, filter_lattice);
        map.MapFromTo(1, 0, 1, filter_exterior);
        
        map.MapFromTo(1, 7, filter_plains);
        map.MapFromTo(1, 8, filter_forests);
        map.MapFromTo(1, 9, filter_mountains);

        Texture2D tex = new Texture2D(dims, dims);
        for (int i = 0; i < dims; i++)
        {
            for (int j = 0; j < dims; j++)
            {
                int ind = map[i, j];
                if (ind == 0)
                {
                    tex.SetPixel(i, j, Color.black);
                }
                else if (ind == 5)
                {
                    tex.SetPixel(i, j, Color.blue);
                }
                else if (ind == 7)
                {
                    tex.SetPixel(i, j, Color.yellow);
                }
                else if (ind == 8)
                {
                    tex.SetPixel(i, j, Color.green);
                }
                else if (ind == 9)
                {
                    tex.SetPixel(i, j, Color.red);
                }
                else if (ind == 10)
                {
                    tex.SetPixel(i, j, Color.white);
                }
                else
                {
                    tex.SetPixel(i, j, Color.white);
                }

            }
        }
        tex.filterMode = FilterMode.Point;
        tex.Apply();

        rend.material.mainTexture = tex;
    }
    
}