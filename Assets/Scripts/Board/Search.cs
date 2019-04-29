  using System;
  using System.Collections.Generic;
  using Boo.Lang.Runtime;
  using UnityEngine;
  using Utils;

  public class Search
    {
        public class NodePath
        {
            public Board.Board.Node currentNode;
            public System.Collections.Generic.List<Direction> path;
            public int weight;

            public NodePath(Board.Board.Node n, System.Collections.Generic.List<Direction> p, int remaining)
            {
                currentNode = n;
                path = p;

                weight = path.Count + remaining;
            }
        }
        
        public static System.Collections.Generic.List<Direction> Navigate(Board.Board board, IsoVector2 start, IsoVector2 dest)
        {
            Board.Board.Node node = board.Get(start.x, start.y);
            if (node == null)
            {
                throw new RuntimeException("Started a search from a non-existent node: " + node);
            }

            PriorityQueue startFringe = new PriorityQueue();
            foreach (var initial in expand(new NodePath(node, new List<Direction>(), start.distance(dest)), board, dest))
            {
                startFringe.Push(initial);
            }
            return doSearch(board, dest, startFringe, new List<Board.Board.Node>());
        }

        // Using a priority queue would be much more efficient
        private static System.Collections.Generic.List<Direction> doSearch(Board.Board board, IsoVector2 dest, PriorityQueue fringe, System.Collections.Generic.List<Board.Board.Node> visited)
        {
            var start = Time.time * 1000;
            var iterations = 0;
            while (true)
            {
                iterations++;
                if (iterations > 500) {
                    var cur = fringe != null && fringe.GetFirstNode() != null ? fringe.GetFirstNode().currentNode : new Board.Board.Node(-1, -1);
                    Debug.Log(string.Format("Unable to find path from {0} to destination {1}", cur, dest));
                    return new List<Direction>();
                }
                
                if (fringe.Length <= 0)
                {
                    Debug.Log("Visited " + visited.Count + " nodes before failing");
                    return new List<Direction>();
                }
                
                NodePath checkPath = fringe.GetFirstNode();
                fringe.Remove(checkPath);
                visited.Add(checkPath.currentNode);

                if (dest.Equals(checkPath.currentNode.x, checkPath.currentNode.y))
                {
//                    Debug.Log("Took " + iterations + " iterations to complete");
//                    Debug.Log("Explored " + visited.Count + " nodes");
//                    Debug.Log("Fringe left with " + fringe.Length + " unexplored items");
//                    Debug.Log("Took " + ((Time.time * 1000 - start)) + " milliseconds to complete");
                    return checkPath.path;
                }
                
                if (checkPath.currentNode.occupier != null)
                {
                    // Can't walk through occupied spaces
                    continue;
                }

                if (checkPath.currentNode.npcOffLimits)
                {
                    // Can't go here
                    continue;
                }
                
                foreach (var next in expand(checkPath, board, dest))
                {
                    if (!visited.Contains(next.currentNode))
                    {
                        fringe.Push(next);
                    }
                }
            }
        }

        public static System.Collections.Generic.List<NodePath> expand(NodePath p, Board.Board board, IsoVector2 dest)
        {
           // Debug.Log("Expanding: " + p.currentNode.x + ", " + p.currentNode.y);
            System.Collections.Generic.List<NodePath> nexts = new System.Collections.Generic.List<NodePath>();

            Board.Board.Node check = p.currentNode.down;
            if (check != null)
            {
                NodePath newP = new NodePath(check, new System.Collections.Generic.List<Direction>(p.path), dest.distance(check.x, check.y));
                newP.path.Add(Direction.Down);
                nexts.Add(newP);
            }
            
            check = p.currentNode.up;
            if (check != null)
            {
                NodePath newP = new NodePath(check, new System.Collections.Generic.List<Direction>(p.path), dest.distance(check.x, check.y));
                newP.path.Add(Direction.Up);
                nexts.Add(newP);
            }
            
            check = p.currentNode.right;
            if (check != null)
            {
                NodePath newP = new NodePath(check, new System.Collections.Generic.List<Direction>(p.path), dest.distance(check.x, check.y));
                newP.path.Add(Direction.Right);
                nexts.Add(newP);
            }
            
            check = p.currentNode.left;
            if (check != null)
            {
                NodePath newP = new NodePath(check, new System.Collections.Generic.List<Direction>(p.path), dest.distance(check.x, check.y));
                newP.path.Add(Direction.Left);
                nexts.Add(newP);
            }

           // Debug.Log("Found " + nexts.Count + " expansions");
            return nexts;
        }

        
    }