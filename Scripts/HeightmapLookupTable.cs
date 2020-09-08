using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Supports serialization and O(1) int-value mapping.
/// </summary>
/// <typeparam name="T">The serializable type that is mapped to</typeparam>
public class HeightmapLookupTable<T>: ISerializationCallbackReceiver
{
    [SerializeField, HideInInspector] private List<int> m_keys = new List<int>();
    [SerializeField, HideInInspector] private List<T> m_values = new List<T>();

    private Dictionary<int, T> keyValuePairs = new Dictionary<int, T>();

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
        if (keyValuePairs.ContainsKey(key))
        {
            keyValuePairs.Remove(key);
        }
    }

    public void OnBeforeSerialize()
    {
        // before writing, convert data in table into serializable collections
        m_keys = new List<int>(keyValuePairs.Keys);
        m_values = new List<T>(keyValuePairs.Values);
    }

    public void OnAfterDeserialize()
    {
        // after reading in from serializable List<int>/List<Color>, resume using authoritative dictionary
        for (int i = 0; i < m_keys.Count; i++)
        {
            keyValuePairs[m_keys[i]] = m_values[i];
        }
    }

    /// <summary>
    /// Get a list of keys within this collection.
    /// </summary>
    public List<int> Keys
    {
        get
        {
            return new List<int>(keyValuePairs.Keys);
        }
    }

    /// <summary>
    /// Get a list of values within this collection.
    /// <typeparam name="T">The serializable type that is mapped to</typeparam>
    /// </summary>
    public List<T> Values
    {
        get
        {
            return new List<T>(keyValuePairs.Values);
        }
    }

    /// <summary>
    /// Setter and getter using index operator.
    /// </summary>
    /// <param name="i">A key</param>
    /// <typeparam name="T">The serializable type that is mapped to</typeparam>
    /// <returns>A Color paired to that key</returns>
    public T this[int i]
    {
        get
        {
            if (keyValuePairs.ContainsKey(i))
            {
                return keyValuePairs[i];
            }
            return default;
        }
        set
        {
            keyValuePairs[i] = value;
        }
    }

}
