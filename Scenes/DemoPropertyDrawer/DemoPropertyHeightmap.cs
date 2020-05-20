using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The purpose of this class is to demonstrate 
/// </summary>
#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class DemoPropertyHeightmap : MonoBehaviour
{
    [SerializeField] Heightmap map;

    private void OnEnable()
    {
        // serialized map property is automatically drawn by HeightmapDrawer
        map = new Heightmap(256, 256, 10);
    }
}
