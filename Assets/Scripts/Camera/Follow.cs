using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{

    void Start()
    {

    }

    void Update()
    {
        var newPosition = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
        Camera.main.transform.position = newPosition;
    }
}