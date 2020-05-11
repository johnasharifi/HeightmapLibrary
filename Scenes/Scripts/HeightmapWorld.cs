using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class HeightmapWorld : MonoBehaviour
{
    [SerializeField] private Renderer rend;
    [SerializeField, Range(64, 256)] private int scale = 128;

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

        Heightmap minerals = new Heightmap(scale, scale, 10.0f);
        bool[,] isForest = Heightmap.AND(0.3f < minerals, minerals < 0.4f);
        bool[,] isPlains = Heightmap.AND(0.5f < minerals, interior < 0.6f);
        bool[,] isMountain = Heightmap.AND(0.4f < interior, interior < 0.5f);

        Texture2D tex = (Texture2D)h;

        for (int i = 0; i < scale; i++)
        {
            for (int j = 0; j < scale; j++)
            {
                if (isRiver[i, j])
                    tex.SetPixel(i, j, Color.blue);
                else if (isLattice[i, j])
                    tex.SetPixel(i, j, Color.white);
                else if (isExterior[i, j])
                    tex.SetPixel(i, j, Color.red);
                else if (isMountain[i, j])
                    tex.SetPixel(i, j, Color.grey);
                else if (isForest[i, j])
                    tex.SetPixel(i, j, Color.green);
                else if (isPlains[i, j])
                    tex.SetPixel(i, j, Color.yellow);
                else
                    tex.SetPixel(i, j, Color.white);
            }
        }
        tex.filterMode = FilterMode.Point;
        tex.Apply();

        rend.material.mainTexture = tex;


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
