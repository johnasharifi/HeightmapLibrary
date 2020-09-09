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

    [SerializeField] private int originClass;
    [SerializeField] private int targetClass;
    [SerializeField] private int failClass;
    [SerializeField] private int predicateType;
    [SerializeField] private float predicatePerlinScale;
    [SerializeField] private float predicateThresholdA;
    [SerializeField] private float predicateThresholdB;
}