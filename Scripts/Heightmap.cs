using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Heightmap
{
    /// <summary>
    /// Only returns values of sec for which values of prime are within specified range
    /// </summary>
    /// <param name="prime"></param>
    /// <param name="sec"></param>
    /// <returns></returns>
    public static float[,] AndPrimaryInBin(float[,] prime, float min, float max, float[,] sec)
    {
        int dim1 = prime.GetLength(0);
        int dim2 = prime.GetLength(1);
        
        float[,] h = new float[prime.GetLength(0), prime.GetLength(1)];

        for (int i = 0; i < dim1; i++)
        {
            for (int j = 0; j < dim2; j++)
            {
                h[i, j] = (min < prime[i, j] && prime[i, j] < max ? 1 : 0) * sec[i, j];
            }
        }

        return (h);
    }

    public static float[,] GetBlend(float[,] h1, float[,] h2, float alpha)
    {
        int dim1 = h1.GetLength(0);
        int dim2 = h1.GetLength(1);

        float[,] h3 = new float[dim1, dim2];

        for (int i = 0; i < dim1; i++)
        {
            for (int j = 0; j < dim2; j++)
            {
                h3[i, j] = Mathf.Lerp(h1[i,j], h2[i,j], alpha);
            }
        }

        return (h3);
    }

    public static float[,] GetExteriorWeight(int dim1, int dim2)
    {
        float[,] h = new float[dim1, dim2];

        System.Func<float, float, float> rad = (float x, float y) =>
        {
            return 1.0f - (Mathf.Pow(x - dim1/2, 2) + Mathf.Pow(y - dim2/2, 2)) * 2.0f /
                (dim1 * dim2);
        };
        
        for (int i = 0; i < dim1; i++)
        {
            for (int j = 0; j < dim2; j++)
            {
                h[i, j] = rad(i, j);
            }
        }

        return (h);
    }

    public static float[,] GetHeightmap(int dim1, int dim2, float perlinScale = 1.0f)
    {
        return GetHeightmap(dim1, dim2, new float[] {perlinScale});
    }

    public static float[,] GetHeightmap(int dim1, int dim2, params float[] perlinScales)
    {
        float[,] h = new float[dim1, dim2];

        float xoff0 = Random.value * 1000.0f;
        float yoff0 = Random.value * 1000.0f;
        float xoff1 = Random.value * 1000.0f;
        float yoff1 = Random.value * 1000.0f;

        System.Func<float, float, int, float> perl = (float x, float y, int scaleInd) =>
        {
            return 0.5f * Mathf.PerlinNoise((x + xoff0) * perlinScales[scaleInd] / dim1, (y + yoff0) * perlinScales[scaleInd] / dim2) +
                0.5f * Mathf.PerlinNoise((x + xoff1) * perlinScales[scaleInd] / dim1, (y + yoff1) * perlinScales[scaleInd] / dim2); 
        };

        for (int i = 0; i < dim1; i++)
        {
            for (int j = 0; j < dim2; j++)
            {
                float val = 0.0f;
                for (int k = 0; k < perlinScales.Length; k++)
                {
                    val += perl(i, j, k) / perlinScales.Length;
                }
                h[i, j] = val;
            }
        }

        return (h);
    }

    public static Texture2D GetTexture(float[,] heightmap)
    {
        int x = heightmap.GetLength(0);
        int y = heightmap.GetLength(1);

        Texture2D tex = new Texture2D(heightmap.GetLength(0), heightmap.GetLength(1));

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                tex.SetPixel(i, j, Color.Lerp(Color.red, Color.green, heightmap[i,j]));
            }
        }

        tex.Apply();

        return (tex);
    }

}
