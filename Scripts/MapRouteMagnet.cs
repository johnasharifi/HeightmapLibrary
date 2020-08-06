using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Path = System.Collections.Generic.List<System.Tuple<int, int>>;

public class MapRouteMagnet : MonoBehaviour
{
    private const int magnetScaleReduction = 16;
    
    // Grid-adjacent map route magnets
    private HashSet<MapRouteMagnet> adjacents = new HashSet<MapRouteMagnet>();

    void SetPosition(Vector3 worldPos)
    {
        origin = new Vector2(Mathf.CeilToInt(transform.position.x), Mathf.CeilToInt(transform.position.z));
        span = new Vector2(magnetScaleReduction, magnetScaleReduction);
    }

    [SerializeField] private PathfindingTotem totem;

    [SerializeField] private Vector2 origin;
    [SerializeField] private Vector2 span;

    public override string ToString()
    {
        return string.Format("{0} x {1} to \t {2} x {3}", origin.x, origin.y, origin.x + span.x, origin.y + span.y);
    }

    public Vector3 P1
    {
        get
        {
            return Origin + transform.forward * magnetScaleReduction;
        }
    }

    public Vector3 P3
    {
        get
        {
            return Origin + transform.right * magnetScaleReduction;
        }
    }

    public Vector3 Origin
    {
        get
        {
            return transform.position - new Vector3(0.5f, 0f, 0.5f);
        }
    }

    public Vector3 Terminus
    {
        get
        {
            return Origin + new Vector3(span.x, 0f, span.y);
        }
    }
}
