using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotateSpeed;    
    private Transform tr;
    private void Awake()
    {
        tr = transform;
    }
    void Update()
    {
        
        tr.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
    }
}
