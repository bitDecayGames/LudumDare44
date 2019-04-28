using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperTiled2Unity;
using Board;
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
            IsoVector2 playerBoardPos = playerPos.boardPos;

            BoardManager bm = FindObjectOfType<BoardManager>();
            Board.Board.Node playerBoardNode = bm.board.Get(playerBoardPos.x, playerBoardPos.y);

            Board.Board.Node checkNode = player.facing == SimpleMove.Facing.Up ? playerBoardNode.up :
                                         player.facing == SimpleMove.Facing.Down ? playerBoardNode.down :
                                         player.facing == SimpleMove.Facing.Left ? playerBoardNode.left :
                                         player.facing == SimpleMove.Facing.Right ? playerBoardNode.right:
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
                    var taskStepType = new CustomProperty();
                    if (nodeProperty.TryGetCustomProperty("TaskStepType", out taskStepType))
                    {

                        if (taskStepType.m_Value == TaskStep.GetStepName(TaskStepType.MoveToSafe)){
                            TaskManager manager = FindObjectOfType<TaskManager>();
                            manager.CompleteTaskStep(TaskStepType.MoveToSafe);
                        }

                    }
                }
            }
        }
    }
}
