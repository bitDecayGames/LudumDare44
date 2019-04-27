using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMove : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
            transform.position = transform.position + Vector3.left/20f;
        if (Input.GetKey(KeyCode.D))
            transform.position = transform.position + Vector3.right/20f;
        if (Input.GetKey(KeyCode.W))
            transform.position = transform.position + Vector3.up/20f;
        if (Input.GetKey(KeyCode.S))
            transform.position = transform.position + Vector3.down/20f;

        Debug.Log(transform.position);
    }
}
