using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Serializable class for defining colors of biomes in a Heightmap.
/// 
/// Important for defining a custom UI for this serializable type.
/// 
/// Requires a SortedDictionary for normal operations, and serializable Lists for serializable operations.
/// </summary>
[System.Serializable]
public class HeightmapColorLookupTable
{
    [SerializeField, HideInInspector] private List<int> m_keys = new List<int>();
    [SerializeField, HideInInspector] private List<Color> m_values = new List<Color>();

    public void Remove(int index)
    {
        if (index <= m_keys.Count)
            m_keys.RemoveAt(index);
        if (index <= m_values.Count)
            m_values.RemoveAt(index);
    }
    
    public List<int> Keys
    {
        get
        {
            return m_keys;
        }
    }

    public List<Color> Values
    {
        get
        {
            return m_values;
        }
    }
    
    public Color this[int i]
    {
        get
        {
            for (int index = 0; index < m_keys.Count; index++)
            {
                if (m_keys[index] == i)
                    return m_values[index];
            }
            return default;
        }
        set
        {
            for (int index = 0; index < m_keys.Count; index++)
            {
                if (m_keys[index] == i)
                {
                    m_values[index] = value;
                    return;
                }
            }
            
            m_keys.Add(i);
            m_values.Add(value);
        }
    }
}
