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
        FilteredMap map = new FilteredMap(dims, dims);

        var filter1 = FilterMapFactory.GetPerlinFilter(dims, dims, 10.0f, 0.5f);
        var filter2 = FilterMapFactory.GetPerlinFilter(dims, dims, 10.0f, 0.5f);
        var filter3 = FilterMapFactory.GetBlendedExteriorWeight(dims, dims, 0.5f);
        var filter4 = FilterMapFactory.GetPerlinSurface(dims, dims, 10.0f);

        map.MapFromTo(-1, 0, 1, filter3);

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