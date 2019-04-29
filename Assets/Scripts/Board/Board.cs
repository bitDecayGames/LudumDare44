using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Analytics;
using Utils;

namespace Board {
    
    public class Board {
        public int width { get; private set; }
        public int height { get; private set; }
        public int startX { get; private set; }
        public int startY { get; private set; }
        private List<Node> board;

        public Dictionary<string, List<Occupier>> stepLocations = new Dictionary<string, List<Occupier>>();
        public Dictionary<string, List<POI>> poiLocations = new Dictionary<string, List<POI>>();
        public Dictionary<int, List<POI>> lineLocations = new Dictionary<int, List<POI>>();
        

        public Board(int startX, int startY, int width, int height) {
            this.width = width;
            this.height = height;
            this.startX = startX;
            this.startY = startY;
            // Looks like:
            //
            // 0 4 8 
            // 1 5 9
            // 2 6 ...
            // 3 7
            board = new List<Node>();
            for (int x = startX; x < startX + width; x++) {
                for (int y = startY; y < startY + height; y++) {
                    var node = new Node(x, y);
                    board.Add(node);
                    if (x - 1 >= startX) {
                        node.left = Get(x - 1, y);
                        node.left.right = node;
                    }

                    if (y - 1 >= startY) {
                        node.up = Get(x, y - 1);
                        node.up.down = node;
                    }
                }
            }
        }

        public void ForEach(Action<Node> nodeF) {
            board.ForEach(nodeF);
        }

        public Node Get(int x, int y) {
            if (x < startX || y < startY) return null;
            if (x > startX + width || y > startY + height) return null;
            return board[(x - startX) * height + (y - startY)];
        }

        public bool Set(Occupier occupier, int x, int y) {
            var node = Get(x, y);
            if (node == null) return false;
            if (node.occupier != null) return false;
            node.occupier = occupier;
            occupier.myNode = node;
            return true;
        }

        public bool SetForce(Occupier occupier, int x, int y) {
            var node = Get(x, y);
            if (node == null) return false;
            if (node.occupier != null) {
                Debug.Log("Forcibly removed something from the board at (" + x + ", " + y + ")");
                node.occupier.myNode = null;
                node.occupier = null;
            }
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
            return MoveTo(occupier, occupier.myNode.x, occupier.myNode.y - 1);
        }

        public bool MoveDown(Occupier occupier) {
            if (occupier.myNode == null) return false;
            return MoveTo(occupier, occupier.myNode.x, occupier.myNode.y + 1);
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
            var topLeftX = startX + width;
            var topLeftY = startY + height;
            var bottomRightX = startX;
            var bottomRightY = startY;
            
            board.ForEach(n => {
                if (n.occupier != null) {
                    if (topLeftX > n.x) topLeftX = n.x;
                    if (topLeftY > n.y) topLeftY = n.y;
                    if (bottomRightX < n.x) bottomRightX = n.x;
                    if (bottomRightY < n.y) bottomRightY = n.y;
                }
            });
            StringBuilder sb = new StringBuilder();
            for (int y = topLeftY; y <= bottomRightY; y++) {
                for (int x = topLeftX; x <= bottomRightX; x++) {
                    var node = Get(x, y);
                    if (node.occupier != null) sb.Append("@");
                    else if (node.npcOffLimits) sb.Append("#");
                    else if (node.poi != null) sb.Append("+");
                    else sb.Append(".");
                }

                sb.AppendLine();
            }

            sb.AppendLine();
            sb.Append("Add these to the Line and Column value to get the correct coordinate:").AppendLine();
            sb.Append("(").Append(topLeftX - 1).Append(", ").Append(topLeftY - 1).Append(")").AppendLine();

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
            public POI poi;
            public Occupier isBusy = null;
            public bool npcOffLimits;

            public Node(int x, int y) {
                this.x = x;
                this.y = y;
            }

            public IsoVector2 IsoLoc()
            {
                return new IsoVector2(x, y);
            }

            public override string ToString() {
                return string.Format("({0}, {1})", x, y);
            }
        }

        public class Occupier : MonoBehaviour {
            public Node myNode;

            private void OnDestroy() {
                if (myNode != null) {
                    myNode.occupier = null;
                    if (myNode.isBusy == this) myNode.isBusy = null;
                }
            }
        }

        public class POI
        {
            public Node myNode;
            public string Meta { get; set; }
        }
    }
}