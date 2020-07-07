using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stub class. Currently move speed will just be a function of terrain type.
/// Eventually it will be a function of
/// - terrain type
/// - unit type
/// - local effects
/// </summary>
public class HeightmapSpeedLookupTable : Dictionary<int, float>
{
    private const float minMoveSpeed = 0.001f;

    public float GetTraversalCost(int terrain)
    {
        if (!this.ContainsKey(terrain))
        {
            Debug.LogFormat("Warning: move speed for terrain {0} undefined", terrain);
            return 1.0f / minMoveSpeed;
        }
        return 1.0f / this[terrain];
    }
}
