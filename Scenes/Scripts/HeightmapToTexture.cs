using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ad-hoc class for converting a float[,] array to a texture
/// </summary>
public class HeightmapToTexture : MonoBehaviour
{
    [SerializeField] private MeshRenderer mr1;
    [SerializeField] private MeshRenderer mr2;
    [SerializeField] private MeshRenderer mr3;

    // Start is called before the first frame update
    void Start()
    {
        const int dim = 128;

        Heightmap rivers = new Heightmap(dim, dim, 1.0f);

        Heightmap surface1 = new Heightmap(dim, dim, 2.0f);
        float[,] surface_alpha = (float[,])new Heightmap(dim, dim);
        Heightmap surface3 = Heightmap.Blend(surface1, rivers, surface_alpha);
        
        rivers = rivers & (0.40f < rivers) & (rivers < 0.45f);
        Texture2D tex = (Texture2D)rivers;
        mr1.material.mainTexture = tex;

        Heightmap exterior = new Heightmap(Heightmap.GetExteriorWeight(dim, dim));
        Heightmap interior = new Heightmap(dim, dim, 10.0f);
        float[,] alpha = Heightmap.GetExteriorWeight(dim, dim);
        Heightmap combined_radial = Heightmap.Blend(exterior, interior, alpha);

        Heightmap lattice = new Heightmap(dim, dim, 10.0f);
        lattice = lattice & (0.4f < lattice) & (lattice < 0.5f);

        Heightmap minerals = new Heightmap(dim, dim, 10.0f);

        mr2.material.mainTexture = (Texture2D) combined_radial;

        Texture2D tex_map = new Texture2D(dim, dim);
        for (int i = 0; i < dim; i++)
        {
            for (int j = 0; j < dim; j++)
            {
                if (0.40f < rivers[i, j] && rivers[i, j] < 0.42f)
                    tex_map.SetPixel(i, j, Color.blue);
                else if (0.4f < lattice[i, j] && lattice[i, j] < 0.5f)
                    tex_map.SetPixel(i, j, Color.white);
                else if (!(0.4f < lattice[i, j] && lattice[i, j] < 0.5f) && 0.4f < combined_radial[i, j])
                    tex_map.SetPixel(i, j, Color.red);
                else if (0.25f < surface3[i, j] && surface3[i, j] < 0.45f)
                    tex_map.SetPixel(i, j, Color.green);
                else if (0.50f < surface3[i, j] && surface3[i, j] < 0.55f)
                    tex_map.SetPixel(i, j, Color.yellow);
                else if (0.30f < minerals[i, j] && minerals[i, j] < 0.40f)
                    tex_map.SetPixel(i, j, Color.grey);
                else if (0.6f < minerals[i, j] && minerals[i, j] < 0.70f)
                    tex_map.SetPixel(i, j, Color.black);
                else
                    tex_map.SetPixel(i, j, Color.white);
            }
        }
        tex_map.Apply();
        mr3.material.mainTexture = tex_map;   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
