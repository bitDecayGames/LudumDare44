using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class BoardPosition : MonoBehaviour {
    public int X {
        get { return boardPos.x; }
        set {
            boardPos.x = value;
            UpdatePos();
        }
    }
    
    public int Y {
        get { return boardPos.y; }
        set {
            boardPos.y = value;
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

    private IsoVector2 boardPos = new IsoVector2();

    private void UpdatePos() {
        transform.localPosition = boardPos.ToWorldPos();
    }
}