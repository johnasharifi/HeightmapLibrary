using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Caches information about what Transforms are near points in an array / game world.
/// 
/// Assumes the game world dims are equal to array dims.
/// </summary>
public static class MapAdjacencyCacheUtility
{
    /// <summary>
    /// Gets game colliders near point.
    /// 
    /// Internal, unfiltered data acquisition of colliders using physics engine.
    /// </summary>
    /// <param name="p">A point in space</param>
    /// <param name="radius">a distance</param>
    /// <returns>A collection of Transforms which have colliders near p</returns>
    public static HashSet<Transform> GetTransformsNear(Vector3 p, float radius = 10.0f)
    {
        // hopefully casting IEnum<T> to HashSet<T> is relatively fast...
        // downstream calls will require HashSet's fast add/remove/contains; cannot use a simple IEnum
        return (HashSet<Transform>) Physics.OverlapSphere(p, radius).Select(x=> x.transform);
    }
}
