using System;
using System.Collections.Generic;
using SuperTiled2Unity;
using UnityEngine;
using Utils;
using Boo.Lang.Runtime;

namespace Board {
    public class BoardManager : MonoBehaviour {
        public Board board = new Board(-100, -100, 200, 200);
        private bool initialized = false;
        public bool isDebug; // change this in the UI
        
        private const float squareSize = 0.2f;

        private SuperMap map;

        private enum LayerAction {
            NONE,
            HIDE,
            DESTROY,
        }

        private void Start() {
            Initialize();
        }

        private void Update() {
            if (isDebug) DrawDebugSquares();
        }

        private void ForObjInLayer(Action<SuperObject> objF, LayerAction layerAction, params string[] layers) {
            if (map != null) {
                foreach (var componentsInChild in map.GetComponentsInChildren<SuperObjectLayer>()) {
                    foreach (var layerName in layers) {
                        if (layerName.ToLower().Trim() == componentsInChild.m_TiledName.ToLower().Trim()) {
                            foreach (var superObject in componentsInChild.GetComponentsInChildren<SuperObject>()) {
                                objF(superObject);
                            }

                            switch (layerAction) {
                                case LayerAction.HIDE:
                                    componentsInChild.gameObject.SetActive(false);
                                    break;
                                case LayerAction.DESTROY:
                                    Destroy(componentsInChild.gameObject);
                                    break;
                            }
                            break;
                        }
                    }
                }
            }
        }

        public void Initialize() {
            if (!initialized) {
                initialized = true;
                map = FindObjectOfType<SuperMap>();
                if (map != null) {
                    ForObjInLayer(HandleOffLimitLayer, LayerAction.HIDE, "OffLimits", "NpcOnlyCollisions", "NpcOnlyCollision", "OffLimit");
                    ForObjInLayer(HandleCollisionsLayer, LayerAction.HIDE, "InvisibleCollisions", "InvisibleCollision");
                    ForObjInLayer(HandleCollisionsLayer, LayerAction.NONE, "Collisions", "Collision", "Interactable", "Interactables");
                    ForObjInLayer(HandlePOIsLayer, LayerAction.NONE, "POIs", "POI");
                }
            }
        }

        private void HandleOffLimitLayer(SuperObject superObject) {
            IsoVector2 superObjBoardPos = IsoVector2.GridCoordsToBoard(superObject.m_X, superObject.m_Y);
            var x = superObjBoardPos.x;
            var y = superObjBoardPos.y;
            board.Get(x, y).npcOffLimits = true;
        }

        private void HandleCollisionsLayer(SuperObject superObject) {
            IsoVector2 superObjBoardPos = IsoVector2.GridCoordsToBoard(superObject.m_X, superObject.m_Y);
            var x = superObjBoardPos.x;
            var y = superObjBoardPos.y;
            var occupier = superObject.gameObject.AddComponent<Board.Occupier>();
            var props = superObject.GetComponent<SuperCustomProperties>();
            if (props != null) {
                foreach (var p in props.m_Properties) {
                    if (p.m_Name == "TaskStepType") {
                        string key = p.m_Value.ToLower();
                        if (!board.stepLocations.ContainsKey(key)) {
                            board.stepLocations.Add(key, new List<Board.Occupier>());
                        }

                        board.stepLocations[key].Add(occupier);
                    }
                }
            }

            if (!board.Set(occupier, x, y)) {
                Debug.Log(string.Format("Failed to set ({0}, {1}): {2}", x, y, superObject));
            }
        }

        private void HandlePOIsLayer(SuperObject superObject) {
            IsoVector2 superObjBoardPos = IsoVector2.GridCoordsToBoard(superObject.m_X, superObject.m_Y);
            var x = superObjBoardPos.x;
            var y = superObjBoardPos.y;
            var props = superObject.GetComponent<SuperCustomProperties>();
            if (props != null) {
                foreach (var p in props.m_Properties) {
                    if (p.m_Name == "poiName") {
                        string key = p.m_Value.ToLower();
                        if (!board.poiLocations.ContainsKey(key)) {
                            board.poiLocations.Add(key, new List<Board.POI>());
                        }

                        Board.Node poiNode = board.Get(x, y);
                        Board.POI poi = new Board.POI();
                        poi.myNode = poiNode;
                        poiNode.poi = poi;
                        board.poiLocations[key].Add(poi);
                    }

                    if (p.m_Name == "LineNumber") {
                        if (!board.lineLocations.ContainsKey(p.m_Value.ToInt())) {
                            board.lineLocations.Add(p.m_Value.ToInt(), new List<Board.POI>());
                        }

                        if (superObject.m_TiledName == "LineStart") {
                            SuperObject line = GetLine(superObjBoardPos);
                            if (line != null) {
                                // Debug.Log("We got a line mother fucker: " + x + ", " + y);
                                var linePoints = line.GetComponent<EdgeCollider2D>().points;

                                Vector2 lastPoint = Vector2.negativeInfinity;
                                var lastNodeCoords = new IsoVector2(x, y);
                                foreach (var point in linePoints) {
                                    var nextNodeCoords = new IsoVector2();
                                    if (lastPoint.Equals(Vector2.negativeInfinity)) {
                                        nextNodeCoords.x = lastNodeCoords.x;
                                        nextNodeCoords.y = lastNodeCoords.y;
                                        lastPoint = point;
                                    } else {
                                        var diff = point - lastPoint;
                                        if (Mathf.Abs(diff.x) != 0.5f) {
                                            throw new RuntimeException("Line x coords are bad: " + diff);
                                        }

                                        if (Mathf.Abs(diff.y) != 0.25f) {
                                            throw new RuntimeException("Line y coords are bad" + diff);
                                        }

                                        lastPoint = point;


                                        if (diff.x > 0 && diff.y > 0) {
                                            // UP
                                            nextNodeCoords.x = lastNodeCoords.x;
                                            nextNodeCoords.y = lastNodeCoords.y - 1;
                                        } else if (diff.x > 0 && diff.y < 0) {
                                            // RIGHT
                                            nextNodeCoords.x = lastNodeCoords.x + 1;
                                            nextNodeCoords.y = lastNodeCoords.y;
                                        } else if (diff.x < 0 && diff.y > 0) {
                                            // LEFT
                                            nextNodeCoords.x = lastNodeCoords.x - 1;
                                            nextNodeCoords.y = lastNodeCoords.y;
                                        } else if (diff.x < 0 && diff.y < 0) {
                                            // DOWN
                                            nextNodeCoords.x = lastNodeCoords.x;
                                            nextNodeCoords.y = lastNodeCoords.y + 1;
                                        }
                                    }

                                    lastNodeCoords = nextNodeCoords;

                                    // Debug.Log("next point: " + nextNodeCoords.x + ", " + nextNodeCoords.y);
                                    Board.Node lineNode = board.Get(nextNodeCoords.x, nextNodeCoords.y);
                                    Board.POI linePOI = new Board.POI();
                                    linePOI.myNode = lineNode;
                                    lineNode.poi = linePOI;
                                    foreach (var innerProp in props.m_Properties)
                                    {
                                        if (innerProp.m_Name == "ForTask")
                                        {
                                            linePOI.Meta = innerProp.m_Value;
                                        }
                                    }
                                    board.lineLocations[p.m_Value.ToInt()].Add(linePOI);
                                }
                            } else {
                                throw new RuntimeException("Line " + p.m_Value + " has not been FOUND!!");
                            }
                        }
                    }
                }
            }
        }

        private SuperObject GetLine(IsoVector2 lineStartBoard) {
            // Debug.Log("Line start board pos: " + lineStartBoard.x + " " + lineStartBoard.y);
            SuperMap map = FindObjectOfType<SuperMap>();
            if (map != null) {
                foreach (var componentsInChild in map.GetComponentsInChildren<SuperObjectLayer>()) {
                    if ("Line" == componentsInChild.m_TiledName) {
                        foreach (var superObject in componentsInChild.GetComponentsInChildren<SuperObject>()) {
                            IsoVector2 linePos = IsoVector2.GridCoordsToBoard(superObject.m_X, superObject.m_Y);
                            // Debug.Log("Line pos: " + linePos.x + " " + linePos.y);

                            // if (IsoVector2.WorldToBoardPos((int) superObject.m_X, (int) superObject.m_Y).Equals(lineStartBoard)){
                            if (linePos.Equals(lineStartBoard)) {
                                return superObject;
                            }
                        }
                    }
                }
            }

            return null;
        }

        private void DrawDebugSquares() {
            board.ForEach(n => {
                Color color;
                if (n.occupier != null) color = Color.green;
                else if (n.isBusy != null) color = Color.yellow;
                else if (n.npcOffLimits) color = Color.red;
                else if (n.poi != null) color = Color.cyan;
                else return;
                DrawSquareAt(n.IsoLoc(), color);
            });
        }

        private void DrawSquareAt(IsoVector2 iso, Color color) {
            var middle = iso.ToWorldPosReadable() + new Vector2(-squareSize / 2f, squareSize / 2f);
            var lines = new List<Vector3>();
            lines.Add(middle);
            lines.Add(new Vector3(middle.x + squareSize, middle.y));
            lines.Add(new Vector3(middle.x + squareSize, middle.y - squareSize));
            lines.Add(new Vector3(middle.x, middle.y - squareSize));
            lines.Add(middle);

            for (int i = 0; i + 1 < lines.Count; i++) {
                var a = lines[i];
                var b = lines[i + 1];
                Debug.DrawLine(a, b, color, 0, false);
            }
        }
    }
}