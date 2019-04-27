  using System.Collections.Generic;
  using Boo.Lang.Runtime;

  public class Search
    {
        public class NodePath
        {
            public Board.Board.Node currentNode;
            public System.Collections.Generic.List<Direction> path;

            public NodePath(Board.Board.Node n, System.Collections.Generic.List<Direction> p)
            {
                currentNode = n;
                path = p;
            }
        }
        
        public static System.Collections.Generic.List<Direction> Navigate(Board.Board board, int startX, int startY, string type)
        {
            Board.Board.Node node = board.Get(startX, startY);
            if (node == null)
            {
                throw new RuntimeException("Started a search from a non-existent node: " + node);
            }

            Queue<NodePath> startFringe = new Queue<NodePath>();
            startFringe.Enqueue(new NodePath(node, new List<Direction>()));
            return doSearch(board, type, startFringe, new List<NodePath>());
        }

        // Using a priority queue would be much more efficient
        private static System.Collections.Generic.List<Direction> doSearch(Board.Board board, string target, Queue<NodePath> fringe, System.Collections.Generic.List<NodePath> visited)
        {
            while (true)
            {
                if (fringe.Count <= 0)
                {
                    throw new RuntimeException("No nodes to search for " + target + ". Boom");
                }
                
                NodePath checkPath = fringe.Dequeue();
                if (checkPath.currentNode.occupier != null)
                {
                    if (checkPath.currentNode.occupier.name == target)
                    {
                        return checkPath.path;
                    } 
                }
                
                visited.Add(checkPath);
                foreach (var next in expand(checkPath, board))
                {
                    foreach (var visitCheck in visited)
                    {
                        // we've already been to this node.
                        if (visitCheck.path.Count < next.path.Count)
                        {
                            // we've been here in a shorter path
                            continue;
                        }
                        else
                        {
                            // we found a shorter path to the same thing, UPDATE
                            // TODO: update
                            continue;
                        }
                    }  
                    fringe.Enqueue(next);                    
                }
            }
        }

        public static System.Collections.Generic.List<NodePath> expand(NodePath p, Board.Board board)
        {
            System.Collections.Generic.List<NodePath> nexts = new System.Collections.Generic.List<NodePath>();

            Board.Board.Node check = board.Get(p.currentNode.x, p.currentNode.y + 1);
            if (check != null)
            {
                NodePath newP = new NodePath(check, new System.Collections.Generic.List<Direction>(p.path));
                newP.path.Add(Direction.Down);
                nexts.Add(newP);
            }
            
            check = board.Get(p.currentNode.x, p.currentNode.y - 1);
            if (check != null)
            {
                NodePath newP = new NodePath(check, new System.Collections.Generic.List<Direction>(p.path));
                newP.path.Add(Direction.Up);
                nexts.Add(newP);
            }
            
            check = board.Get(p.currentNode.x+1, p.currentNode.y);
            if (check != null)
            {
                NodePath newP = new NodePath(check, new System.Collections.Generic.List<Direction>(p.path));
                newP.path.Add(Direction.Right);
                nexts.Add(newP);
            }
            
            check = board.Get(p.currentNode.x-1, p.currentNode.y);
            if (check != null)
            {
                NodePath newP = new NodePath(check, new System.Collections.Generic.List<Direction>(p.path));
                newP.path.Add(Direction.Left);
                nexts.Add(newP);
            }

            return nexts;
        }
    }