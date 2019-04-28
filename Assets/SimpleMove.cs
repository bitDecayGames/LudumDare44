﻿using Animations;
using Board;
using DefaultNamespace;
using UnityEngine;

public class SimpleMove : MonoBehaviour {
    public const float TIME_TO_MOVE = 0.25f;
    
    private BoardPosition boardPos;

    private BoardManager board;
    private Board.Board.Occupier occupier;
    private Animator animator;
    private LerpAnimator lerper;
    private bool isAcceptingInput = true;
    private bool standing = false;

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
        boardPos = GetComponent<BoardPosition>();
        animator = GetComponentInChildren<Animator>();
        boardPos = GetComponent<BoardPosition>();
        occupier = GetComponent<Board.Board.Occupier>();
        if (occupier == null) occupier = gameObject.AddComponent<Board.Board.Occupier>();
        board = FindObjectOfType<BoardManager>();
        lerper = GetComponentInChildren<LerpAnimator>();
        if (lerper == null) lerper = animator.gameObject.AddComponent<LerpAnimator>();
        
        animator.Play(AnimationConstants.WALK_UP_LEFT);
        Debug.Log(board.board);
    }

    // Update is called once per frame
    void Update() {
        if (isAcceptingInput) {
            if (Input.GetKey(KeyCode.A)) {
                facing = Facing.Left;
                if (board.board.MoveLeft(occupier) || Input.GetKey(KeyCode.LeftArrow)) {
                    UpdateWalkAnimation();
                    var originalPos = boardPos.ToWorldPos();
                    boardPos.X -= 1;
                    StartLerp(originalPos, boardPos.ToWorldPos());
                } else {
                    UpdateStandAnimation();
                }
            } else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
                facing = Facing.Right;
                if (board.board.MoveRight(occupier)) {
                    UpdateWalkAnimation();
                    var originalPos = boardPos.ToWorldPos();
                    boardPos.X += 1;
                    StartLerp(originalPos, boardPos.ToWorldPos());
                } else {
                    UpdateStandAnimation();
                }
            } else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
                facing = Facing.Up;
                if (board.board.MoveDown(occupier)) {
                    UpdateWalkAnimation();
                    var originalPos = boardPos.ToWorldPos();
                    boardPos.Y -= 1;
                    StartLerp(originalPos, boardPos.ToWorldPos());
                } else {
                    UpdateStandAnimation();
                }
            } else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
                facing = Facing.Down;
                if (board.board.MoveUp(occupier)) {
                    UpdateWalkAnimation();
                    var originalPos = boardPos.ToWorldPos();
                    boardPos.Y += 1;
                    StartLerp(originalPos, boardPos.ToWorldPos());
                } else {
                    UpdateStandAnimation();
                }
            } else if (!standing) {
                UpdateStandAnimation();
            }
        }
    }

    private void StartLerp(Vector2 originalPos, Vector2 endPos) {
        isAcceptingInput = false;
        standing = false;
        lerper.Begin(originalPos, endPos, TIME_TO_MOVE, () => {
            isAcceptingInput = true;
        });
    }

    private void UpdateStandAnimation() {
        standing = true;
        switch (facing) {
            case Facing.Up:
                animator.Play(AnimationConstants.STAND_UP_RIGHT);
                break;
            case Facing.Right:
                animator.Play(AnimationConstants.STAND_DOWN_RIGHT);
                break;
            case Facing.Down:
                animator.Play(AnimationConstants.STAND_DOWN_LEFT);
                break;
            case Facing.Left:
                animator.Play(AnimationConstants.STAND_UP_LEFT);
                break;
        }
    }

    private void UpdateWalkAnimation() {
        switch (facing) {
            case Facing.Up:
                animator.Play(AnimationConstants.WALK_UP_RIGHT);
                break;
            case Facing.Right:
                animator.Play(AnimationConstants.WALK_DOWN_RIGHT);
                break;
            case Facing.Down:
                animator.Play(AnimationConstants.WALK_DOWN_LEFT);
                break;
            case Facing.Left:
                animator.Play(AnimationConstants.WALK_UP_LEFT);
                break;
        }
    }
}