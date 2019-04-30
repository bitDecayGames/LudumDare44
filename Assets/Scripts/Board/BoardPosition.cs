using UnityEngine;
using Utils;

namespace Board {
    public class BoardPosition : MonoBehaviour {

        public Vector2 pos = new Vector2();
        
        public int X {
            get { return boardPos.x; }
            set {
                boardPos.x = value;
                pos.x = value;
                UpdatePos();
            }
        }
    
        public int Y {
            get { return boardPos.y; }
            set {
                boardPos.y = value;
                pos.y = value;
                UpdatePos();
            }
        }

        public Vector2 ToWorldPos() {
            return boardPos.ToWorldPos();
        }

        public BoardPosition Add(int x, int y) {
            boardPos.Add(x, y);
            UpdatePos();
            return this;
        }

        public IsoVector2 CopyIsoVector2() {
            return new IsoVector2(boardPos.x, boardPos.y, boardPos.z);
        }

        private IsoVector2 boardPos = new IsoVector2();

        private void UpdatePos() {
            transform.localPosition = boardPos.ToWorldPos();
        }

    }
}