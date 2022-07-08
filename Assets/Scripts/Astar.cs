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
        List<Vector2Int> closedList = new List<Vector2Int>();

        Node currentNode = new Node(startPos, null, 0, 0);
        Node endNode = new Node(endPos, null, 0, 0);
        openList.Add(currentNode);

        int nodes = 0;

        while (true)
        {
            currentNode = openList[0];

            for (int i = 1; i < openList.Count; i++) {
                //if the two nodes have same fcost we check the hcost and then take the one closest to the end of the node
                if (openList[i].FScore < currentNode.FScore || (openList[i].FScore == currentNode.FScore && openList[i].HScore < currentNode.HScore)) {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode.position);

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

            foreach (Node neighbour in GetNeighbours(currentNode, grid)) {
                if (closedList.Contains(neighbour.position)) {
                    continue;
                }
                //so remeber gCost is shortest path from the start node and is what we use to check if shorter
                float newCostToNeighbour = currentNode.GScore + Vector2Int.Distance(currentNode.position, neighbour.position);

                if (newCostToNeighbour < neighbour.GScore || !openList.Contains(neighbour)) {
                    neighbour.GScore = newCostToNeighbour;
                    neighbour.HScore = Vector2Int.Distance(currentNode.position, endNode.position);
                    neighbour.parent = currentNode;
                    if (!openList.Contains(neighbour)) {
                        openList.Add(neighbour);
                    }
                }
            }

            nodes++;
        }
        return null;
    }

    public List<Node> GetNeighbours(Node currentNode, Cell[,] grid)
    {
        var neighbours = new List<Node>();
        var Walls = LookForWalls(grid[currentNode.position.x, currentNode.position.y]);

        for (int x = -1; x < 2; x++) 
        for (int y = -1; y < 2; y++) {
            int neighbourX = currentNode.position.x + x;
            int neighbourY = currentNode.position.y + y;

            if (neighbourX < 0 || neighbourX >= grid.GetLength(0) || neighbourY < 0 || neighbourY >= grid.GetLength(1) || Mathf.Abs(x) == Mathf.Abs(y))
                continue;

            if(Walls.Contains(new Vector2Int(x, y))) {
                continue;
            }

            var tmpNode = new Node(new Vector2Int(neighbourX, neighbourY), null, 0, 0);
            neighbours.Add(tmpNode);
        }

        return neighbours;
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

    int Distance(Node node, Node goalNode) {
        return (int)Mathf.Abs(node.position.y - goalNode.position.y) + Mathf.Abs(node.position.x - goalNode.position.x);
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
