using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperTiled2Unity;
using Board;

public class PlayerTaskController : MonoBehaviour
{
    void Start()
    {

    }
    
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Debug.Log("wer space    ");
            // Get player board position
            BoardPosition playerPos = GetComponent<BoardPosition>();

            BoardManager bm = FindObjectOfType<BoardManager>();
            var boardNode = bm.board.Get(playerPos.boardPos.x, playerPos.boardPos.y);
            Debug.Log("Board Pos " + boardNode.x + " " + boardNode.y);
            Debug.Log("My Pos" + playerPos.boardPos.x + " " + playerPos.boardPos.y);
            if (boardNode.occupier != null)
            {
                Debug.Log("Found ocupierer");
                var nodeProperty = boardNode.occupier.GetComponent<SuperCustomProperties>();
                if (nodeProperty != null)
                {
                    Debug.Log("Found Node Property");
                    var taskStepType = new CustomProperty();
                    if (nodeProperty.TryGetCustomProperty("TaskStepType", out taskStepType))
                    {
                        Debug.Log("Found Node Property");
                        if (taskStepType.m_Value == "MoveToSafe")
                        {
                            Debug.Log("Found Move to safe");
                            TaskManager manager = FindObjectOfType<TaskManager>();
                            manager.CompleteStep(TaskStepType.MoveToSafe);
                        }
                    }
                }
            }
        }
    }
}
