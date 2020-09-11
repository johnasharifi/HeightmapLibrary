using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Point = System.Tuple<int, int>;

[RequireComponent(typeof(LineRenderer))]
public class PathfindingTotem : MonoBehaviour
{
    // [SerializeField] private Renderer rend;
    [SerializeField] private LineRenderer lrend;
    [SerializeField] private Transform target;
    [SerializeField] private Heightmap map;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            Point origxz = new Point(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));
            Point targetxz = new Point(Mathf.FloorToInt(target.position.x), Mathf.FloorToInt(target.position.z));

            List<Point> path = map.PathFromTo(origxz, targetxz);

            if (path == null || path.Count == 0)
            {
                lrend.enabled = false;
            }
            else
            {
                lrend.enabled = true;
                Vector3[] points = new Vector3[path.Count];
                for (int i = 0; i < path.Count; i++)
                {
                    points[i] = new Vector3(path[i].Item1, 0f, path[i].Item2);
                }
                lrend.positionCount = path.Count;
                lrend.SetPositions(points);
            }
            
        }
    }
}
