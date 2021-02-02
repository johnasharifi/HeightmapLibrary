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

    private const int consolidationMaxSize = 8;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="initPoint"></param>
    /// <param name="group"></param>
    /// <param name="visited">Modifies this array</param>
    /// <returns></returns>
    static Tuple<int,int> ConsolidateFrom(Tuple<int,int> initPoint, HashSet<Tuple<int,int>> group, bool[,] visited)
    {
        // from init point, find a large chunk of points which all have same type and are visitable
        // this is cause for consolidating those points into same point

        // local helper function for calculating whether an individual point is visitable
        Func<Tuple<int,int>, bool> isPointVisitable = (Tuple<int, int> elem) => 
        {
            return group.Contains(elem) && elem.Item1 <= visited.GetLength(0) && elem.Item2 < visited.GetLength(1) && !visited[elem.Item1, elem.Item2];
        };

        // bubble up dim0 / col count until we hit a limit
        int colSpan = 1;
        while (isPointVisitable(new Tuple<int, int>(initPoint.Item1 + colSpan, initPoint.Item2)) && colSpan < consolidationMaxSize)
        {
            colSpan++;
        }

        int rowSpan = 1;

        // update step: modify the array and mark our span as having been visited
        for (int i = 0; i < colSpan; i++)
        {
            for (int j = 0; j < rowSpan; j++)
            {
                visited[initPoint.Item1 + i, initPoint.Item2 + j] = true;
            }
        }

        // return the terminal Tuple that pairs with the initial Tuple to form a rect
        return new Tuple<int, int>(initPoint.Item1 + Mathf.Max(0, colSpan - 1), initPoint.Item2 + Mathf.Max(0, rowSpan - 1));
    }
    
    public static Dictionary<int, Mesh> GetMeshesFromHeightmap(Heightmap heightmapWithBiomes)
    {
        // shared vars
        Dictionary<int, Mesh> meshes = new Dictionary<int, Mesh>();
        int maxdim = heightmapWithBiomes.getMaxDim();
        const int vertGroupCount = 4;

        foreach (int biome in heightmapWithBiomes.biomes)
        {
            // per-mesh vars
            Mesh m = new Mesh();
            List<Vector3> verts0 = new List<Vector3>();
            List<int> tris0 = new List<int>();
            
            bool[,] visitedForConsolidation = new bool[heightmapWithBiomes.getDim(0), heightmapWithBiomes.getDim(0)];
            float verticalOffset = -1f * heightmapWithBiomes.biomeVerticalOffsetTable[biome];
            bool isVerticallyOffset = !Mathf.Approximately(verticalOffset, 0.0f);

            foreach (Tuple<int,int> point in heightmapWithBiomes[biome])
            {
                if (!meshes.ContainsKey(biome) || meshes[biome] == null)
                {
                    meshes[biome] = new Mesh();
                }

                if (visitedForConsolidation[point.Item1, point.Item2])
                {
                    // case: already in another bin. skip
                    continue;
                }
                
                Tuple<int, int> spanPoint = spanPoint = ConsolidateFrom(point, heightmapWithBiomes[biome], visitedForConsolidation);
                
                int lastInd = verts0.Count;

                Vector3 pOrig = new Vector3(point.Item1, point.Item2, 0);
                Vector3 pTerm = new Vector3(spanPoint.Item1, spanPoint.Item2, 0);

                verts0.AddRange(new Vector3[] {
                    pOrig + new Vector3(-0.5f, -0.5f, verticalOffset),
                    new Vector3(pOrig.x, pTerm.y, 0.0f) + new Vector3(-0.5f, 0.5f, verticalOffset),
                    pTerm + new Vector3(0.5f, 0.5f, verticalOffset) ,
                    new Vector3(pTerm.x, pOrig.y, 0.0f) + new Vector3(0.5f, -0.5f, verticalOffset) }
                );
                tris0.AddRange(new int[] {lastInd + 0, lastInd + 1, lastInd + 2, lastInd + 0, lastInd + 2, lastInd + 3});
            }

            // additional step: when vertically offset, duplicate our verts and create a rect that spans from (xy, k) to (xy, 0)
            if (isVerticallyOffset)
            {
                // acquire vert len now, iterate through the original verts only
                int vertsInitialCount = verts0.Count;
                for (int i = 0; i < vertsInitialCount - vertGroupCount; i = i + vertGroupCount)
                {
                    // for left face we will always want to add a rect which has verts at i[0], i[1] to i[0] - vertOffset i[1] - vertOffset
                    Vector3 ind0 = verts0[i];
                    Vector3 ind1 = verts0[i + 1];
                    Vector3 ind0Flattened = new Vector3(ind0.x, ind0.y, 0.0f);
                    Vector3 ind1Flattened = new Vector3(ind1.x, ind1.y, 0.0f);

                    int lastInd = verts0.Count; // cache ind before adding, so that our addition math is correct
                    verts0.AddRange(new Vector3[] {
                        ind1Flattened,
                        ind1,
                        ind0,
                        ind0Flattened,
                    });
                    tris0.AddRange(new int[] { lastInd + 0, lastInd + 1, lastInd + 2, lastInd + 0, lastInd + 2, lastInd + 3 });
                    
                    // for right face we will want to add rect which has verts at i[2], i[3] to i[2] - vertOffset, i[3] - vertOffset
                    Vector3 ind2 = verts0[i + 2];
                    Vector3 ind3 = verts0[i + 3];
                    Vector3 ind2Flattened = new Vector3(ind2.x, ind2.y, 0.0f);
                    Vector3 ind3Flattened = new Vector3(ind3.x, ind3.y, 0.0f);
                    
                    lastInd = verts0.Count; // cache ind before adding, so that our addition math is correct
                    verts0.AddRange(new Vector3[] {
                        ind3Flattened,
                        ind3,
                        ind2,
                        ind2Flattened,
                    });
                    tris0.AddRange(new int[] { lastInd + 0, lastInd + 1, lastInd + 2, lastInd + 0, lastInd + 2, lastInd + 3 });
                }
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
