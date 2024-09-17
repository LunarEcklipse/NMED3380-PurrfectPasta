using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartGameButton : MonoBehaviour
{
    private Button button;
    private GameStateManager gsm;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the button on this component
        button = GetComponent<Button>();
        // Set the button text to start
        button.GetComponentInChildren<TextMeshProUGUI>().text = "Start Game";
        // Get the GameStateManager from the scene
        gsm = GameObject.Find("GameWorld").GetComponent<GameStateManager>();
        button.onClick.AddListener(StartGame);
    }

    // button onclick
    public void StartGame()
    {
        gsm.StartGame();
    }
    // Update is called once per frame
    void Update()
    {if (gsm.IsGameRunning())
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = true;
        }
        
    }
}
