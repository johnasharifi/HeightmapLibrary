using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpawner : MonoBehaviour
{
    [SerializeField] private GameObject mapEntityPrefab;
    [Range(1.0f, 10.0f), SerializeField] private float spawnRange = 5.0f;
    [Range(1.0f, 10.0f), SerializeField] private float spawnInterval = 5.0f;

    private WaitForSeconds interval;

    private void Start()
    {
        interval = new WaitForSeconds(spawnInterval);
        StartCoroutine(SpawnPeriodic());
    }

    IEnumerator SpawnPeriodic()
    {
        // random stagger spawn times
        yield return new WaitForSeconds(Random.value * 5.0f);
        for (; ;)
        {
            if (mapEntityPrefab == null)
            {
                // might end up supporting case where prefab is null; just don't spawn on that tick
                Debug.LogErrorFormat("Error: no prefab attached");
            }
            else
            {
                GameObject obj = GameObject.Instantiate(mapEntityPrefab);
                float theta = Random.value * Mathf.PI * 2.0f;
                obj.transform.position = transform.position + Random.value * spawnRange * new Vector3(Mathf.Cos(theta), 0.0f, Mathf.Sin(theta));
            }

            yield return interval;
        }
    }
}
