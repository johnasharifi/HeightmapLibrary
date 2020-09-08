using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Serializable class for defining colors of biomes in a Heightmap.
/// 
/// Important for defining a custom UI for this serializable type.
/// 
/// Recruits the HeightmapColorLookupTable type.
/// </summary>
[System.Serializable]
public class HeightmapColorLookupTable: HeightmapLookupTable<Color>
{
}
