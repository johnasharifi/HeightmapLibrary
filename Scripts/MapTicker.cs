using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapTicker : MonoBehaviour
{
    private Dictionary<int, HashSet<Action>> tickers = new Dictionary<int, HashSet<Action>>();
    
    // Update is called once per frame
    void Update()
    {
        foreach (int i in tickers.Keys)
        {
            if (Time.frameCount % i == 0)
            {
                foreach (Action act in tickers[i])
                    act();
            }
        }
    }
    
    /// <summary>
    /// Adds an action to collection of events which may occur periodically.
    /// 
    /// Freezes state / reference which the Action refers to.
    /// </summary>
    /// <param name="act">An action to execute periodically.</param>
    /// <param name="interval">A rate at which to execute an action.</param>
    public void AddTicker(Action act, int interval)
    {
        // TODO consider adding an offset param; and allow frame % interval == offset, rather than frame % interval == 0
        if (!tickers.ContainsKey(interval))
        {
            tickers[interval] = new HashSet<Action>();
        }
        tickers[interval].Add(act);

        
    }
}