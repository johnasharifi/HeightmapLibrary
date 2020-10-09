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
        Dictionary<int, Mesh> meshes = GetMeshesFromHeightmap(map);
        GetComponent<MeshFilter>().mesh = meshes[1];
    }
    
    public static Dictionary<int, Mesh> GetMeshesFromHeightmap(Heightmap heightmapWithBiomes)
    {
        // shared vars
        Dictionary<int, Mesh> meshes = new Dictionary<int, Mesh>();
        int maxdim = heightmapWithBiomes.getMaxDim();

        foreach (int biome in heightmapWithBiomes.biomes)
        {
            // per-mesh vars
            Mesh m = new Mesh();
            List<Vector3> verts0 = new List<Vector3>();
            List<int> tris0 = new List<int>();

            foreach (Tuple<int,int> point in heightmapWithBiomes[biome])
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

            m.SetVertices(verts0);
            m.SetTriangles(tris0.ToArray(), 0);

            m.RecalculateBounds();
            m.RecalculateNormals();
            m.RecalculateTangents();

            meshes[biome] = m;
        }

        return meshes;
    }
}
