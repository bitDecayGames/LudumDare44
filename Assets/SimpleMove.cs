using Board;
using DefaultNamespace;
using UnityEngine;

public class SimpleMove : MonoBehaviour {
    private BoardPosition boardPos;

    private BoardManager board;
    private Board.Board.Occupier occupier;
    private Animator animator;

    public Facing facing;

    public enum Facing {
        Up,
        Left,
        Down,
        Right,
        Unknown
    }

    // Start is called before the first frame update
    void Start() {
        animator = GetComponentInChildren<Animator>();
        boardPos = GetComponent<BoardPosition>();
        occupier = GetComponent<Board.Board.Occupier>();
        if (occupier == null) occupier = gameObject.AddComponent<Board.Board.Occupier>();
        board = FindObjectOfType<BoardManager>();
        
        
        animator.Play(AnimationConstants.WALK_UP_LEFT);
        Debug.Log(board.board);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            animator.Play(AnimationConstants.STAND_UP_LEFT);
            if (board.board.MoveLeft(occupier)) {
                GetComponent<BoardPosition>().boardPos.x -= 1;
            }
            facing = Facing.Left;
        }

        if (Input.GetKeyDown(KeyCode.D)) {
            animator.Play(AnimationConstants.STAND_DOWN_RIGHT);
            if (board.board.MoveRight(occupier)) {
                GetComponent<BoardPosition>().boardPos.x += 1;
            }
            facing = Facing.Right;
        }

        if (Input.GetKeyDown(KeyCode.W)) {
            animator.Play(AnimationConstants.STAND_UP_RIGHT);
            if (board.board.MoveDown(occupier)) {
                GetComponent<BoardPosition>().boardPos.y -= 1;
            }
            facing = Facing.Up;
        }

        if (Input.GetKeyDown(KeyCode.S)) {
            animator.Play(AnimationConstants.STAND_DOWN_LEFT);
            if (board.board.MoveUp(occupier)) {
                GetComponent<BoardPosition>().boardPos.y += 1;
            }
            facing = Facing.Down;
        }
    }
}