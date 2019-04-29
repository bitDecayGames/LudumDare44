using System;
using System.Collections.Generic;
using System.Threading;
using Animations;
using Board;
using DefaultNamespace;
using UnityEngine;
using Utils;

namespace Movement {
    public class NpcMove : MonoBehaviour {
        public const float TIME_TO_MOVE = 0.4f;
        public const float TIME_TO_MOVE_VARIANCE = 0.075f;

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
        private float speed = TIME_TO_MOVE;
        private int tries = 0;
        private const int maxTries = 30;
        private int overallTries = 0;
        private const int maxOverallTries = 6;
        private Board.Board.Node currentStepLocation;
        private bool initialized = false;

        private float waitTime;
        private TaskStep _taskStep;

        [HideInInspector]
        public Facing facing;

        private void Start() {
            Initialize();
        }

        public void Initialize() {
            if (!initialized) {
                initialized = true;
                speed = UnityEngine.Random.Range(TIME_TO_MOVE - TIME_TO_MOVE_VARIANCE, TIME_TO_MOVE + TIME_TO_MOVE_VARIANCE);
                boardPos = GetComponent<BoardPosition>();
                animator = GetComponentInChildren<Animator>();
                occupier = GetComponent<Board.Board.Occupier>();
                if (occupier == null) occupier = gameObject.AddComponent<Board.Board.Occupier>();
                board = FindObjectOfType<BoardManager>();
                if (board == null) throw new Exception("Missing BoardManager in the Scene, just drag the script onto the Camera");
                lerper = GetComponentInChildren<LerpAnimator>();
                if (lerper == null) lerper = animator.gameObject.AddComponent<LerpAnimator>();
            }
        }

        void Update() {
            if (waitTime > 0)
            {
                waitTime -= Time.deltaTime;
                return;
            }
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
                                Finish();
                            } else {
                                waitTime = 2;
                                tries = 0;
                                overallTries++;
                                if (overallTries <= maxOverallTries) getDirections();
                                else throw new Exception(string.Format("This shouldn't happen... The npc tried {0} times to follow directions to a node and failed all of them from {1} to {2}", overallTries, occupier.myNode != null ? occupier.myNode.ToString() : "(null_node)", _taskStep));
                            }
                        }
                    }
                } else {
                    if (!occupier.myNode.IsoLoc().Equals(currentStepLocation.IsoLoc()))
                    {
                        getDirections();
                        waitTime = 2;
                    }
                    else
                    {
                        Finish();
                    }
                }
            }
        }

        private void Finish() {
//            UpdateStandAnimation();
            currentStepLocation.isBusy = null;
            isFindingPath = false;
            overallTries = 0;
            tries = 0;
            callback();
        }

        public void Move(TaskStep taskStep, Action callback) {
            Initialize();
            _taskStep = taskStep;
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
            if (dir == Direction.Wait) return false;
            var thingInFront = Peek(dir);
            return thingInFront == currentStepLocation && (thingInFront.isBusy == occupier || thingInFront.isBusy == null);
        }

        private Board.Board.Node getStepLocation(List<Board.Board.Occupier> stepLocations) {
            // TODO: MW figure out if these step locations are currently available to be interacted with and then pick a random one
            return stepLocations[0].myNode;
        }

        private void getDirections() {
            if (currentStepLocation.isBusy == occupier || currentStepLocation.isBusy == null) {
                if (occupier.myNode == null) throw new Exception(string.Format("This NPC({0}) doesn't have a node, it must have been 1) forcibly removed from the board or 2) was never added to the board in the first place.", gameObject.name));
                
                currentStepLocation.isBusy = occupier;
                currentDirections = Search.Navigate(board.board,
                    occupier.myNode.IsoLoc(),
                    currentStepLocation.IsoLoc());
                if (currentDirections.Count == 0) {
                    Debug.Log(string.Format("Received 0 directions from Search Algorithm from {0} to {1} for task step {2}", occupier.myNode, currentStepLocation, _taskStep));
                }
            } else {
                currentDirections = new List<Direction>{Direction.Wait};
            }
            isFindingPath = true;
        }
        

        private bool AttemptMoveInDirection(Direction dir) {
            if (dir == Direction.Wait) return false;
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
            lerper.Begin(originalPos, endPos, speed, () => { isAcceptingInput = true; });
        }

        private void playAnimationIfNotPlayingAsAFunction(string animName)
        {
            animator.Play(animName);
        }

        private void UpdateStandAnimation() {
            if (!standing) {
                standing = true;
                switch (facing) {
                    case Facing.Up:
                        playAnimationIfNotPlayingAsAFunction(AnimationConstants.STAND_UP_RIGHT);
                        break;
                    case Facing.Right:
                        playAnimationIfNotPlayingAsAFunction(AnimationConstants.STAND_DOWN_RIGHT);
                        break;
                    case Facing.Down:
                        playAnimationIfNotPlayingAsAFunction(AnimationConstants.STAND_DOWN_LEFT);
                        break;
                    case Facing.Left:
                        playAnimationIfNotPlayingAsAFunction(AnimationConstants.STAND_UP_LEFT);
                        break;
                }
            }
        }

        private void UpdateWalkAnimation() {
            standing = false;
            switch (facing) {
                case Facing.Up:
                    playAnimationIfNotPlayingAsAFunction(AnimationConstants.WALK_UP_RIGHT);
                    break;
                case Facing.Right:
                    playAnimationIfNotPlayingAsAFunction(AnimationConstants.WALK_DOWN_RIGHT);
                    break;
                case Facing.Down:
                    playAnimationIfNotPlayingAsAFunction(AnimationConstants.WALK_DOWN_LEFT);
                    break;
                case Facing.Left:
                    playAnimationIfNotPlayingAsAFunction(AnimationConstants.WALK_UP_LEFT);
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