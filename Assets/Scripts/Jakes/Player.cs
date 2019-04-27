using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    const float SPEED = 5f;

    private CharacterController _controller;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
    }
    
    void Update()
    {
        Move();

        if (Input.GetKeyDown("space"))
        {
            TaskManager manager = GameObject.FindObjectOfType<TaskManager>();
            manager.CompleteStep(TaskStepType.MoveToSafe);
        }
    }

    void Move()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        _controller.Move(move * Time.deltaTime * SPEED);
    }
}
