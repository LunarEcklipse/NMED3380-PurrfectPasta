using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovementController : MonoBehaviour
{
    public int spawnX = 0;
    public int spawnY = 0;
    public float speedMultiplier = 1.0f; // Add this line
    private Tile currentTile;
    private Tile targetTile;
    private bool isMoving = false;
    private TileMapper tileMapper;
    private Coroutine movementCoroutine;

    public bool IsMoving()
    {
        return isMoving;
    }

    void Start()
    {
        transform.position = new Vector3(spawnX, spawnY, 0);
        if (!GameObject.Find("TileWrapper").TryGetComponent(out tileMapper))
        {
            Debug.LogError("Could not get TileMapper component from TileWrapper.");
        }
        currentTile = tileMapper.GetTile(spawnX, spawnY);
    }

    IEnumerator MoveAlongPath(Tile pathCurrentTile, Tile pathTargetTile, List<Node> path)
    {
        if (pathCurrentTile.coordX == pathTargetTile.coordX && pathCurrentTile.coordY == pathTargetTile.coordY) // If both tiles are the same we don't travel.
        {
            yield break;
        }
        isMoving = true;
        // Set the current tile as unoccupied
        if (pathCurrentTile != null)
        {
            pathCurrentTile.SetOccupied(false);
        }
        if (pathTargetTile == null)
        {
            yield break;
        }
        // Set the target tile as reserved
        pathTargetTile.SetReserved(true);

        foreach (Node node in path)
        {
            Tile thisTargetTile = tileMapper.GetTile(node.x, node.y);

            Vector3 targetPosition = new Vector3(node.x, node.y, 0);
            while (transform.position != targetPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 5 * speedMultiplier); // Use speedMultiplier here
                yield return null;
            }
        }
        currentTile.isOccupied = false;
        currentTile.isReserved = false;
        targetTile.isOccupied = true;
        currentTile = targetTile;
        targetTile = null;
        isMoving = false;
    }


    public void MoveCharacterToTile(int x, int y)
    {
        MoveCharacterToTile(x, y, null);
    }

    public void MoveCharacterToTile(int x, int y, Tile currentTargetTile)
    {
        if (tileMapper == null)
        {
            // Get the component from TileWrapper
            if (!GameObject.Find("TileWrapper").TryGetComponent(out tileMapper))
            {
                Debug.LogError("Could not get TileMapper component from TileWrapper.");
                return;
            }
            return;
        }
        targetTile = tileMapper.GetTile(x, y);
        if (targetTile == null)
        {
            Debug.Log("Target tile was null");
            return;
        }
        if (!targetTile.CanMoveToTile())
        {
            if (targetTile.isTable)
            {
                Debug.Log("Moving to right of table instead of table.");
                targetTile = tileMapper.GetTile(x + 1, y);
                if (targetTile == null)
                {
                    return;
                }
                if (!targetTile.CanMoveToTile())
                {
                    Debug.Log("Tile to the right of the table is occupied.");
                    return;
                }
            }
            else if (targetTile.isIngredientTile)
            {
                Debug.Log("Moving to left of ingredient tile instead of ingredient tile.");
                targetTile = tileMapper.GetTile(x - 1, y);
                if (targetTile == null)
                {
                    Debug.Log("Target tile was null");
                    return;
                }
                if (!targetTile.CanMoveToTile())
                {
                    Debug.Log("Tile to the left of the ingredient tile is occupied.");
                    return;
                }
            }
            else
            {
                Debug.Log("Target tile is occupied.");
                return;
            }
        }
        List<Node> path = tileMapper.FindPath(currentTile.coordX, currentTile.coordY, x, y);
        if (path == null) // If there is no path, don't move.
        {
            Debug.Log("No path found.");
            return;
        }
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
            // Figure out what tile the cat is currently on
            if (currentTargetTile != null)
            {
                currentTargetTile.SetReserved(false);
            }
            currentTile = tileMapper.GetTile((int)transform.position.x, (int)transform.position.y);
        }
        movementCoroutine = StartCoroutine(MoveAlongPath(currentTile, targetTile, path));
    }
}
