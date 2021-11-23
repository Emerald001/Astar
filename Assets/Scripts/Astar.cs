using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Astar
{
    /// <summary>
    /// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path
    /// Note that you will probably need to add some helper functions
    /// from the startPos to the endPos
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="grid"></param>
    /// <returns></returns>
    /// 

    public bool EndFound = false;


    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();

        Node currentNode = new Node(startPos, null, 0, 0);
        openList.Add(currentNode);

        while (openList.Count != 0)
        {
            if (currentNode.position == endPos)
            {
                var path = new List<Vector2Int>();

                while (currentNode.position != startPos)
                {
                    path.Add(currentNode.position);
                    currentNode = currentNode.parent;
                }
                path.Reverse();

                return path;
            }

            var neighbours = new List<Node>();
            var Walls = LookForWalls(grid[currentNode.position.x, currentNode.position.y]);

            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    int neighbourX = currentNode.position.x + x;
                    int neighbourY = currentNode.position.y + y;

                    if (neighbourX < 0 || neighbourX >= grid.GetLength(0) || neighbourY < 0 || neighbourY >= grid.GetLength(1) || Mathf.Abs(x) == Mathf.Abs(y))
                        continue;

                    bool Break = false;
                    for (int i = 0; i < Walls.Count; i++)
                    {
                        if (new Vector2Int(x, y) == Walls[i])
                        {
                            Debug.Log("Found wall");
                            Break = true;
                        }
                    }

                    for (int i = 0; i < closedList.Count; i++)
                    {
                        if (closedList[i].position == new Vector2Int(neighbourX, neighbourY))
                        {
                            Debug.Log("in closed list");
                            Break = true;
                        }
                    }
                    if (Break)
                        continue;

                    var tmpNode = new Node(new Vector2Int(neighbourX, neighbourY), null, 0, 0);

                    DefineNodes(startPos, endPos, tmpNode);
                    neighbours.Add(tmpNode);
                }
            }

            //Node lowestScoringNode = neighbours.OrderBy(node => node.FScore).First();
            Node lowestScoringNode = null;

            if (neighbours.Count == 0)
                Debug.Log("list Empty");

            for (int i = 0; i < neighbours.Count; i++)
            {
                if (lowestScoringNode == null)
                    lowestScoringNode = neighbours[i];
                if (neighbours[i].FScore < lowestScoringNode.FScore)
                    lowestScoringNode = neighbours[i];
            }

            if (lowestScoringNode == null)
                Debug.Log("No Neighbours");

            lowestScoringNode.parent = currentNode;
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            currentNode = lowestScoringNode;

            openList.Add(currentNode);
        }
        return null;
    }

    public bool CheckNeighbours()
    {
        return true;
    }

    public List<Vector2Int> LookForWalls(Cell cell)
    {
        var wallList = new List<Vector2Int>();

        if ((cell.walls & Wall.DOWN) != 0) { wallList.Add(Vector2Int.down); }
        if ((cell.walls & Wall.UP) != 0) { wallList.Add(Vector2Int.up); }
        if ((cell.walls & Wall.LEFT) != 0) { wallList.Add(Vector2Int.left); }
        if ((cell.walls & Wall.RIGHT) != 0) { wallList.Add(Vector2Int.right); }

        return wallList;
    }

    public void DefineNodes(Vector2Int startPos, Vector2Int endPos, Node node)
    {
        if(node.parent != null)
            node.GScore = node.parent.GScore + Vector2Int.Distance(node.position, node.parent.position);
        node.HScore = Vector2Int.Distance(node.position, endPos);
    }

    /// <summary>
    /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
    /// </summary>
    public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node

        public float FScore { //GScore + HScore
            get { return GScore + HScore; }
        }
        public float GScore; //Current Travelled Distance + distance to new node
        public float HScore; //Distance estimated based on Heuristic

        public Node() { }
        public Node(Vector2Int position, Node parent, int GScore, int HScore)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
        }
    }
}
