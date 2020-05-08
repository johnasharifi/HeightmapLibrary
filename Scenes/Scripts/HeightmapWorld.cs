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
        bool[,] isRiver = Heightmap.AND(0.40f < rivers, rivers < 0.45f);
        
        Texture2D tex = (Texture2D)h;

        for (int i = 0; i < scale; i++)
        {
            for (int j = 0; j < scale; j++)
            {
                if (isRiver[i, j])
                    tex.SetPixel(i, j, Color.blue);
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
