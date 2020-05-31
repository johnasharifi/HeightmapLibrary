using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Serializable class for defining colors of biomes in a Heightmap.
/// 
/// Important for defining a custom UI for this serializable type.
/// </summary>
[SerializeField]
public class HeightmapColorLookupTable
{
    private HashSet<int> uncontainedKeyCollection = new HashSet<int>();
    private Dictionary<int, Color> mapper = new Dictionary<int, Color>();

    public HeightmapColorLookupTable(Dictionary<int, Color> _mapping)
    {
        // acquire definitions from user-specified mapping
        mapper = _mapping;
    }

    public Color this[int i]
    {
        get
        {
            if (mapper.ContainsKey(i))
            {
                return mapper[i];
            }
            if (!uncontainedKeyCollection.Contains(i))
            {
                uncontainedKeyCollection.Add(i);
                Debug.LogErrorFormat("Undefined color mapping for {0}", i);
            }
            return default;
        }
    }
}
