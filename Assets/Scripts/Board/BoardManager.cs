using SuperTiled2Unity;
using UnityEngine;

namespace Board {
    public class BoardManager : MonoBehaviour {
        public Board board;
        
        void Start() {
            board = new Board(-100, -100, 200, 200);
            SuperMap map = FindObjectOfType<SuperMap>();
            foreach (var componentsInChild in map.GetComponentsInChildren<SuperObjectLayer>())
            {
                if ("Collisions" == componentsInChild.m_TiledName)
                {
                    foreach (var superObject in componentsInChild.GetComponentsInChildren<SuperObject>())
                    {
                        // HACK: everything has to be -2, -2 before it goes onto the board state... ask Logan
                        var x = (int)superObject.m_X / 8 - 2; // 8 is tile size
                        var y = (int)superObject.m_Y / 8 - 2; // 8 is tile size
                        var occupier = superObject.gameObject.AddComponent<Board.Occupier>();
                        if (!board.Set(occupier, x, y)) {
                            Debug.Log(string.Format("Failed to set ({0}, {1}): {2}", x, y,superObject));
                        }
                    }
                }
            }
        }
    }
}