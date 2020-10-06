using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HeightmapBiomeFilterLookupTable: IEnumerable<HeightmapBiomeFilter>
{
    [SerializeField] private List<HeightmapBiomeFilter> filters = new List<HeightmapBiomeFilter>();

    public void Add(HeightmapBiomeFilter filter)
    {
        filters.Add(filter);
    }

    public IEnumerator<HeightmapBiomeFilter> GetEnumerator()
    {
        return ((IEnumerable<HeightmapBiomeFilter>)filters).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<HeightmapBiomeFilter>)filters).GetEnumerator();
    }

    public HeightmapBiomeFilter this[int i]
    {
        get
        {
            return filters[i];
        }
    }
}
