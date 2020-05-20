using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class HeightmapWorld : MonoBehaviour
{
    [SerializeField] private Renderer rend;
    [SerializeField, Range(64, 256)] private int scale = 128;

    [SerializeField] private MapDoodad[] doodadLayers = new MapDoodad[0];

    // Start is called before the first frame update
    void Start()
    {
        if (rend == null) rend = GetComponent<Renderer>();

        GenerateMap();
    }

    void GenerateMap()
    {
        Heightmap h = new Heightmap(scale, scale, 10.0f);

        Heightmap rivers = new Heightmap(scale, scale, 1.0f);
        bool[,] isRiver = Heightmap.AND(0.43f < rivers, rivers < 0.45f);
        
        Heightmap exterior = new Heightmap(Heightmap.GetExteriorWeight(scale, scale));
        Heightmap interior = new Heightmap(scale, scale, 10.0f);
        float[,] exteriorAlpha = Heightmap.GetExteriorWeight(scale, scale);
        Heightmap combined_radial = Heightmap.Blend(exterior, interior, exteriorAlpha);
        bool[,] isExterior = combined_radial > 0.5f;

        Heightmap lattice = new Heightmap(scale, scale, 10.0f);
        bool[,] isLattice = Heightmap.AND(0.40f < lattice, lattice < 0.50f);

        Heightmap binForest = new Heightmap(scale, scale, 10);
        Heightmap binPlains = new Heightmap(scale, scale, 1);
        Heightmap binMountain = new Heightmap(scale, scale, 2);

        Heightmap minerals = new Heightmap(scale, scale, 10.0f);
        bool[,] isForest = Heightmap.AND(Heightmap.AND(0.0f < minerals, minerals < 0.5f), Heightmap.AND(0.4f < binForest, binForest < 0.5f));
        bool[,] isPlains = Heightmap.AND(Heightmap.AND(0.4f < minerals, minerals < 0.6f), Heightmap.AND(0.4f < binPlains, binPlains < 0.5f));
        bool[,] isMountain = Heightmap.AND(Heightmap.AND(0.4f < minerals, minerals < 0.6f), Heightmap.AND(0.4f < binMountain, binMountain < 0.5f));
        
        Texture2D tex = (Texture2D)h;

        System.Action<int, int, string> SpawnDoodad = new System.Action<int, int, string>( (int i, int j, string biome) => 
        {
            GameObject go = new GameObject(string.Format("Doodad b{2}\t{0} x {1}", i, j, biome));
            go.transform.localPosition = new Vector3(transform.position.x - i + scale / 2 - 0.5f, 0, transform.position.z - j + scale / 2 - 0.5f);
            go.transform.parent = transform;
            BoxCollider c = go.AddComponent<BoxCollider>();
            MapDoodadColliderFlag flag = go.AddComponent<MapDoodadColliderFlag>();
            if (biome == "forest")
                flag.m_type = MapDoodadColliderFlag.MapDoodadType.FOREST;
            else if (biome == "plains")
                flag.m_type = MapDoodadColliderFlag.MapDoodadType.PLAINS;
        });

        for (int i = 0; i < scale; i++)
        {
            for (int j = 0; j < scale; j++)
            {
                if (isRiver[i, j])
                {
                    tex.SetPixel(i, j, doodadLayers[0].color);
                }
                else if (isLattice[i, j])
                {
                    tex.SetPixel(i, j, doodadLayers[1].color);
                }
                else if (isExterior[i, j])
                {
                    tex.SetPixel(i, j, doodadLayers[2].color);
                }
                else if (isMountain[i, j])
                {
                    tex.SetPixel(i, j, doodadLayers[3].color);
                }
                else if (isForest[i, j])
                {
                    tex.SetPixel(i, j, doodadLayers[4].color);
                    SpawnDoodad(i, j, "forest");
                }
                else if (isPlains[i, j])
                {
                    tex.SetPixel(i, j, doodadLayers[5].color);
                    SpawnDoodad(i, j, "plains");
                }
                else
                {
                    tex.SetPixel(i, j, doodadLayers[6].color);
                }

            }
        }
        tex.filterMode = FilterMode.Point;
        tex.Apply();

        rend.material.mainTexture = tex;
    }
}

[System.Serializable]
public class MapDoodad
{
    [SerializeField] public string name;
    [SerializeField] public Color color;
    [SerializeField] public int layer;
}