using System.Collections.Generic;
using SuperTiled2Unity;
using UnityEngine;
using Utils;
using Boo.Lang.Runtime;

namespace Board {
    public class BoardManager : MonoBehaviour {
        public Board board = new Board(-100, -100, 200, 200);
        private bool initialized = false;

        private void Start() {
            Initialize();
        }

        public void Initialize() {
            if (!initialized) {
                initialized = true;
                SuperMap map = FindObjectOfType<SuperMap>();
                if (map != null) {
                    foreach (var componentsInChild in map.GetComponentsInChildren<SuperObjectLayer>()) {
                        if ("OffLimits" == componentsInChild.m_TiledName)
                        {
                            foreach (var superObject in componentsInChild.GetComponentsInChildren<SuperObject>())
                            {

                                IsoVector2 superObjBoardPos =
                                    IsoVector2.GridCoordsToBoard(superObject.m_X, superObject.m_Y);
                                // var x = (int) superObject.m_X / 8 - 2; // 8 is tile size
                                // var y = (int) superObject.m_Y / 8 - 2; // 8 is tile size
                                var x = superObjBoardPos.x;
                                var y = superObjBoardPos.y;
                                board.Get(x, y).npcOffLimits = true;
                            }

                            // Once we've scanned this, no need to render it
                            Destroy(componentsInChild.gameObject);
                        }
                        if ("Collisions" == componentsInChild.m_TiledName || "Interactables" == componentsInChild.m_TiledName) {
                            foreach (var superObject in componentsInChild.GetComponentsInChildren<SuperObject>()) {
                                IsoVector2 superObjBoardPos = IsoVector2.GridCoordsToBoard(superObject.m_X, superObject.m_Y);
                                // var x = (int) superObject.m_X / 8 - 2; // 8 is tile size
                                // var y = (int) superObject.m_Y / 8 - 2; // 8 is tile size
                                var x = superObjBoardPos.x;
                                var y = superObjBoardPos.y;
                                var occupier = superObject.gameObject.AddComponent<Board.Occupier>();
                                var props = superObject.GetComponent<SuperCustomProperties>();
                                if (props != null)
                                {
                                    foreach (var p in props.m_Properties)
                                    {
                                        if (p.m_Name == "TaskStepType")
                                        {
                                            string key = p.m_Value.ToLower();
                                            if (!board.stepLocations.ContainsKey(key))
                                            {
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
                        }
                        else if ("POIs" == componentsInChild.m_TiledName)
                        {    
                            foreach (var superObject in componentsInChild.GetComponentsInChildren<SuperObject>()) {
                                IsoVector2 superObjBoardPos = IsoVector2.GridCoordsToBoard(superObject.m_X, superObject.m_Y);
                                var x = superObjBoardPos.x;
                                var y = superObjBoardPos.y;
                                var props = superObject.GetComponent<SuperCustomProperties>();
                                if (props != null)
                                {
                                    foreach (var p in props.m_Properties)
                                    {
                                        if (p.m_Name == "poiName")
                                        {
                                            string key = p.m_Value;
                                            if (!board.poiLocations.ContainsKey(key))
                                            {
                                                board.poiLocations.Add(key, new List<Board.POI>());
                                            }
                                            Board.Node poiNode = board.Get(x, y);
                                            Board.POI poi = new Board.POI();
                                            poi.myNode = poiNode;
                                            poiNode.poi = poi;
                                            board.poiLocations[key].Add(poi);
                                        }
                                        if (p.m_Name == "LineNumber") {
                                            if (!board.lineLocations.ContainsKey(p.m_Value.ToInt()))
                                            {
                                                board.lineLocations.Add(p.m_Value.ToInt(), new List<Board.POI>());
                                            }
                                            if(superObject.m_TiledName == "LineStart"){
                                                SuperObject line = GetLine(superObjBoardPos);
                                                if (line != null)
                                                {
                                                    // Debug.Log("We got a line mother fucker: " + x + ", " + y);
                                                    var linePoints = line.GetComponent<EdgeCollider2D>().points;

                                                    Vector2 lastPoint = Vector2.negativeInfinity;
                                                    var lastNodeCoords = new IsoVector2(x, y);
                                                    foreach (var point in linePoints)
                                                    {
                                                        var nextNodeCoords = new IsoVector2();
                                                        if (lastPoint.Equals(Vector2.negativeInfinity)) {
                                                            nextNodeCoords.x = lastNodeCoords.x;
                                                            nextNodeCoords.y = lastNodeCoords.y;
                                                            lastPoint = point;
                                                        } else {
                                                            var diff = point - lastPoint;
                                                            if (Mathf.Abs(diff.x) != 0.5f) {
                                                                throw new RuntimeException("Line x coords are bad");
                                                            }
                                                            if (Mathf.Abs(point.y - lastPoint.y) != 0.25f) {
                                                                throw new RuntimeException("Line y coords are bad");
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
                        }
                    }
                }
            }
            // Debug.Log("Dictionary size: " + board.stepLocations.Count);
            // foreach (KeyValuePair<string, List<Board.Occupier>> entry in board.stepLocations)
            // {
            //     Debug.Log("key " + entry.Key);
            //     Debug.Log("occupier: " + entry.Value.Count);
            // }
            // Debug.Log("Dictionary size: " + board.lineLocations.Count);
            // foreach (KeyValuePair<int, List<Board.Occupier>> entry in board.lineLocations)
            // {
            //     Debug.Log("key" + entry.Key);
            //     foreach(var occupier in entry.Value)
            //     {
            //         Debug.Log("Point: " + occupier.myNode.x + ", " + occupier.myNode.y);
            //     }
            // }
        }

        private SuperObject GetLine(IsoVector2 lineStartBoard)
        {
            // Debug.Log("Line start board pos: " + lineStartBoard.x + " " + lineStartBoard.y);
            SuperMap map = FindObjectOfType<SuperMap>();
            if (map != null) {
                foreach (var componentsInChild in map.GetComponentsInChildren<SuperObjectLayer>()) {
                    if ("Line" == componentsInChild.m_TiledName) {
                        foreach (var superObject in componentsInChild.GetComponentsInChildren<SuperObject>()) {
                            IsoVector2 linePos = IsoVector2.GridCoordsToBoard(superObject.m_X, superObject.m_Y);
                            // Debug.Log("Line pos: " + linePos.x + " " + linePos.y);

                            // if (IsoVector2.WorldToBoardPos((int) superObject.m_X, (int) superObject.m_Y).Equals(lineStartBoard)){
                            if (linePos.Equals(lineStartBoard)){
                                return superObject;
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}