using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HeightmapBiomeFilterLookupTable
{
    [SerializeField] private List<HeightmapBiomeFilter> filters = new List<HeightmapBiomeFilter>();

    public void Add(HeightmapBiomeFilter filter)
    {
        filters.Add(filter);
    }

    public HeightmapBiomeFilter this[int i]
    {
        get
        {
            return filters[i];
        }
    }
}
