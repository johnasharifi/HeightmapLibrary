using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(MapRouteMagnet))]
public class MapRouteMagnetEditor : Editor
{
    Vector3 P1;
    Vector3 P2;

    private void OnEnable()
    {
        MapRouteMagnet myTarget = (MapRouteMagnet)target;
        P1 = myTarget.Origin;
        P2 = myTarget.Terminus;
    }

    private void OnSceneGUI()
    {
        MapRouteMagnet myTarget = (MapRouteMagnet)target;
        
        Debug.DrawLine(myTarget.Origin, myTarget.P1);
        Debug.DrawLine(myTarget.P1, myTarget.Terminus);
        Debug.DrawLine(myTarget.Terminus, myTarget.P3);
        Debug.DrawLine(myTarget.P3, myTarget.Origin);
        
        Quaternion id = Quaternion.identity;
        
        P1 = Handles.DoPositionHandle(P1, id);
        P2 = Handles.DoPositionHandle(P2, id);
    }
}
