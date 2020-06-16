using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Expedient implementation of a self-organizing wandering map entity / unit.
/// 
/// Units need to have colliders so they can be found by other colliders.
/// 
/// All MapEntities will have a lifetime by default; and will destroy themselves eventually.
/// In editor, MapEntities will halt the self-destruction process when selected by user. This
/// assists in debugging.
/// </summary>
[RequireComponent(typeof(Collider))]
public class MapEntity : MonoBehaviour
{
    private Vector3 spawnLocalPosition;

    private Collider myCollider;
    private Collider mapCollider;
    
    // Start is called before the first frame update
    void Start()
    {
        spawnLocalPosition = transform.localPosition;
    }
}
