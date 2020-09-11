using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HeightmapBiomeFilter
{
    // TODO enum
    public static readonly string[] predicateTypes = new string[] { "blended exterior", "perlin band" };

    /// <summary>
    /// Defines which predicate type is blended exterior.
    /// </summary>
    /// <param name="i">Index of blended exterior in string[]</param>
    /// <returns>True if i is equal to index of blended exterior in predicateTypes</returns>
    public static bool IsBlendedExterior(int i)
    {
        return i == 0;
    }

    [SerializeField] private int m_originClass;
    public int originClass
    {
        get
        {
            return m_originClass;
        }
    }
    [SerializeField] private int m_targetClass;
    public int targetClass
    {
        get
        {
            return m_targetClass;
        }
    }
    [SerializeField] private int m_failClass;
    public int failClass
    {
        get
        {
            return m_failClass;
        }
    }
    [SerializeField] private int m_predicateType;
    public int predicateType
    {
        get
        {
            return m_predicateType;
        }
    }
    [SerializeField] private float m_predicatePerlinScale;
    public float predicatePerlinScale
    {
        get
        {
            return m_predicatePerlinScale;
        }
    }
    [SerializeField] private float m_predicateThresholdA;
    public float predicateThresholdA
    {
        get
        {
            return m_predicateThresholdA;
        }
    }
    [SerializeField] private float m_predicateThresholdB;
    public float predicateThresholdB
    {
        get
        {
            return m_predicateThresholdB;
        }
    }
}