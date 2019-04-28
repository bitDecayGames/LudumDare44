using System;
using System.Collections.Generic;
using Animations;
using Board;
using DefaultNamespace;
using UnityEngine;
using Utils;

namespace Movement {
    public class NpcMove : MonoBehaviour {
        public const float TIME_TO_MOVE = 0.1f;

        private BoardPosition boardPos;

        private BoardManager board;
        private Board.Board.Occupier occupier;
        private Animator animator;
        private LerpAnimator lerper;
        private bool isAcceptingInput = true;
        private bool standing = false;
        private Action callback;
        private bool isFindingPath = false;
        private List<Direction> currentDirections;
        private int tries = 0;
        private const int maxTries = 30;
        private int overallTries = 0;
        private const int maxOverallTries = 5;
        private Board.Board.Node currentStepLocation;
        private bool initialized = false;

        [HideInInspector]
        public Facing facing;

        private void Start() {
            Initialize();
        }

        public void Initialize() {
            if (!initialized) {
                initialized = true;
                boardPos = GetComponent<BoardPosition>();
                animator = GetComponentInChildren<Animator>();
                occupier = GetComponent<Board.Board.Occupier>();
                if (occupier == null) occupier = gameObject.AddComponent<Board.Board.Occupier>();
                board = FindObjectOfType<BoardManager>();
                if (board == null) throw new Exception("Missing BoardManager in the Scene, just drag the script onto the Camera");
                if (board.board.stepLocations.ContainsKey(TaskStepType.NpcSpawn.ToString().ToLower()))
                {
                    // TODO make sure we spawn these thigns reasonably
                    var mySpawn = board.board.stepLocations[TaskStepType.NpcSpawn.ToString().ToLower()][0];
                    boardPos.X = mySpawn.myNode.x;
                    boardPos.Y = mySpawn.myNode.y;
                    board.board.SetForce(occupier, mySpawn.myNode.x, mySpawn.myNode.y);

                    // Debug.Log("Board position bitch: " + boardPos.X + ", " + boardPos.Y);
                    // Debug.Log("My spawn bitch: " + mySpawn);
                    // Debug.Log("My node bitch: " + mySpawn.myNode.x + ", " + mySpawn.myNode.y);

                } else {
                    throw new Exception("No Npc spawner has been is found");
                }
                lerper = GetComponentInChildren<LerpAnimator>();
                if (lerper == null) lerper = animator.gameObject.AddComponent<LerpAnimator>();
            }
        }

        void Update() {
            if (isFindingPath && isAcceptingInput) {
                if (currentDirections.Count > 0) {
                    if (AttemptMoveInDirection(currentDirections[0])) {
                        currentDirections.RemoveAt(0);
                        tries = 0;
                    } else {
                        tries++;
                        if (tries >= maxTries) {
                            if (IsTheThingInfrontTheThingIWant(currentDirections[0])) {
                                currentDirections.RemoveAt(0);
                                tries = 0;
                            } else {
                                tries = 0;
                                overallTries++;
                                if (overallTries <= maxOverallTries) getDirections();
                                else throw new Exception("This shouldn't happen... The npc tried x times to follow directions to a node and failed all of them.");
                            }
                        }
                    }
                } else {
                    UpdateStandAnimation();
                    isFindingPath = false;
                    overallTries = 0;
                    callback();
                }
            }
        }

        public void Move(TaskStep taskStep, Action callback) {
            Initialize();
            this.callback = callback;
            var taskStepName = TaskStep.GetStepName(taskStep.type).ToLower();

            if (taskStep.node != null) {
                currentStepLocation = taskStep.node;
                getDirections();
            } else if (board.board.stepLocations.ContainsKey(taskStepName)) {
                var taskStepLocations = board.board.stepLocations[taskStepName];
                if (taskStepLocations.Count > 0) {
                    currentStepLocation = getStepLocation(taskStepLocations);
                    getDirections();
                } else {
                    throw new Exception("The board has an empty list of step locations with the task step name: " + taskStepName);
                }
            } else {
                throw new Exception("The board has no record of a step location with the task step name: " + taskStepName);
            }
        }

        private bool IsTheThingInfrontTheThingIWant(Direction dir) {
            var thingInFront = Peek(dir);
            return thingInFront == currentStepLocation;
        }

        private Board.Board.Node getStepLocation(List<Board.Board.Occupier> stepLocations) {
            // TODO: MW figure out if these step locations are currently available to be interacted with and then pick a random one
            return stepLocations[0].myNode;
        }

        private void getDirections() {
            currentDirections = Search.Navigate(board.board,
            occupier.myNode.IsoLoc(),
            currentStepLocation.IsoLoc());
            isFindingPath = true;
        }
        

        private bool AttemptMoveInDirection(Direction dir) {
            facing = DirectionToFacing(dir);
            if (DoMove(dir)) {
                UpdateWalkAnimation();
                var originalPos = boardPos.ToWorldPos();
                DirectionToBoardPosition(dir);
                StartLerp(originalPos, boardPos.ToWorldPos());
                return true;
            }
            UpdateStandAnimation();
            return false;
        }

        private void DirectionToBoardPosition(Direction dir) {
            switch (dir) {
                case Direction.Up:
                    boardPos.Y -= 1;
                    break;
                case Direction.Down:
                    boardPos.Y += 1;
                    break;
                case Direction.Left:
                    boardPos.X -= 1;
                    break;
                case Direction.Right:
                    boardPos.X += 1;
                    break;
            }
        }

        private void StartLerp(Vector2 originalPos, Vector2 endPos) {
            isAcceptingInput = false;
            standing = false;
            lerper.Begin(originalPos, endPos, TIME_TO_MOVE, () => { isAcceptingInput = true; });
        }

        private void UpdateStandAnimation() {
            if (!standing) {
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
        }

        private void UpdateWalkAnimation() {
            standing = false;
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

        private static Facing DirectionToFacing(Direction dir) {
            switch (dir) {
                case Direction.Up:
                    return Facing.Up;
                case Direction.Down:
                    return Facing.Down;
                case Direction.Left:
                    return Facing.Left;
                case Direction.Right:
                    return Facing.Right;
                default:
                    return Facing.Unknown;
            }
        }

        private bool DoMove(Direction dir) {
            switch (dir) {
                case Direction.Up:
                    return board.board.MoveUp(occupier);
                case Direction.Down:
                    return board.board.MoveDown(occupier);
                case Direction.Left:
                    return board.board.MoveLeft(occupier);
                case Direction.Right:
                    return board.board.MoveRight(occupier);
                default:
                    return false;
            }
        }

        private Board.Board.Node Peek(Direction dir) {
            switch (dir) {
                case Direction.Up:
                    return board.board.Get(occupier.myNode.x, occupier.myNode.y - 1);
                case Direction.Down:
                    return board.board.Get(occupier.myNode.x, occupier.myNode.y + 1);
                case Direction.Left:
                    return board.board.Get(occupier.myNode.x - 1, occupier.myNode.y);
                case Direction.Right:
                    return board.board.Get(occupier.myNode.x + 1, occupier.myNode.y);
            }

            return null;
        }
    }
}