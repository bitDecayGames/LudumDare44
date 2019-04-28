using System.Collections.Generic;
using SuperTiled2Unity;
using UnityEngine;
using Utils;

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
                        if ("Collisions" == componentsInChild.m_TiledName || "Interactables" == componentsInChild.m_TiledName) {
                            foreach (var superObject in componentsInChild.GetComponentsInChildren<SuperObject>()) {
                                // HACK: everything has to be -2, -2 before it goes onto the board state... ask Logan
                                var x = (int) superObject.m_X / 8 - 2; // 8 is tile size
                                var y = (int) superObject.m_Y / 8 - 2; // 8 is tile size
                                var occupier = superObject.gameObject.AddComponent<Board.Occupier>();
                                var props = superObject.GetComponent<SuperCustomProperties>();
                                if (props != null)
                                {
                                    foreach (var p in props.m_Properties)
                                    {
                                        if (p.m_Name == "stepName")
                                        {
                                            if (!board.stepLocations.ContainsKey(p.m_Value))
                                            {
                                                board.stepLocations.Add(p.m_Value, new List<Board.Occupier>());
                                            }
                                            board.stepLocations[p.m_Value].Add(occupier);
                                        }
                                    }
                                } 
                                if (!board.Set(occupier, x, y)) {
                                    Debug.Log(string.Format("Failed to set ({0}, {1}): {2}", x, y, superObject));
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}