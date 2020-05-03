using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heightmap
{
    private float[,] surface;

    /// <summary>
    /// Creates a heightmap whose surface should be manually allocated. 
    /// This constructor is used by Heightmap operators that have return values.
    /// </summary>
    private Heightmap()
    {
    }
    
    public Heightmap(int dim1, int dim2, float perlinScale = 1.0f)
    {
        dim1 = Mathf.Max(1, dim1);
        dim2 = Mathf.Max(1, dim2);

        surface = new float[dim1, dim2];
        
        float xoff0 = Random.Range(0.0f, 1000.0f);
        float xoff1 = Random.Range(0.0f, 1000.0f);
        float yoff0 = Random.Range(0.0f, 1000.0f);
        float yoff1 = Random.Range(0.0f, 1000.0f);

        // params of this function: perl offsets, scale
        // variables: x coord, y coord
        System.Func<int, int, float> perl = (int x, int y) =>
        {
            return 0.5f * Mathf.PerlinNoise((x + xoff0) * perlinScale / dim1, (y + yoff0) * perlinScale/ dim2) +
                0.5f * Mathf.PerlinNoise((x + xoff1) * perlinScale / dim1, (y + yoff1) * perlinScale / dim2);
        };

        for (int i = 0; i < dim1; i++)
        {
            for (int j = 0; j < dim2; j++)
            {
                surface[i, j] = perl(i, j);
            }
        }
    }

    private int dim1
    {
        get { return surface.GetLength(0); }
    }

    private int dim2
    {
        get { return surface.GetLength(1); }
    }

    /// <summary>
    /// Private internal accessor using integers. 
    /// </summary>
    /// <param name="ind1">Index 1 into surface</param>
    /// <param name="ind2">Index 2 into surface</param>
    /// <returns></returns>
    private float this[int ind1, int ind2]
    {
        get
        {
            return surface[Mathf.Clamp(ind1, 0, dim1 - 1), Mathf.Clamp(ind2, 0, dim2 - 1)];
        }
    }

    public static implicit operator Texture2D (Heightmap h)
    {
        Texture2D tex = new Texture2D(h.dim1, h.dim2);
        tex.filterMode = FilterMode.Point;

        for (int i = 0; i < h.dim1; i++)
        {
            for (int j = 0; j < h.dim2; j++)
            {
                tex.SetPixel(i, j, Color.Lerp(Color.white, Color.black, h[i, j]));
            }
        }

        tex.Apply();
        return (tex);
    }

    /// <summary>
    /// Defines Heightmap * Heightmap operation
    /// </summary>
    /// <param name="orig">First heightmap arg</param>
    /// <param name="other">Second heightmap arg</param>
    /// <returns>A new heightmap which has values far from 0.5 where orig or other had values far from 0.5</returns>
    public static Heightmap operator * (Heightmap orig, Heightmap other)
    {
        Heightmap result = new Heightmap();
        result.surface = new float[orig.dim1, orig.dim2];

        for (int i = 0; i < orig.dim1; i++)
        {
            for (int j = 0; j < orig.dim2; j++)
            {
                float f1 = (orig[i, j] - 0.5f) * 2; // transform from range [0, 1] into [-1, 1]
                float f2 = (other[i, j] - 0.5f) * 2; // transform from range [0, 1] into [-1, 1]
                // introduce eccentricity; (f1 + f2) / 2 shifts distribution toward mean
                // float eccentric = Mathf.Sign(f1 * f2) * Mathf.Pow(Mathf.Abs(f1 * f2), 0.5f);
                float eccentric = Mathf.Sign(f1 * f2) * Mathf.Pow(Mathf.Abs(f1 * f2), 0.5f);
                result.surface[i, j] = Mathf.Clamp01( (eccentric) * 0.5f + 0.5f);
            }
        }

        return (result);
    }

    /// <summary>
    /// Compares each member in surface array to v.
    /// </summary>
    /// <param name="orig">A heightmap with values in an array</param>
    /// <param name="v">A float to compare heightmap values to</param>
    /// <returns></returns>
    public static bool[,] operator < (Heightmap orig, float v)
    {
        bool[,] orig_less_than_v = new bool[orig.dim1, orig.dim2];
        for (int i = 0; i < orig.dim1; i++)
        {
            for (int j = 0; j < orig.dim2; j++)
            {
                orig_less_than_v[i, j] = orig[i, j] < v;
            }
        }
        return (orig_less_than_v);
    }

    public static bool[,] operator <(float v, Heightmap orig)
    {
        // v < orig 
        // has equivalent signs as
        // orig > v
        return orig > v;
    }

    public static bool[,] operator >(Heightmap orig, float v)
    {
        bool[,] orig_greater_than_v = new bool[orig.dim1, orig.dim2];
        for (int i = 0; i < orig.dim1; i++)
        {
            for (int j = 0; j < orig.dim2; j++)
            {
                orig_greater_than_v[i, j] = orig[i, j] > v;
            }
        }
        return (orig_greater_than_v);
    }

    public static bool[,] operator >(float v, Heightmap orig)
    {
        // v > orig
        // has equivalent signs as
        // orig < v
        return orig < v;
    }

    /// <summary>
    /// Zeroes out values in orig when flag is false
    /// </summary>
    /// <param name="flags">Array of boolean flags. Flagij == false makes heightmap value in same position zero</param>
    /// <param name="orig"></param>
    /// <returns></returns>
    public static Heightmap operator &(bool[,] flags, Heightmap orig)
    {
        Heightmap h = new Heightmap();
        h.surface = new float[orig.dim1, orig.dim2];

        for (int i = 0; i < orig.dim1; i++)
        {
            for (int j = 0; j < orig.dim2; j++)
            {
                h.surface[i, j] = (flags[i, j]? 1:0) * orig[i, j];
            }
        }

        return (h);
    }

    public static Heightmap operator &(Heightmap orig, bool[,] flags)
    {
        return flags & orig;
    }

    /// -------------
    /// static members; TODO convert into oo



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
