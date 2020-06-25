using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapTicker : MonoBehaviour
{
    // max: 1000 frames. if FPS is 60, ~16.6 seconds
    private const int maxFrameInterval = 1000;
    private Dictionary<int, HashSet<Action>> tickers = new Dictionary<int, HashSet<Action>>();
    private int offset;

    // Update is called once per frame
    void Update()
    {
        // TODO
        // evaluates n items every frame. want to evaluate once every n frames
        foreach (int i in tickers.Keys)
        {
            if (Time.frameCount % i == offset)
            {
                if (MapAdjacencyCacheUtility.GetTransformsNear(transform.position, radius: 1).Count < 10)
                {
                    foreach (Action act in tickers[i])
                        act();
                }
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
        interval = Mathf.Max(maxFrameInterval, interval);
        if (tickers.Keys.Count == 0)
        {
            offset = UnityEngine.Random.Range(0, maxFrameInterval);
        }
        if (!tickers.ContainsKey(interval))
        {
            tickers[interval] = new HashSet<Action>();
        }
        tickers[interval].Add(act);

        
    }
}