using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotateObject : MonoBehaviour
{
    [SerializeField] private bool active = true;
    [SerializeField] private float speed;
    [SerializeField] private Vector3 axis;

    private Transform _transform;

    private void Start()
    {
        _transform = transform;
    }

    void Update()
    {
        if(!active) return;

        _transform.Rotate(axis * (speed * Time.deltaTime));
    }
}
