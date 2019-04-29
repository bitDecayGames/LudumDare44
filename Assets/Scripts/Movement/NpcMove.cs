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

        public bool isDebug; // change this through the UI

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

        private SpriteRenderer _spriteRenderer;

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
                _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
        }

        void Update() {
            if (isDebug) DrawDebugLines();
            if (waitTime > 0)
            {
                waitTime -= Time.deltaTime;
                if (waitTime <= 0)
                {
                    if (currentDirections.Count > 0 && currentDirections[0] == Direction.Wait)
                    {
                        currentDirections.Clear();
                    }
                }
                return;
            }
            if (isFindingPath && isAcceptingInput) {
                if (currentDirections.Count > 0) {
                    NpcHelper.SmartNamer(gameObject, _taskStep.type, currentDirections[0]);
                    if (AttemptMoveInDirection(currentDirections[0])) {
                        _spriteRenderer.color = Color.white;
                        currentDirections.RemoveAt(0);
                        tries = 0;
                    } else {
                        if (currentDirections[0] == Direction.Wait) {
                            getDirections();
                            if (isDebug) {
                                _spriteRenderer.color = Color.yellow;
                                Debug.DrawLine(occupier.myNode.IsoLoc().ToWorldPosReadable(), currentStepLocation.IsoLoc().ToWorldPosReadable(), Color.red, 0, true);
                            }
                            return; // don't increment tries while waiting
                        }
                        tries++;
                        if (tries >= maxTries) {
                            if (IsTheThingInfrontTheThingIWant(currentDirections[0])) {
                                currentDirections.RemoveAt(0);
                                Finish();
                            } else {
                                waitTime = 1;
                                tries = 0;
                                overallTries++;
                                if (overallTries <= maxOverallTries) getDirections();
                                else throw new Exception(string.Format("This shouldn't happen... The npc tried {0} times to follow directions to a node and failed all of them from {1} to {2}\n{3}\n{4}", overallTries, occupier.myNode != null ? occupier.myNode.ToString() : "(null_node)", _taskStep, string.Join("->", currentDirections.ConvertAll(c => c.ToString()).ToArray()), board.board));
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
            var taskStepName = TaskStep.GetStepName(taskStep.type);

            if (taskStep.node != null) {
                currentStepLocation = taskStep.node;
                getDirections();
            } else {
                var possibleTaskNodes = getValidNodeLocations(taskStepName);
                if (possibleTaskNodes.Count > 0) {
                    currentStepLocation = getRandomStepLocation(possibleTaskNodes);
                    getDirections();
                } else {
                    throw new Exception("The board has an empty list of step locations with the task step name: " + taskStepName);
                    throw new Exception("The board has no record of a step location with the task step name: " + taskStepName);
                }
            }
        }

        private void DrawDebugLines() {
            List<Vector3> lines = new List<Vector3>();
            var cur = boardPos.CopyIsoVector2();
            lines.Add(cur.ToWorldPosReadable());
            if (currentDirections != null)
            {
                currentDirections.ForEach(dir => {
                    lines.Add(DirectionAddToIsoVector(dir, cur).ToWorldPosReadable());
                });
            }
            for (int i = 0; i + 1 < lines.Count; i++) {
                var a = lines[i];
                var b = lines[i + 1];
                Debug.DrawLine(a, b, Color.cyan, 0, true);
            }
        }

        private bool IsTheThingInfrontTheThingIWant(Direction dir) {
            if (dir == Direction.Wait) return false;
            var thingInFront = Peek(dir);
            return thingInFront == currentStepLocation && (thingInFront.isBusy == occupier || thingInFront.isBusy == null);
        }

        private List<Board.Board.Node> getValidNodeLocations(string taskStepName) {
            List<Board.Board.Node> nodes = new List<Board.Board.Node>();
            if (board.board.stepLocations.ContainsKey(taskStepName.ToLower())) {
                nodes.AddRange(board.board.stepLocations[taskStepName.ToLower()].ConvertAll(o => o.myNode));
            }
            if (board.board.poiLocations.ContainsKey(taskStepName.ToLower())) {
                nodes.AddRange(board.board.poiLocations[taskStepName.ToLower()].ConvertAll(o => o.myNode));
            }
            return nodes.FindAll(n => n != null);
        }

        private Board.Board.Node getRandomStepLocation(List<Board.Board.Node> stepLocations) {
            var nonBusyNodes = stepLocations.FindAll(n => n.isBusy == null || n.isBusy == occupier);
            // find a non busy node if it exists
            if (nonBusyNodes.Count > 0) return nonBusyNodes[UnityEngine.Random.Range(0, nonBusyNodes.Count)];
            // if all nodes are busy, queue one up anyways so that the wait logic works
            return stepLocations[UnityEngine.Random.Range(0, stepLocations.Count)];
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
                Debug.Log("Someone is already in my node. I'm waiting");
                currentDirections = new List<Direction>{Direction.Wait};
                waitTime = 2;
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

        private IsoVector2 DirectionAddToIsoVector(Direction dir, IsoVector2 v) {
            switch (dir) {
                case Direction.Up:
                    v.y -= 1;
                    break;
                case Direction.Down:
                    v.y += 1;
                    break;
                case Direction.Left:
                    v.x -= 1;
                    break;
                case Direction.Right:
                    v.x += 1;
                    break;
            }

            return v;
        }
    }
}