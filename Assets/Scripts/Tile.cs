using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int coordX;
    public int coordY;
    public bool isWalkable;
    public bool isOccupied;
    public bool isReserved;
    public bool isTable = false;
    public bool isIngredientTile = false;
    public PastaNoodle giveNoodle = PastaNoodle.NONE;
    public PastaSauce giveSauce = PastaSauce.NONE;
    public PastaTopping giveTopping = PastaTopping.NONE;
    [SerializeField] private List<Sprite> tiles = new();
    [SerializeField] private Sprite tableTile;
    [SerializeField] private Sprite counterTile;
    private new Collider2D collider;
    private SpriteRenderer floorSpriteRenderer;
    private SpriteRenderer overlaySpriteRenderer;
    private GameStateManager gsm;
    private Customer tableCustomer;
    public PastaSprites pastaSprites;
    public void SetTileIndex(int index)
    {
        floorSpriteRenderer.sprite = tiles[index];
    }
    public bool CanMoveToTile()
    {
        return isWalkable;
    }
    public void SetOccupied(bool occupied)
    {
        isOccupied = occupied;
    }

    public void SetReserved(bool reserved)
    {
        isReserved = reserved;
    }

    public bool IsIngedientTile()
    {
        return isIngredientTile;
    }

    public void GiveIngredientToPlayer(Pasta inPasta)
    {
        if (!isIngredientTile)
        {
            Debug.LogError("Tile tried to deliver an ingredient that doesn't exist");
            return;
        }
        if (giveNoodle != PastaNoodle.NONE)
        {
            inPasta.SetNoodles(giveNoodle);
        }
        if (giveSauce != PastaSauce.NONE)
        {
            inPasta.SetSauce(giveSauce);
        }
        if (giveTopping != PastaTopping.NONE)
        {
            inPasta.SetTopping(giveTopping);
        }
    }

    public void CustomerReserveTable(Customer customer)
    {
        if (!isTable)
        {
            Debug.LogError("Customer tried to reserve a non-table tile");
            return;
        }
        if (isReserved)
        {
            Debug.LogError("Customer tried to reserve a reserved table");
            return;
        }
        isReserved = true;
        tableCustomer = customer;
    }

    public void SetTableCustomer(Customer customer)
    {
        tableCustomer = customer;
    }

    public Customer GetTableCustomer()
    {
        return tableCustomer;
    }
    public bool TableHasCustomer()
    {
        return tableCustomer != null;
    }
    public void ClearTable()
    {
        tableCustomer = null;
        isReserved = false;
    }
    public void SetTable(bool table)
    {
        isTable = table;
        if (isTable)
        {
            isWalkable = false;
            overlaySpriteRenderer.enabled = true;
            overlaySpriteRenderer.sprite = tableTile;
        }
        else
        {
            isWalkable = true;
            overlaySpriteRenderer.enabled = false;
        }
    }

    public void SetCounter(bool isCounter, PastaNoodle pastaNoodle, PastaSauce pastaSauce, PastaTopping pastaTopping)
    {
        isWalkable = !isCounter;
        isOccupied = false;
        isTable = false;
        isIngredientTile = true;
        giveNoodle = pastaNoodle;
        giveSauce = pastaSauce;
        giveTopping = pastaTopping;
        floorSpriteRenderer.sprite = counterTile;
        overlaySpriteRenderer.enabled = true;
        switch (giveNoodle)
        {
            case PastaNoodle.SPAGHETTI:
                overlaySpriteRenderer.sprite = pastaSprites.spaghettiSprite;
                return;
            case PastaNoodle.MACARONI:
                overlaySpriteRenderer.sprite = pastaSprites.macaroniSprite;
                return;
            case PastaNoodle.RAVIOLI:
                overlaySpriteRenderer.sprite = pastaSprites.ravioliSprite;
                return;
            case PastaNoodle.NONE:
                break;
        }
        switch (giveSauce)
        {
            case PastaSauce.TOMATO:
                overlaySpriteRenderer.sprite = pastaSprites.tomatoSprite;
                return;
            case PastaSauce.ALFREDO:
                overlaySpriteRenderer.sprite = pastaSprites.alfredoSprite;
                return;
            case PastaSauce.PESTO:
                overlaySpriteRenderer.sprite = pastaSprites.pestoSprite;
                return;
            case PastaSauce.NONE:
                break;
        }
        switch (giveTopping)
        {
            case PastaTopping.FISH:
                overlaySpriteRenderer.sprite = pastaSprites.fishSprite;
                return;
            case PastaTopping.BIRD:
                overlaySpriteRenderer.sprite = pastaSprites.birdSprite;
                return;
            case PastaTopping.RAT:
                overlaySpriteRenderer.sprite = pastaSprites.ratSprite;
                return;
            case PastaTopping.NONE:
                break;
        }
        Debug.LogError("Counter was not set to any ingredient");
        return;
    }

    private void Awake()
    {
        // Create a new spriterenderer on the gameobject
        floorSpriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        // Set the layer to the bottom most layer
        floorSpriteRenderer.sortingOrder = -1;
        // Create a new gameobject that is a child of this object
        GameObject overlayObject = new("Overlay");
        // Set the parent of the overlay object to this object
        overlayObject.transform.parent = transform;
        // Set the position of the overlay object to the position of this object
        overlayObject.transform.position = transform.position;
        // Create a new spriterenderer on the overlay object
        overlaySpriteRenderer = overlayObject.AddComponent<SpriteRenderer>();
        overlaySpriteRenderer.sortingOrder = 1;

        
        // Set the sprite to the first sprite in the list
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get this tile's collider
        collider = GetComponent<Collider2D>();
        // Get the gamestate manager
        gsm = GameObject.Find("GameWorld").GetComponent<GameStateManager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Determine if this tile was clicked on
    private void OnMouseDown()
    {
        if (!gsm.IsGameRunning())
        {
            return;
        }
        gsm.clickedTile = this;
        // If the tile is a table, log that it is a table
        if (isTable)
        {
            Debug.Log("This tile is a table");
        }
        // Otherwise log that it is not a table
        else
        {
            Debug.Log("This tile is not a table");
        }
    }
}
