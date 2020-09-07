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
public class HeightmapColorLookupTable: ISerializationCallbackReceiver
{
    [SerializeField, HideInInspector] private List<int> m_keys = new List<int>();
    [SerializeField, HideInInspector] private List<Color> m_values = new List<Color>();

    private Dictionary<int, Color> keyColorPairs = new Dictionary<int, Color>();

    /// <summary>
    /// Triggers OnBeforeSerialize. Then applies serialized json data to this object. 
    /// </summary>
    /// <param name="text">Text which contains json-format data which will populate m_keys / m_values</param>
    public void Overwrite(string text)
    {
        try
        {
            JsonUtility.FromJsonOverwrite(text, this);
        }
        catch (System.Exception e)
        {
            Debug.LogFormat("Error while parsing {0}:\n{1}", text, e);
        }
    }

    /// <summary>
    /// Removes key from known key-color pairs.
    /// </summary>
    /// <param name="key">A key to remove</param>
    public void Remove(int key)
    {
        if (keyColorPairs.ContainsKey(key))
        {
            keyColorPairs.Remove(key);
        }
    }

    public void OnBeforeSerialize()
    {
        // before writing, convert data in table into serializable collections
        m_keys = new List<int>(keyColorPairs.Keys);
        m_values = new List<Color>(keyColorPairs.Values);
    }

    public void OnAfterDeserialize()
    {
        // after reading in from serializable List<int>/List<Color>, resume using authoritative dictionary
        for(int i = 0; i < m_keys.Count; i++)
        {
            keyColorPairs[m_keys[i]] = m_values[i];
        }
    }

    /// <summary>
    /// Get a list of keys within this collection.
    /// </summary>
    public List<int> Keys
    {
        get
        {
            return new List<int>(keyColorPairs.Keys);
        }
    }

    /// <summary>
    /// Get a list of values within this collection.
    /// </summary>
    public List<Color> Values
    {
        get
        {
            return new List<Color>(keyColorPairs.Values);
        }
    }
    
    /// <summary>
    /// Setter and getter using index operator.
    /// </summary>
    /// <param name="i">A key</param>
    /// <returns>A Color paired to that key</returns>
    public Color this[int i]
    {
        get
        {
            if (keyColorPairs.ContainsKey(i))
            {
                return keyColorPairs[i];
            }
            return default;
        }
        set
        {
            keyColorPairs[i] = value;
        }
    }
}
