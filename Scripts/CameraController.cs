﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField, Range(1, 10)]
    private float translateSpeed = 5.0f;
    [SerializeField, Range(1, 10)]
    private float zoomSpeed = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(Input.mouseScrollDelta.y) > Mathf.Epsilon) {
            transform.position += -1 * zoomSpeed * Vector3.up * Input.mouseScrollDelta.y;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position += -1 * translateSpeed* transform.right;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += 1 * translateSpeed * transform.right;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += 1 * translateSpeed * Vector3.Project(transform.forward, Vector3.forward);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position += -1 * translateSpeed * Vector3.Project(transform.forward, Vector3.forward);
        }
    }
}
