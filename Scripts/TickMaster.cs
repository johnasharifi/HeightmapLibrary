using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Class which registers actions to be performed periodically.
/// 
/// While user can have more than one TickMaster in scene at a time, it is better if there is only one.
/// </summary>
public class TickMaster: MonoBehaviour
{
    // static values for lazy singleton
    private static TickMaster instance;

    // highly divisible
    public const int maxSeconds = 120;

    // instanced params
    [SerializeField] private bool paused = false;
    [SerializeField] private float myTime = 0.0f;
    [SerializeField] private int curr = 0;

    private Dictionary<int, HashSet<Action>> events = new Dictionary<int, HashSet<Action>>();

    void Update()
    {
        int prev = Mathf.FloorToInt(myTime);
        myTime += (paused? 0:1) * Time.deltaTime;
        curr = Mathf.FloorToInt(myTime);

        if (prev != curr)
        {
            // then trigger events for prev
            int offset = prev % maxSeconds;
            if (events.ContainsKey(offset))
            {
                foreach (Action a in events[offset])
                {
                    a();
                }
            }
        }
    }
    
    /// <summary>
    /// Get/set the Istance of this ad-hoc singleton
    /// </summary>
    public static TickMaster Instance {
        set
        {
            if (instance != null)
            {
                Debug.LogErrorFormat("Error: a TickMaster already exists on {0} but you are adding a new one to {1}", instance.gameObject.name, value.gameObject.name);
            }
            else
            {
                instance = value;
            }
        }
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("TickMasterSingleton");
                instance = go.AddComponent<TickMaster>();
            }
            
            return instance;
        }
    }

    /// <summary>
    /// Registers an action to occur every <paramref name="tick"/> seconds
    /// </summary>
    /// <param name="tick">How many seconds the event should occur</param>
    /// <param name="act">An action to perform periodically</param>
    public static void AddAction(int tick, Action act)
    {
        tick = tick % maxSeconds;
        if (!Instance.events.ContainsKey(tick))
        {
            Instance.events.Add(tick, new HashSet<Action>());
        }
        Instance.events[tick].Add(act);
    }

    /// <summary>
    /// Unregister an action which had been occurring every <paramref name="tick"/> seconds
    /// </summary>
    /// <param name="tick">What time interval to search in</param>
    /// <param name="act">Action to unregister</param>
    /// <returns>True if action had been registered previously</returns>
    public static bool RemoveAction(int tick, Action act)
    {
        if (!Instance.events.ContainsKey(tick))
        {
            Instance.events.Add(tick, new HashSet<Action>());
        }
        return Instance.events[tick].Remove(act);
    }

    private void Awake()
    {
        Instance = this;
    }

}
