using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Expedient implementation of a map thing. Will contain info about properties, map position.
/// 
/// MapEntities need to have colliders so they can be found by map searches.
/// </summary>
[RequireComponent(typeof(Collider))]
public class MapEntity : MonoBehaviour
{
    private const string mapTypeKey = "RESOURCE";
    private const string factKey = "FACTION";
    private readonly Dictionary<string, HashSet<string>> tags = new Dictionary<string, HashSet<string>>();

    private Vector3 spawnLocalPosition;

    private Collider myCollider;
    private Collider mapCollider;
    
    // Start is called before the first frame update
    void Start()
    {
        spawnLocalPosition = transform.localPosition;
    }

    /// <summary>
    /// Checks if this MapEntity is affiliated with a certain faction.
    /// </summary>
    /// <param name="faction">A string ID for a faction</param>
    /// <returns>True if this map entity is positively affiliated with a faction, false if no faction or not same faction(s)</returns>
    public bool IsFaction(string faction)
    {
        faction = faction.ToUpperInvariant();
        if (!tags.ContainsKey(factKey))
        {
            tags[factKey] = new HashSet<string>();
        }

        return tags[factKey].Contains(faction);
    }

    /// <summary>
    /// Adds note that this MapEntity is a type of map object. E.g. resource, unit, building, etc.
    /// </summary>
    /// <param name="type">Type of map doodad.</param>
    public void AddType(string type)
    {
        type = type.ToUpperInvariant();
        if (!tags.ContainsKey(mapTypeKey))
        {
            tags[mapTypeKey] = new HashSet<string>();
        }

        tags[mapTypeKey].Add(type);
    }
}
