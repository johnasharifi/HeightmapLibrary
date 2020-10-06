using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

/// <summary>
/// A class for converting a heightmap into a chunked mesh
/// </summary>
public class BiomeToFloodfilledMesh : MonoBehaviour
{
    [SerializeField] private Heightmap map;
    
    // Start is called before the first frame update
    void Start()
    {
        MeshIteration();
    }
    
    void MeshIteration()
    {
        Dictionary<int, Mesh> meshes = new Dictionary<int, Mesh>();

        List<Vector3> verts0 = new List<Vector3>();
        List<int> tris0 = new List<int>();

        int maxdim = map.getMaxDim();

        // foreach (int biome in map.biomes)
        foreach (int biome in new[] { 1})
        {
            foreach (Tuple<int,int> point in map[biome])
            {
                if (!meshes.ContainsKey(biome) || meshes[biome] == null)
                {
                    meshes[biome] = new Mesh();
                }

                int lastInd = verts0.Count;

                Vector3 p = new Vector3(point.Item1, point.Item2, 0);

                verts0.AddRange(new Vector3[] { p + new Vector3(-0.5f, 0.5f, 0.0f), p + new Vector3(0.5f, 0.5f, 0.0f), p + new Vector3(0.5f, -0.5f, 0.0f), p + new Vector3(-0.5f, -0.5f, 0.0f) });
                tris0.AddRange(new int[] {lastInd + 0, lastInd + 1, lastInd + 2, lastInd + 0, lastInd + 2, lastInd + 3});
            }
        }

        Mesh m = new Mesh();

        m.SetVertices(verts0);
        m.SetTriangles(tris0.ToArray(), 0);

        m.RecalculateBounds();
        m.RecalculateNormals();
        m.RecalculateTangents();

        GetComponent<MeshFilter>().mesh = m;
    }
}
