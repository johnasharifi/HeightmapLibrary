using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Expedient implementation of a self-organizing wandering map entity / unit.
/// 
/// Units need to have colliders so they can be found by other colliders.
/// 
/// All MapEntities will have a lifetime by default; and will destroy themselves eventually.
/// In editor, MapEntities will halt the self-destruction process when selected by user. This
/// assists in debugging.
/// </summary>
[RequireComponent(typeof(Collider))]
public class MapEntity : MonoBehaviour
{
    private Vector3 spawnPosition;

    private Collider myCollider;
    private Collider mapCollider;

    [SerializeField] private Transform target;

    [SerializeField, Range(0, 10.0f)] private float moveSpeed = 10.0f;
    [SerializeField, Range(0, 10.0f)] private float rotateSpeed = 10.0f;

    const float acquisitionRadius = 20.0f;
    
    private readonly WaitForSeconds acquireTargetInterval = new WaitForSeconds(5.0f);
    private readonly WaitForSeconds delayDestructionInterval = new WaitForSeconds(10.0f);

    // Start is called before the first frame update
    void Start()
    {
        // TODO populate these params in a more elegant way
        spawnPosition = transform.position;
        // set random initial polarity so that doodads start out with random spin in xy plane
        transform.rotation = Quaternion.Euler(0.0f, Random.value * 360f, 0f);
        myCollider = GetComponent<Collider>();
        StartCoroutine(DelayDestruction());
        StartCoroutine(AcquireTarget());
    }

    // Update is called once per frame
    void Update()
    {
        // use continuous movement so that we don't have to deal with snappy movement which hides info in most frames
        Vector3 idealPos = transform.position + transform.forward * 10.0f;
        if (target != null)
        {
            idealPos = GetIdealPositionWithRespectTo(target);
        }

        Vector3 dt = (idealPos - transform.position).normalized;
        Quaternion look = Quaternion.LookRotation(dt);
        transform.rotation = Quaternion.Lerp(transform.rotation, look, rotateSpeed * Time.deltaTime);
        transform.position = transform.position + transform.forward * moveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// Calculates ideal organic position for a MapEntity to trail another Transform by
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    Vector3 GetIdealPositionWithRespectTo(Transform t)
    {
        if (t == null)
        {
            Debug.LogErrorFormat("Error: no transform for GetIdealPosition");
            return transform.position;
        }
        // eventually this will be a parameterized function
        // but for now do fixed process
        return t.position - t.forward * 5.0f;
    }

    IEnumerator DelayDestruction()
    {
        yield return delayDestructionInterval;
#if UNITY_EDITOR
        while (UnityEditor.Selection.activeGameObject != null && UnityEditor.Selection.activeGameObject.transform == this.transform.root)
        {
            yield return delayDestructionInterval;
            continue;
        }
#endif

        // if we got to this step, we have waited a DDI, + (debug case) we are not selected by user in editor
        Destroy(gameObject);
    }

    IEnumerator AcquireTarget()
    {
        for (; ;)
        {
            HashSet<Transform> adj = MapAdjacencyCacheUtility.GetTransformsNear(transform.position);

            IEnumerable<Collider> colliderCollection = adj.Where(x => x != null && x.GetComponent<Collider>() != myCollider && x.GetComponent<Collider>() != mapCollider).Select(x => x.GetComponent<Collider>());
            
            if (colliderCollection != null && colliderCollection.Count() > 0)
            {
                // obviously later we should do filtering here
                // want organic self-organized geometry. not random draws out of a hat
                target = colliderCollection.ElementAt(Random.Range(0, colliderCollection.Count())).transform;
            }

            yield return acquireTargetInterval;
        }
    }
}
