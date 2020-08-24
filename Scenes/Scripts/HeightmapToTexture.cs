using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightmapToTexture : MonoBehaviour
{
    [SerializeField] private Renderer rend;

    [Range(64, 512)]
    [SerializeField] private int dims;
    
    // Start is called before the first frame update
    void Start()
    {
        // Heightmap map = new Heightmap(dims, dims);
        Heightmap map = rend.GetComponent<Heightmap>();

        var filter_water = MapFilterFactory.GetPerlinBand(dims, dims, 1.0f, 0.4f, 0.43f);
        var filter_exterior = MapFilterFactory.GetBlendedExteriorWeight(dims, dims, 0.5f);
        var filter_lattice = MapFilterFactory.GetPerlinBand(dims, dims, 10.0f, 0.3f, 0.4f);
        var filter_mountains = MapFilterFactory.GetPerlinBand(dims, dims, 5.0f, 0.4f, 0.5f);
        var filter_forests = MapFilterFactory.GetPerlinBand(dims, dims, 10.0f, 0.4f, 0.5f);
        var filter_plains = MapFilterFactory.GetPerlinBand(dims, dims, 5.0f, 0.4f, 0.5f);

        map.MapFromTo(-1, 5, -1, filter_water);
        map.MapFromTo(-1, 10, 1, filter_lattice);
        map.MapFromTo(1, 0, 1, filter_exterior);

        map.MapFromTo(1, 9, filter_mountains);
        map.MapFromTo(1, 8, filter_forests);
        map.MapFromTo(1, 7, filter_plains);

        System.Action<int, int> spawnMapEntitiesOnPlains = (int i, int j) =>
        {
            Vector3 p = new Vector3(j, i);
            GameObject go = new GameObject();
            go.transform.SetParent(transform, true);
            go.transform.localRotation = Quaternion.identity;
            go.transform.localPosition = new Vector3((p.x + 0.5f - dims / 2) / transform.localScale.x, (p.y + 0.5f - dims / 2) / transform.localScale.y, 0.0f);
            go.transform.localScale = new Vector3(1.0f / transform.localScale.x, 1.0f / transform.localScale.y, 1.0f / transform.localScale.z);
            
            int interval = Random.Range(0, TickMaster.maxSeconds);
            System.Action a = () =>
            {
                // TODO generate in a static stateless factory so that we do not pin a bunch of references to objects into our memory
                GameObject subobj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                subobj.transform.position = go.transform.position + new Vector3(Random.value, 0f, Random.value);
                subobj.transform.localScale = Vector3.one * 0.5f;
                subobj.transform.SetParent(go.transform);
                MapEntity me = subobj.AddComponent<MapEntity>();
                me.AddTag("RESOURCE", "NUGGET");
                Destroy(subobj, 5f);
            };
            TickMaster.AddAction(interval, a);
        };
        map.ApplyFunctionTo(7, spawnMapEntitiesOnPlains);

        Dictionary<int, Color> mapping = new Dictionary<int, Color>()
        {
            { 0, Color.red },
            { 5, Color.blue },
            { 7, Color.yellow },
            { 8, Color.green},
            { 9, Color.red},
            { 10, Color.white},
            { -1, Color.white },
            {1, Color.white }
        };
        HeightmapColorLookupTable lut = new HeightmapColorLookupTable(mapping);

        Texture2D tex = map.AsTexture2D(lut);
        tex.filterMode = FilterMode.Point;
        tex.Apply();

        rend.material.mainTexture = tex;
    }
    
}