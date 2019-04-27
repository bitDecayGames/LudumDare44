using Board;
using UnityEngine;

public class SimpleMove : MonoBehaviour {
    private BoardPosition boardPos;

    private BoardManager board;
    private Board.Board.Occupier occupier;

    // Start is called before the first frame update
    void Start() {
        boardPos = GetComponent<BoardPosition>();
        occupier = GetComponent<Board.Board.Occupier>();
        board = FindObjectOfType<BoardManager>();
        
        Debug.Log(board.board);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            if (board.board.MoveLeft(occupier)) {
                GetComponent<BoardPosition>().boardPos.x -= 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.D)) {
            if (board.board.MoveRight(occupier)) {
                GetComponent<BoardPosition>().boardPos.x += 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.W)) {
            if (board.board.MoveDown(occupier)) {
                GetComponent<BoardPosition>().boardPos.y -= 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.S)) {
            if (board.board.MoveUp(occupier)) {
                GetComponent<BoardPosition>().boardPos.y += 1;
            }
        }
    }
}