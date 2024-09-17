using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Tile clickedTile;
    private GameStateManager gsm;
    private MovementController movementController;
    private TileMapper tileMapper;
    private GameState lastState;
    private Coroutine queuedInteractionCoroutine;
    private Customer tableCustomer;
    private Pasta pasta;
    void Start()
    {
        if (!GameObject.Find("GameWorld").TryGetComponent<GameStateManager>(out gsm))
        {
            Debug.LogError("GameStateManager not found");
        }
        if (!GameObject.Find("TileWrapper").TryGetComponent<TileMapper>(out tileMapper))
        {
            Debug.LogError("TileMapper not found");
        }
        if (!GameObject.Find("Pasta").TryGetComponent<Pasta>(out pasta))
        {
            Debug.LogError("Pasta not found");
        }
        // Get the MovementController component
        movementController = GetComponent<MovementController>();
        lastState = GameState.None;
    }

    // Update is called once per frame
    void Update()
    {
        switch(gsm.gameState)
        {
            case GameState.Playing:
                if (lastState != GameState.Playing)
                {
                    Debug.Log("Game just started!");
                    lastState = GameState.Playing;
                    // Teleport this gameobject to 0, 0, 0
                    transform.position = new Vector3(0, 0, 0);
                }
                if (gsm.clickedTile != null)
                {
                    // Get the clicked tile
                    Tile lastClickedTile = clickedTile;
                    clickedTile = gsm.clickedTile;
                    gsm.clickedTile = null;
                    // Check if the clicked tile can be walked to.
                    if (clickedTile.CanMoveToTile() || (!clickedTile.CanMoveToTile() && clickedTile.isTable) || !(clickedTile.CanMoveToTile() && clickedTile.isIngredientTile))
                    {
                        // Get the customer at the table if there is one
                        if (clickedTile.isTable)
                        {
                            tableCustomer = clickedTile.GetTableCustomer();
                        }
                        if (queuedInteractionCoroutine != null)
                        {
                            StopCoroutine(queuedInteractionCoroutine);
                        }
                        if (clickedTile.isTable)
                        {
                            queuedInteractionCoroutine = StartCoroutine(GiveCustomerOrderAtTable());
                        }
                        if (clickedTile.isIngredientTile)
                        {
                            queuedInteractionCoroutine = StartCoroutine(CollectIngredient());
                        }
                        movementController.MoveCharacterToTile(clickedTile.coordX, clickedTile.coordY, lastClickedTile);
                    }
                    else
                    {
                        clickedTile = lastClickedTile;
                    }
                }
                break;
            case GameState.StartMenu:
                if (lastState != GameState.StartMenu)
                {
                    Debug.Log("Game is in the start menu!");
                    lastState = GameState.StartMenu;
                    transform.position = new Vector3(-100, -100, 0);
                    tableCustomer = null;
                    clickedTile = null;
                }
                break;
            case GameState.GameOver:
                if (lastState != GameState.GameOver)
                {
                    Debug.Log("Game is over!");
                    lastState = GameState.GameOver;
                    // Stop all movements of the player
                    movementController.StopAllCoroutines();
                    this.StopAllCoroutines();
                }
                break;
        }
    }

    private IEnumerator GiveCustomerOrderAtTable()
    {
        yield return null;
        // Wait until the player is not moving anymore.
        while (movementController.IsMoving())
        {
            yield return null;
        }
        if (clickedTile.isTable && tableCustomer != null)
        {
            if (tableCustomer.DoesDeliveryEqualOrder())
            {
                tableCustomer.DeliverOrder();
            }
            else
            {
                // Game over
                gsm.EndGame();
            }
        }
    }

    private IEnumerator CollectIngredient()
    {
        yield return null;
        while (movementController.IsMoving())
        {
            yield return null;
        }
        if (clickedTile.isIngredientTile)
        {
            // Iterate through the ingredients of the clicked tile and add them to the dish.
            switch (clickedTile.giveNoodle)
            {
                case PastaNoodle.SPAGHETTI:
                    pasta.SetNoodles(PastaNoodle.SPAGHETTI);
                    break;
                case PastaNoodle.MACARONI:
                    pasta.SetNoodles(PastaNoodle.MACARONI);
                    break;
                case PastaNoodle.RAVIOLI:
                    pasta.SetNoodles(PastaNoodle.RAVIOLI);
                    break;
                case PastaNoodle.NONE:
                    break;
            }
            switch (clickedTile.giveSauce)
            {
                case PastaSauce.TOMATO:
                    pasta.SetSauce(PastaSauce.TOMATO);
                    break;
                case PastaSauce.ALFREDO:
                    pasta.SetSauce(PastaSauce.ALFREDO);
                    break;
                case PastaSauce.PESTO:
                    pasta.SetSauce(PastaSauce.PESTO);
                    break;
                case PastaSauce.NONE:
                    break;
            }
            switch (clickedTile.giveTopping)
            {
                case PastaTopping.RAT:
                    pasta.SetTopping(PastaTopping.RAT);
                    break;
                case PastaTopping.FISH:
                    pasta.SetTopping(PastaTopping.FISH);
                    break;
                case PastaTopping.BIRD:
                    pasta.SetTopping(PastaTopping.BIRD);
                    break;
                case PastaTopping.NONE:
                    break;
            }
        }
    }
}
