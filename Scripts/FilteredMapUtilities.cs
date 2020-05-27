using UnityEngine;

public static class FilterMapFactory
{
    public static System.Func<int, int, bool> GetPerlinFilter(int dim0, int dim1, float perlinScale, float threshold)
    {
        System.Func<int, int, float> perlSurface = GetPerlinSurface(dim0, dim1, perlinScale);
        System.Func<int, int, bool> predicate = (int x, int y) =>
        {
            return perlSurface(x, y) < threshold;
        };

        return (predicate);
    }

    public static System.Func<int, int, bool> GetBlendedExteriorWeight(int dim0, int dim1, float threshold)
    {
        System.Func<int, int, float> perlin = GetPerlinSurface(dim0, dim1, 10.0f);

        System.Func<int, int, bool> rad = (int x, int y) =>
        {
            float f1 = perlin(x, y);
            //  f2 alone is perfect weight function
            float f2 = 1.0f - (Mathf.Pow(x - dim0 / 2, 2) + Mathf.Pow(y - dim1 / 2, 2)) * 2.0f / (dim0 * dim1);
            // blend slightly between f1 and f2 for a more organic result
            return (f1 + f2) * 0.5f < threshold;
        };
        return (rad);
    }

    private static System.Func<int, int, float> GetPerlinSurface(int dim0, int dim1, float perlinScale)
    {
        // params: dims, perlinScale, threshold, xy offsets
        float xoff0 = Random.Range(0f, 1000f);
        float yoff0 = Random.Range(0f, 1000f);
        float xoff1 = Random.Range(0f, 1000f);
        float yoff1 = Random.Range(0f, 1000f);

        System.Func<int, int, float> filter = (int i, int j) =>
        {
            return  0.5f * Mathf.PerlinNoise(i * perlinScale/ dim0 + xoff0, j * perlinScale / dim1 + yoff0) +
                    0.5f * Mathf.PerlinNoise(i * perlinScale / dim0 + xoff1, j * perlinScale / dim1 + yoff1);
        };

        return filter;
    }
}