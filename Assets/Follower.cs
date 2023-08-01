using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]

public class Follower : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] private float lerpRate;

    private void Update()
    {
        //transform.position = Vector3.Lerp(transform.position, target.position, lerpRate * Time.deltaTime);
        transform.position = target.position;
    }
}
