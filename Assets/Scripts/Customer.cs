using System.Collections;
using UnityEngine;

public enum CurrentAction
{
    WaitingToSeat,
    Seating,
    Ordering,
    WaitingForOrder,
    Leaving
}


public class Customer : MonoBehaviour
{
    private MovementController controller;
    private TileMapper tileMapper;
    private GameStateManager gsm;
    private SpriteRenderer spriteRenderer;
    private Tile currentTile;
    private CurrentAction currentAction;
    private Pasta deliveryPasta;
    private int currentPatience;
    public RandomPasta order;
    private Tile tableTile;
    private Tile targetTile;
    private Coroutine movementCoroutine;
    private Coroutine waitingCoroutine;
    private Coroutine masterCoroutine;

    public bool DoesDeliveryEqualOrder()
    {
        return deliveryPasta.noodle == order.noodle && deliveryPasta.sauce == order.sauce && deliveryPasta.topping == order.topping;
    }

    public void DeliverOrder()
    {
        // Increment score by 1
        gsm.AddScore(1);
        // Clear the pasta plate
        deliveryPasta.ClearPastaPlate();
        // Set the current action to leaving
        currentAction = CurrentAction.Leaving;
        // Set the target tile to the tile at 2, 0
        targetTile = tileMapper.GetTile(2, 0);
        // Unreserve the table
        tableTile.ClearTable();

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Find the gsm on GameWorld
        gsm = GameObject.Find("GameWorld").GetComponent<GameStateManager>();
        // Find the tilemapper on TileWrapper
        tileMapper = GameObject.Find("TileWrapper").GetComponent<TileMapper>();
        // Create a movement controller on this customer
        controller = gameObject.AddComponent<MovementController>();
        // Create a spriterenderer on this object
        if (!GameObject.Find("Pasta").TryGetComponent<Pasta>(out deliveryPasta))
        {
            Debug.LogError("Could not find Pasta component on Pasta GameObject");
        }
        // Set the customer's position to 2, 0
        controller.spawnX = 2;
        controller.spawnY = 0;
        // Set the current tile to the tile at 2, 0
        currentTile = tileMapper.GetTile(2, 0);
        // Pick an open table.
        tableTile = tileMapper.GetRandomOpenTable();
        // Reserve that table
        tableTile.CustomerReserveTable(this);
        // Set the target tile to the tile above the table.
        
        // Set the current action to seating
        order = (RandomPasta)ScriptableObject.CreateInstance("RandomPasta");
        Debug.Log("Random Pasta is being generated");
        switch (Random.Range(0, 3))
        {
            case 0:
                order.noodle = PastaNoodle.SPAGHETTI;
                break;
            case 1:
                order.noodle = PastaNoodle.MACARONI;
                break;
            case 2:
                order.noodle = PastaNoodle.RAVIOLI;
                break;
        }
        switch (Random.Range(0, 3))
        {
            case 0:
                order.sauce = PastaSauce.TOMATO;
                break;
            case 1:
                order.sauce = PastaSauce.ALFREDO;
                break;
            case 2:
                order.sauce = PastaSauce.PESTO;
                break;
        }
        switch (Random.Range(0, 3))
        {
            case 0:
                order.topping = PastaTopping.BIRD;
                break;
            case 1:
                order.topping = PastaTopping.FISH;
                break;
            case 2:
                order.topping = PastaTopping.RAT;
                break;
        }
        StartCoroutine(CustomerRoutine());
    }

    private IEnumerator CustomerRoutine()
    {
        // This will be called in start, pathfind to the current targetTile
        currentAction = CurrentAction.Seating;
        Tile targetTile = tileMapper.GetTile(tableTile.coordX, tableTile.coordY + 1);
        Debug.Log(targetTile.coordX + " " + targetTile.coordY);
        yield return null;
        controller.MoveCharacterToTile(targetTile.coordX, targetTile.coordY);
        yield return null;
        yield return new WaitUntil(() => !controller.IsMoving());
        currentAction = CurrentAction.WaitingForOrder;
        Debug.Log("Customer is waiting for order");
        // Create a child object called PastaOrder
        GameObject pastaOrder = new GameObject("PastaOrder");
        // Make it a child of this object
        pastaOrder.transform.parent = transform;
        pastaOrder.transform.localPosition = new Vector3(0, 0, 0);
        // Create a PastaPlate, PastaNoodle, PastaSauce, and PastaTopping object
        GameObject pastaPlateObject = new GameObject("PastaOrderPlate");
        GameObject pastaNoodleObject = new GameObject("PastaOrderNoodle");
        GameObject pastaSauceObject = new GameObject("PastaOrderSauce");
        GameObject pastaToppingObject = new GameObject("PastaOrderTopping");
        pastaPlateObject.transform.parent = pastaOrder.transform;
        pastaNoodleObject.transform.parent = pastaOrder.transform;
        pastaSauceObject.transform.parent = pastaOrder.transform;
        pastaToppingObject.transform.parent = pastaOrder.transform;
        // Transform the pastaplateobject up 1 unit
        pastaPlateObject.transform.localPosition = new Vector3(0, 1, 0);
        pastaNoodleObject.transform.localPosition = new Vector3(0, 1.125f, 0);
        pastaSauceObject.transform.localPosition = new Vector3(0.025f, 1.25f, 0);
        pastaToppingObject.transform.localPosition = new Vector3(0, 1.4f, 0);
        // Add a spriterenderer to the pastaplateobject
        SpriteRenderer pastaPlateRenderer = pastaPlateObject.AddComponent<SpriteRenderer>();
        SpriteRenderer pastaNoodleRenderer = pastaNoodleObject.AddComponent<SpriteRenderer>();
        SpriteRenderer pastaSauceRenderer = pastaSauceObject.AddComponent<SpriteRenderer>();
        SpriteRenderer pastaToppingRenderer = pastaToppingObject.AddComponent<SpriteRenderer>();
        // Set the sprite of the pastaplateobject to the plate sprite
        pastaPlateRenderer.sortingOrder = 10;
        pastaNoodleRenderer.sortingOrder = 11;
        pastaSauceRenderer.sortingOrder = 12;
        pastaToppingRenderer.sortingOrder = 13;

        pastaPlateRenderer.sprite = deliveryPasta.pastaSprites.plateSprite;
        switch (order.noodle)
        {
            case PastaNoodle.SPAGHETTI:
                pastaNoodleRenderer.sprite = deliveryPasta.pastaSprites.spaghettiSprite;
                break;
            case PastaNoodle.MACARONI:
                pastaNoodleRenderer.sprite = deliveryPasta.pastaSprites.macaroniSprite;
                break;
            case PastaNoodle.RAVIOLI:
                pastaNoodleRenderer.sprite = deliveryPasta.pastaSprites.ravioliSprite;
                break;
        }
        switch (order.sauce)
        {
            case PastaSauce.TOMATO:
                pastaSauceRenderer.sprite = deliveryPasta.pastaSprites.tomatoSprite;
                break;
            case PastaSauce.ALFREDO:
                pastaSauceRenderer.sprite = deliveryPasta.pastaSprites.alfredoSprite;
                break;
            case PastaSauce.PESTO:
                pastaSauceRenderer.sprite = deliveryPasta.pastaSprites.pestoSprite;
                break;
        }
        switch (order.topping)
        {
            case PastaTopping.FISH:
                pastaToppingRenderer.sprite = deliveryPasta.pastaSprites.fishSprite;
                break;
            case PastaTopping.BIRD:
                pastaToppingRenderer.sprite = deliveryPasta.pastaSprites.birdSprite;
                break;
            case PastaTopping.RAT:
                pastaToppingRenderer.sprite = deliveryPasta.pastaSprites.ratSprite;
                break;
        }
        // Wait until currentAction = currentAction.Leaving
        yield return new WaitUntil(() => currentAction == CurrentAction.Leaving);
        gsm.PlayMeow();
        // Travel to 2, 0
        controller.MoveCharacterToTile(2, 0);
        yield return null;
        yield return new WaitUntil(() => !controller.IsMoving());
        // Destroy this customer
        Destroy(gameObject);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
