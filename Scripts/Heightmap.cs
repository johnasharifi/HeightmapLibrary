using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
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

    public Heightmap(float[,] elements)
    {
        surface = new float[elements.GetLength(0), elements.GetLength(1)];
        System.Array.Copy(elements, surface, dim1 * dim2);
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
    public float this[int ind1, int ind2]
    {
        get
        {
            return surface[Mathf.Clamp(ind1, 0, dim1 - 1), Mathf.Clamp(ind2, 0, dim2 - 1)];
        }
    }

    /// <summary>
    /// Heightmaps support retrieval of contained float data.
    /// Useful for retrieving an array of alpha data in range [0 - 1]
    /// </summary>
    /// <param name="h">A heightmap which contains float[,] data</param>
    public static explicit operator float[,] (Heightmap h)
    {
        float[,] data = new float[h.dim1, h.dim2];
        System.Array.Copy(h.surface, data, h.dim1 * h.dim2);
        return (data);
    }

    /// <summary>
    /// Heightmaps support converesion into Texture2Ds.
    /// </summary>
    /// <param name="h">A heightmap to convert to grayscale bmp</param>
    public static explicit operator Texture2D (Heightmap h)
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

    public static Heightmap operator *(Heightmap other, float[,] scale)
    {
        Heightmap h = new Heightmap();

        h.surface = new float[other.dim1, other.dim2];

        for (int i = 0; i < h.dim1; i++)
        {
            for (int j = 0; j < h.dim2; j++)
            {
                h.surface[i, j] = h.surface[i, j] * scale[i, j];
            }
        }
        
        return (h);
    }

    public static Heightmap operator *(float[,] scale, Heightmap other)
    {
        return other * scale;
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
                h[i, j] = 1.0f - rad(i, j);
            }
        }

        return (h);
    }
    
    public static Heightmap Blend(Heightmap h1, Heightmap h2, float[,] alpha)
    {
        Heightmap h = new Heightmap(h1.dim1, h1.dim2);

        for (int i = 0; i < h.dim1; i++)
        {
            for (int j = 0; j < h.dim2; j++)
            {
                h.surface[i, j] = Mathf.Lerp(h1[i,j], h2[i,j], alpha[i,j]);
            }
        }

        return (h);
    }

    public static bool[,] AND(bool[,] flag1, bool[,] flag2)
    {
        bool[,] data = new bool[flag1.GetLength(0), flag1.GetLength(1)];

        for (int i = 0; i < flag1.GetLength(0); i++)
        {
            for (int j = 0; j < flag2.GetLength(1); j++)
            {
                data[i, j] = flag1[i, j] && flag2[i, j];
            }
        }

        return (data);
    }
}
