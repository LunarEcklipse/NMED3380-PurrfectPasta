public class Node
{
    public int x;
    public int y;
    public bool isWalkable;
    public Node parent;
    public float gCost; // Cost from start to this node
    public float hCost; // Heuristic cost to the end node
    public float FCost { get { return gCost + hCost; } }

    public Node(int x, int y, bool isWalkable)
    {
        this.x = x;
        this.y = y;
        this.isWalkable = isWalkable;
    }
}