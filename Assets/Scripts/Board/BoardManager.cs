using SuperTiled2Unity;
using UnityEngine;

namespace Board {
    public class BoardManager : MonoBehaviour {
        public Board board;
        
        void Start() {
            board = new Board(100, 100);
            SuperMap map = FindObjectOfType<SuperMap>();
            foreach (var componentsInChild in map.GetComponentsInChildren<SuperObjectLayer>())
            {
                if ("Collisions" == componentsInChild.m_TiledName || "Interactables" == componentsInChild.m_TiledName)
                {
                    foreach (var superObject in map.GetComponentsInChildren<SuperObject>())
                    {
                        var x = (int)superObject.m_X / 8; // 8 is tile size
                        var y = (int)superObject.m_Y / 8; // 8 is tile size
                        var occupier = superObject.gameObject.AddComponent<Board.Occupier>();
                        if (!board.Set(occupier, x, y)) {
                            Debug.Log(string.Format("Failed to set ({0}, {1}): {2}", x, y,superObject));
                        }
                    }
                }
            }
            
            Debug.Log(board);
        }
    }
}