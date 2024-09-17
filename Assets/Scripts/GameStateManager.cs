using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public enum GameState
{
    StartMenu,
    Playing,
    GameOver,
    None
}

public class GameStateManager : MonoBehaviour
{
    public GameState gameState = GameState.StartMenu;
    private TileMapper tileMapper;
    public float gameSpeed = 1.0f;
    public Tile clickedTile;
    public int waitingCustomers = 0;
    public int score = 0;
    private Coroutine customerSpawnCoroutine;
    public GameObject customerPrefab;
    public CustomerSprites customerSprites;
    public TextMeshProUGUI scoreText;

    private StartGameButton startGameButton;
    private GameObject startGameImage;
    private GameObject gameOverImage;
    [SerializeField] private List<AudioClip> meows = new();
    private AudioSource audioPlayer;

    public void PlayMeow()
    {
        // Check if there's already audio playing and stop it
        if (audioPlayer.isPlaying)
        {
            audioPlayer.Stop();
        }
        audioPlayer.PlayOneShot(meows[Random.Range(0, meows.Count)]);
    }

    public int GetScore()
    {
        return score;
    }
    public void AddScore(int points)
    {
        score += points;
    }
    public bool IsGameRunning()
    {
        return gameState == GameState.Playing;
    }
    void Start()
    {
        // Get the tilemapper from tilewrapper
        tileMapper = GameObject.Find("TileWrapper").GetComponent<TileMapper>();
        // Get the startgamebutton from the scene
        startGameButton = GameObject.Find("StartGameButton").GetComponent<StartGameButton>();
        startGameImage = GameObject.Find("TitleImage");
        gameOverImage = GameObject.Find("GameOver");
        scoreText = GameObject.Find("Score").GetComponent<TextMeshProUGUI>();
        // Add an AudioSource component to this gameobject
        audioPlayer = gameObject.AddComponent<AudioSource>();
        // Set audioPlayer volume to 0 because I don't have time to rip it out but the SFX are obnoxious
        audioPlayer.volume = 0;
        gameOverImage.SetActive(false);
    }

    private void Update()
    {
        if (IsGameRunning())
        {
            UpdateScore();
        }
    }

    public void StartGame()
    {
        score = 0;
        if (IsGameRunning()) { return; }
        gameState = GameState.Playing;
        customerSpawnCoroutine = StartCoroutine(SpawnCustomers());
        startGameButton.gameObject.SetActive(false);
        startGameImage.SetActive(false);
        gameOverImage.SetActive(false);
        PlayMeow();
    }

    public void EndGame()
    {
        if (!IsGameRunning()) { return; }
        gameState = GameState.GameOver;
        StopCoroutine(customerSpawnCoroutine);
        startGameButton.gameObject.SetActive(true);
        gameOverImage.SetActive(true);
        PlayMeow();
    }

    private void UpdateScore()
    {
        scoreText.text = "Score\n" + score.ToString();
    }

    private IEnumerator SpawnCustomers()
    {
        Debug.Log("Starting customer spawn coroutine");
        // Destroy all existing customers
        foreach (GameObject customer in GameObject.FindGameObjectsWithTag("Customer"))
        {
            Destroy(customer);
        }
        waitingCustomers = 0; 
        float timeUntilNextCustomer = 0;
        // Spawn a customer at the door every 10 seconds. If there is more than 10 waiting customers the game ends.
        while (IsGameRunning())
        {
            if (timeUntilNextCustomer <= 0)
            {
                timeUntilNextCustomer = 5.0f;
                waitingCustomers += 1;
                if (waitingCustomers > 5)
                {
                    EndGame();
                }
            }
            // Check if there is a table available. If so, spawn a customer
            if (tileMapper.IsTableAvailable() && waitingCustomers > 0)
            {
                waitingCustomers -= 1;
                GameObject customer = new("Customer");
                customer.tag = "Customer";
                // Set the customer's position to 2, 0
                customer.transform.position = new Vector3(2, 0, 0);
                SpriteRenderer customerRenderer = customer.AddComponent<SpriteRenderer>();
                customerRenderer.sprite = customerSprites.getRandomSprite();
                customer.AddComponent<Customer>(); // Give them the customer component
                
            }
            timeUntilNextCustomer -= Time.deltaTime;
            yield return null;
        }
    }
}
