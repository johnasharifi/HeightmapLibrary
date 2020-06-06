using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Caches information about what Transforms are near points in an array / game world.
/// 
/// Assumes the game world dims are equal to array dims.
/// </summary>
public class MapAdjacencyCache
{
    private readonly HashSet<Transform>[,] collection;

    public MapAdjacencyCache(int dim1, int dim2)
    {
        dim1 = Mathf.Max(dim1, 1);
        dim2 = Mathf.Max(dim2, 1);
        collection = new HashSet<Transform>[dim1, dim2];
    }

    private Collider[] GetCollidersNear(int i, int j, float radius = 10.0f)
    {
        Collider[] adj = Physics.OverlapSphere(new Vector3(i, 0f, j), radius);
        return (adj);
    }

    public void Clear(int i, int j)
    {
        collection[i, j] = null;
    }
    
    private HashSet<Transform> Get(int i, int j)
    {
        i = Mathf.Clamp(i, 0, collection.GetLength(0));
        j = Mathf.Clamp(j, 0, collection.GetLength(1));

        if (collection[i,j] == null)
        {
            try
            {
                Collider[] adj = GetCollidersNear(i, j);
                collection[i, j] = new HashSet<Transform>(adj.Select(x => x.transform));
            }
            catch (System.Exception e)
            {
                collection[i, j] = null;
            }
        }
        // always clear null elems from collection
        collection[i, j].Remove(null);
        return collection[i, j];
    }
    
    public HashSet<Transform> this[int i, int j]
    {
        get
        {
            return Get(i, j);
        }
    }
}
