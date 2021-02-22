using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapEntityObjective
{
    private readonly List<MapEntityObjective> subObjectives = new List<MapEntityObjective>();

    [SerializeField] private EntityObjectiveLabel label;
}

public enum EntityObjectiveLabel
{
    STOP,
    ATTACK,
    DEFEND,
    PATROL
}
