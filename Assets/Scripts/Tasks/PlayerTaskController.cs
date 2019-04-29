﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperTiled2Unity;
using Board;
using Movement;
using Utils;

public class PlayerTaskController : MonoBehaviour
{
    void Start()
    {

    }
    
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            // Get player
            SimpleMove player = GetComponent<SimpleMove>();

            // Get player board position
            BoardPosition playerPos = GetComponent<BoardPosition>();

            BoardManager bm = FindObjectOfType<BoardManager>();
            Board.Board.Node playerBoardNode = bm.board.Get(playerPos.X, playerPos.Y);

            Board.Board.Node checkNode = player.facing == Facing.Up ? playerBoardNode.up :
                                         player.facing == Facing.Down ? playerBoardNode.down :
                                         player.facing == Facing.Left ? playerBoardNode.left :
                                         player.facing == Facing.Right ? playerBoardNode.right:
                                         playerBoardNode;

            // Debug.Log("check node Pos " + checkNode.x + " " + checkNode.y);
            // Debug.Log("My Pos" + playerBoardNode.x + " " + playerBoardNode.y);
            if (!checkNode.Equals(playerBoardNode) && checkNode.occupier != null)
            {
                // Debug.Log("Found ocupierer on check node");
                var nodeProperty = checkNode.occupier.GetComponent<SuperCustomProperties>();
                if (nodeProperty != null)
                {
                    // Debug.Log("Found Node Property");
                    var typeProperty = new CustomProperty();
                    if (nodeProperty.TryGetCustomProperty("TaskStepType", out typeProperty))
                    {
                        // Debug.Log("Property: " + typeProperty.m_Value);
                        TaskManager manager = FindObjectOfType<TaskManager>();
                        TaskStepType taskStepType = (TaskStepType) System.Enum.Parse(typeof(TaskStepType), typeProperty.m_Value);
                        List<Board.Board.Node> taskStepTypeNodes = manager.GetNodesFromStepType(taskStepType);

                        if (taskStepTypeNodes.Count > 0)
                        {
                            foreach (var node in taskStepTypeNodes)
                            {
                                if(node != null && node.Equals(checkNode))
                                {
                                    manager.CompleteTaskStep(taskStepType, null);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
