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
    private static readonly Dictionary<Vector3, HashSet<Transform>> collection = new Dictionary<Vector3, HashSet<Transform>>();
    
    /// <summary>
    /// Gets game colliders near point.
    /// 
    /// Internal, unfiltered data acquisition of colliders using physics engine.
    /// </summary>
    /// <param name="p">A point in space</param>
    /// <param name="radius">a distance</param>
    /// <returns></returns>
    public static HashSet<Transform> GetTransformsNear(Vector3 p, float radius = 10.0f)
    {
        Vector3 binnedP = new Vector3(Mathf.Floor(p.x), Mathf.Floor(p.y), Mathf.Floor(p.z));
        if (!collection.ContainsKey(binnedP))
        {
            Collider[] adj = Physics.OverlapSphere(p, radius + Vector3.Distance(p, binnedP));
            collection[binnedP] = new HashSet<Transform>(adj.Select(x => x.transform));
        }

        return collection[binnedP];
    }
}
