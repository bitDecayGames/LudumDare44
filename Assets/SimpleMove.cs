using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMove : MonoBehaviour
{

    private BoardPosition boardPos;
    // Start is called before the first frame update
    void Start()
    {
        boardPos = GetComponent<BoardPosition>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            GetComponent<BoardPosition>().
                boardPos.x -= 1;
        if (Input.GetKeyDown(KeyCode.D))
            GetComponent<BoardPosition>().
                boardPos.x += 1;
        if (Input.GetKeyDown(KeyCode.W))
            GetComponent<BoardPosition>().
                boardPos.y -= 1;
        if (Input.GetKeyDown(KeyCode.S))
            GetComponent<BoardPosition>().
                boardPos.y += 1;
    }
}
