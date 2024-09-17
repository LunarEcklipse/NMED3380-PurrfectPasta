using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class TileMapper : MonoBehaviour
{
    public int width = 16;
    public int height = 9;
    public GameObject floorTilePrefab;
    // Create a 2D array of tiles that's width x height
    private GameObject[,] tiles;
    private int floorTileIndex; // Random number between 1 and 3
    public int GetFloorTileIndex()
    {
        return floorTileIndex;
    }
    void Awake()
    {
        floorTileIndex = Random.Range(1, 3);
        tiles = new GameObject[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject tile = Instantiate(floorTilePrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
                tile.name = $"Tile_{x}_{y}";
                Tile tileObject = tile.GetComponent<Tile>();
                tileObject.coordX = x;
                tileObject.coordY = y;
                tileObject.isWalkable = true;
                tileObject.isOccupied = false;
                tileObject.SetTileIndex(floorTileIndex);
                tiles[x, y] = tile; // Ensure the tile is stored in the array
            }
        }
        DetermineTablePositions();
        DetermineCounterPositions();
    }

    public void DetermineTablePositions()
    {
        /* The table positions are as follows:
         * 2, 6
         * 6, 6
         * 4, 4
         * 2, 2,
         * 6, 2
        */
        SetTileTable(2, 6, true);
        SetTileTable(6, 6, true);
        SetTileTable(4, 4, true);
        SetTileTable(2, 2, true);
        SetTileTable(6, 2, true);

    }

    public void DetermineCounterPositions()
    {
        // All tiles in the X axis at 11 are countertops.
        for (int y = 0; y < height; y++)
        {
            SetTileCounter(11, y, true);
        }
    }

    public Tile GetTile(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return tiles[x, y].GetComponent<Tile>();
        }
        return null;
    }

    public void SetTileTable(int x, int y, bool isTable)
    {
        Tile tile = GetTile(x, y);
        if (tile != null)
        {
            tile.SetTable(isTable);
        }
    }

    public void SetTileCounter(int x, int y, bool isCounter)
    {
        Tile tile = GetTile(x, y);
        if (tile != null)
        {
            switch(y)
            {
                /* y values:
                 * 0 = Spaghetti
                 * 1 = Macaroni
                 * 2 = Ravioli
                 * 3 = Tomato
                 * 4 = Alfredo
                 * 5 = Pesto
                 * 6 = Rat
                 * 7 = Fish
                 * 8 = Bird
                 * */
                case 0:
                    tile.SetCounter(isCounter, PastaNoodle.SPAGHETTI, PastaSauce.NONE, PastaTopping.NONE);
                    break;
                case 1:
                    tile.SetCounter(isCounter, PastaNoodle.MACARONI, PastaSauce.NONE, PastaTopping.NONE);
                    break;
                case 2:
                    tile.SetCounter(isCounter, PastaNoodle.RAVIOLI, PastaSauce.NONE, PastaTopping.NONE);
                    break;
                case 3:
                    tile.SetCounter(isCounter, PastaNoodle.NONE, PastaSauce.TOMATO, PastaTopping.NONE);
                    break;
                case 4:
                    tile.SetCounter(isCounter, PastaNoodle.NONE, PastaSauce.ALFREDO, PastaTopping.NONE);
                    break;
                case 5:
                    tile.SetCounter(isCounter, PastaNoodle.NONE, PastaSauce.PESTO, PastaTopping.NONE);
                    break;
                case 6:
                    tile.SetCounter(isCounter, PastaNoodle.NONE, PastaSauce.NONE, PastaTopping.RAT);
                    break;
                case 7:
                    tile.SetCounter(isCounter, PastaNoodle.NONE, PastaSauce.NONE, PastaTopping.FISH);
                    break;
                case 8:
                    tile.SetCounter(isCounter, PastaNoodle.NONE, PastaSauce.NONE, PastaTopping.BIRD);
                    break;
                default:
                    tile.SetCounter(isCounter, PastaNoodle.NONE, PastaSauce.NONE, PastaTopping.NONE);
                    Debug.LogError("Counter Initialized with no ingredient");
                    break;
            }
        }
    }

    public List<Node> FindPath(int startX, int startY, int endX, int endY)
    {
        // Get the tile at the end position
        Tile endTile = GetTile(endX, endY);
        if (endTile == null)
        {
            return null;
        }
        else if (!endTile.CanMoveToTile())
        {
            Debug.Log("Target tile is occupied.");
            // Check if the tile is a table
            if (endTile.isTable)
            {
                Debug.Log("Tile is table, new target is to the right of the table.");
                // Set the target as the tile to the right of the table
                endX++;
            }
            else if (endTile.isIngredientTile)
            {
                Debug.Log("Tile is ingredient tile, new target is to left of the tile.");
                endX--;
            }
            else if (this.gameObject.TryGetComponent<Customer>(out _)) // Check if this gameobject has the Customer component
            {
                // Customer is blocked by player, move to the spot anyways.
                Debug.Log("Player is blocking table. Customer moving to bottom of table instead.");
                endY -= 2;
            }
            else
            {
                return null;
            }
        }
        Node[,] nodes = new Node[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                nodes[x, y] = new Node(x, y, tiles[x, y].GetComponent<Tile>().CanMoveToTile());
            }
        }

        Node startNode = nodes[startX, startY];
        Node endNode = nodes[endX, endY];

        List<Node> openList = new();
        HashSet<Node> closedList = new();
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].FCost < currentNode.FCost || openList[i].FCost == currentNode.FCost && openList[i].hCost < currentNode.hCost)
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode == endNode)
            {
                return RetracePath(startNode, endNode);
            }

            foreach (Node neighbor in GetNeighbors(nodes, currentNode))
            {
                if (!neighbor.isWalkable || closedList.Contains(neighbor))
                {
                    continue;
                }

                float newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newCostToNeighbor < neighbor.gCost || !openList.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, endNode);
                    neighbor.parent = currentNode;

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }
    List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }
    List<Node> GetNeighbors(Node[,] nodes, Node node)
    {
        List<Node> neighbors = new();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int checkX = node.x + x;
                int checkY = node.y + y;

                if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height)
                {
                    neighbors.Add(nodes[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }
    float GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.x - nodeB.x);
        int dstY = Mathf.Abs(nodeA.y - nodeB.y);

        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        return 14 * dstX + 10 * (dstY - dstX);
    }
    // Update is called once per frame
    
    public List<Tile> GetOpenTables()
    {
        // iterate through all tiles and find those with open tables
        List<Tile> openTables = new();
        foreach (GameObject tile in tiles)
        {
            Tile tileObject = tile.GetComponent<Tile>();
            if (tileObject.isTable && !tileObject.TableHasCustomer())
            {
                openTables.Add(tileObject);
            }
        }
        return openTables;
    }

    public Tile GetRandomOpenTable()
    {
        List<Tile> openTables = GetOpenTables();
        if (openTables.Count == 0)
        {
            return null;
        }
        return openTables[Random.Range(0, openTables.Count)];
    }

    public bool IsTableAvailable()
    {
        return GetOpenTables().Count > 0;
    }
    void Update()
    {
        
    }
}
