using UnityEngine;

public static class FilterMapFactory
{
    public static System.Func<int, int, bool> GetPerlinFilter(int dim0, int dim1, float perlinScale, float threshold)
    {
        // params: dim2, perlinScale, threshold, xy offsets
        float xoff0 = Random.Range(0f, 1000f);
        float yoff0 = Random.Range(0f, 1000f);
        float xoff1 = Random.Range(0f, 1000f);
        float yoff1 = Random.Range(0f, 1000f);

        System.Func<int, int, bool> filter = (int i, int j) =>
        {
            float f0 = Mathf.PerlinNoise((i * 1.0f / dim0 + xoff0) * perlinScale, (j * 1.0f / dim1 + yoff0));
            float f1 = Mathf.PerlinNoise((i * 1.0f / dim0 + xoff1) * perlinScale, (j * 1.0f / dim1 + yoff1));

            return (f0 + f1) / 2 < threshold;
        };

        return (filter);
    }
}