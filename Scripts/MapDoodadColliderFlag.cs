using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDoodadColliderFlag : MonoBehaviour
{
    public enum MapDoodadType
    {
        RIVER,
        FOREST,
        PLAINS
    };

    [SerializeField] public MapDoodadType m_type;
}
