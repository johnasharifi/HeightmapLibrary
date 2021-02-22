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
    private readonly Dictionary<string, HashSet<string>> tags = new Dictionary<string, HashSet<string>>();

    private Vector3 spawnLocalPosition;

    [SerializeField] MapEntityObjective m_Objective;

    private Collider myCollider;
    private Collider mapCollider;
    
    // Start is called before the first frame update
    void Start()
    {
        spawnLocalPosition = transform.localPosition;
    }

    /// <summary>
    /// Checks whether this entity has specified tag within specified tag group.
    /// </summary>
    /// <param name="group">A family of tags to search through. E.g. RESOURCE, FACTION</param>
    /// <param name="tag">A tag to check within a tag group</param>
    /// <returns>True tag is within group for this entity.</returns>
    public bool IsTag(string group, string tag)
    {
        group = group.ToUpperInvariant();
        tag = tag.ToUpperInvariant();

        if (!tags.ContainsKey(group))
        {
            return false;
        }
        return tags[group].Contains(tag);
    }

    /// <summary>
    /// Adds note that this MapEntity is a type of map object. E.g. resource, unit, building, etc.
    /// </summary>
    /// <param name="type">Type of map doodad.</param>
    public void AddTag(string group, string tag)
    {
        group = group.ToUpperInvariant();
        tag = tag.ToUpperInvariant();
        
        if (!tags.ContainsKey(group))
        {
            tags[group] = new HashSet<string>();
        }

        tags[group].Add(tag);
    }
}
