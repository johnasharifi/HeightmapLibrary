using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ad-hoc class for converting a float[,] array to a texture
/// </summary>
public class HeightmapToTexture : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        const int dim = 128;
        
        float[,] height = Heightmap.GetHeightmap(dim, dim, 10.0f);
        float[,] rivers = Heightmap.GetHeightmap(dim, dim, 2.0f);
        float[,] fertility = Heightmap.AndPrimaryInBin(rivers, 0.4f, 0.5f, rivers);
        float[,] paths = Heightmap.GetHeightmap(dim, dim, 10.0f);

        float[,] rad1 = Heightmap.GetExteriorWeight(dim, dim);
        float[,] boundary = Heightmap.GetBlend(height, rad1, 0.3f);

        float[,] lowfreq = Heightmap.GetHeightmap(dim, dim, 5.0f);
        float[,] highfreq = Heightmap.GetHeightmap(dim, dim, 20.0f);
        float[,] composite = Heightmap.GetBlend(lowfreq, highfreq, 0.3f);

        float[,] resources = Heightmap.GetHeightmap(dim, dim, 10.0f);

        Texture2D tex = new Texture2D(dim, dim);
        
        for (int i = 0; i < dim; i++)
        {
            for (int j = 0; j < dim; j++)
            {
                if (0.4f < rivers[i, j] && rivers[i, j] < 0.45f)
                    tex.SetPixel(i, j, Color.blue);
                else if (0.4f < paths[i, j] && paths[i, j] < 0.5f)
                    tex.SetPixel(i, j, Color.white);
                else if (0.0f < fertility[i,j] && fertility[i,j] < 1.0f)
                    tex.SetPixel(i, j, Color.green);
                else if (boundary[i, j] < 0.5f)
                    tex.SetPixel(i, j, Color.black);
                else if (0.5f < composite[i, j] && composite[i, j] < 0.9f)
                    tex.SetPixel(i, j, Color.red);
                else if (0.5f < resources[i, j] && resources[i, j] < 0.6f)
                    tex.SetPixel(i, j, Color.yellow);
                else
                    tex.SetPixel(i, j, Color.white);
            }
        }

        tex.filterMode = FilterMode.Point;
        tex.Apply();

        GetComponent<Renderer>().material.mainTexture = tex;

        Heightmap h = new Heightmap(100, 100, 10);
        Texture2D tex2 = h;
        GetComponent<Renderer>().material.mainTexture = tex2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
