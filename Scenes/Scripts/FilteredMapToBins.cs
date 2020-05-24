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

        var perlinMap = FilterMapFactory.GetPerlinFilter(dims, dims, 1.0f, 0.5f);
        var perlinMap2 = FilterMapFactory.GetPerlinFilter(dims, dims, 10.0f, 0.5f);

        map.MapFromTo(-1, 0, 1, perlinMap);
        map.MapFromTo(0, 2, 3, perlinMap2);

        Texture2D tex = new Texture2D(dims, dims);
        for (int i = 0; i < dims; i++)
        {
            for (int j = 0; j < dims; j++)
            {
                int ind = map[i, j];
                if (ind == 0)
                    tex.SetPixel(i, j, Color.red);
                else if (ind == 1)
                    tex.SetPixel(i, j, Color.black);
                else if (ind == 2)
                    tex.SetPixel(i, j, Color.blue);
                else if (ind == 3)
                    tex.SetPixel(i, j, Color.magenta);
                else
                {
                    Debug.LogFormat("ind at position {0}, {1} is {2}", i, j, ind);
                    tex.SetPixel(i, j, Color.white);
                }
                    
            }
        }
        tex.filterMode = FilterMode.Point;
        tex.Apply();

        rend.material.mainTexture = tex;
    }
    
}