using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Board {
    
    public class Board {
        public int width { get; private set; }
        public int height { get; private set; }
        private List<Node> board;

        public Board(int width, int height) {
            this.width = width;
            this.height = height;
            // Looks like:
            //
            // 0 4 8 
            // 1 5 9
            // 2 6 ...
            // 3 7
            board = new List<Node>();
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    var node = new Node(x, y);
                    board.Add(node);
                    if (x - 1 >= 0) {
                        node.left = Get(x - 1, y);
                        node.left.right = node;
                    }

                    if (y - 1 >= 0) {
                        node.up = Get(x, y - 1);
                        node.up.down = node;
                    }
                }
            }
        }

        public Node Get(int x, int y) {
            if (x < 0 || y < 0) return null;
            if (x > width || y > height) return null;
            return board[x * height + y];
        }

        public bool Set(Occupier occupier, int x, int y) {
            var node = Get(x, y);
            if (node == null) return false;
            if (node.occupier != null) return false;
            node.occupier = occupier;
            occupier.myNode = node;
            return true;
        }

        public void Remove(Occupier occupier) {
            occupier.myNode.occupier = null;
            occupier.myNode = null;
        }

        public bool MoveUp(Occupier occupier) {
            if (occupier.myNode == null) return false;
            return MoveTo(occupier, occupier.myNode.x, occupier.myNode.y + 1);
        }

        public bool MoveDown(Occupier occupier) {
            if (occupier.myNode == null) return false;
            return MoveTo(occupier, occupier.myNode.x, occupier.myNode.y - 1);
        }

        public bool MoveRight(Occupier occupier) {
            if (occupier.myNode == null) return false;
            return MoveTo(occupier, occupier.myNode.x + 1, occupier.myNode.y);
        }

        public bool MoveLeft(Occupier occupier) {
            if (occupier.myNode == null) return false;
            return MoveTo(occupier, occupier.myNode.x - 1, occupier.myNode.y);
        }

        public bool MoveTo(Occupier occupier, int goToX, int goToY) {
            var goingTo = Get(goToX, goToY);
            if (goingTo == null) return false;
            var comingFrom = occupier.myNode;
            // failed to move because something was already there
            if (goingTo.occupier != null) return false;
            // success, you have now moved
            if (comingFrom != null) comingFrom.occupier = null;
            goingTo.occupier = occupier;
            occupier.myNode = goingTo;
            return true;
        }

        public bool IsOccupied(int x, int y) {
            var node = Get(x, y);
            if (node == null) throw new Exception("invalid coordinate");
            return node.occupier != null;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    var node = Get(x, y);
                    sb.Append(node.occupier == null ? "." : "@");
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        public class Node {
            public int x;
            public int y;
            public Node up;
            public Node right;
            public Node down;
            public Node left;

            public Occupier occupier;
            
            public Node(int x, int y) {
                this.x = x;
                this.y = y;
            }
        }

        public class Occupier : MonoBehaviour {
            public Node myNode;
        }
    }
}