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

    /// <summary>
    /// Occurs when object is visibe in scene view. Drawn lines remain on screen until a screen refresh is triggered
    /// </summary>
    private void OnDrawGizmos()
    {
        if (Application.isPlaying && UnityEditor.Selection.activeGameObject == gameObject)
        {
            Tuple<int, int> bl = new Tuple<int, int>((int) origin.x, (int) origin.y);
            Tuple<int, int> tr = new Tuple<int, int>((int)(origin.x + span.x - 1), (int)(origin.y + span.y - 1));

            int tbl = totem.GetMap[bl.Item1, bl.Item2];
            int ttr = totem.GetMap[tr.Item1, tr.Item2];

            Color colorOrigin = totem.GetLut[tbl];
            Color colorTerminus = totem.GetLut[ttr];

            Debug.LogFormat("bottom left point {0}\tregion type {1}\tcolor{2}.\t=====\ttop right point {3}\tregion type {4}\tcolor {5}", bl, tbl, colorOrigin, tr, ttr, colorTerminus);

            // bl must be (-1,-1)
            // tr could be (-0,-0) or (0,-1)
        }

        // reset position when user selects
        SetPosition(transform.position);
        if (UnityEditor.Selection.activeGameObject == gameObject) {
            
            foreach (MapRouteMagnet mag in adjacents)
            {
                Debug.DrawLine(transform.position, mag.transform.position);
            }

            // Debug.DrawLine(transform.position, transform.position + transform.forward * magnetScaleReduction);

            Vector3 offset = new Vector3(-0.5f, 0f, -0.5f);
            Debug.DrawLine(transform.position + offset, transform.position + offset + transform.forward * magnetScaleReduction);
            Debug.DrawLine(transform.position + offset + transform.forward * magnetScaleReduction, transform.position + offset + transform.forward * magnetScaleReduction + transform.right * magnetScaleReduction);
            Debug.DrawLine(transform.position + offset + transform.forward * magnetScaleReduction + transform.right * magnetScaleReduction, transform.position + offset + transform.right * magnetScaleReduction);
            Debug.DrawLine(transform.position + offset + transform.right * magnetScaleReduction, transform.position + offset);
        }
    }
}
