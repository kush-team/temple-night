using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameController : MonoBehaviour
{
    public InputField Room;
    public InputField NickName;
    public Text ErrorLabel;
    public Text PlayerList;
    
    public Spawner spawner;
    public Button JoinButton;
    public Button StartButton;
    public Button CloseErrorButton;
    
    public GameObject LoginUi;
    public GameObject LobbyUi;
    public GameObject FinishUi;
    public GameObject ErrorUi;

    public string GameId;
    public string GameOwner;

    private bool isGameStarted = false;



    // Start is called before the first frame update
    void Start()
    {
        FinishUi.SetActive(false);
        LobbyUi.SetActive(false);
        ErrorUi.SetActive(false);
        JoinButton.onClick.AddListener(JoinGame);
        StartButton.onClick.AddListener(StartGame);
        CloseErrorButton.onClick.AddListener(CloseError);
        StartButton.interactable = false;
    }

    void JoinGame() 
    {
        Network.Join(Room.text, NickName.text);   
        spawner.SetLocalPlayerName(NickName.text);
        LobbyUi.SetActive(true);
    }

    void Update() 
    {   
        PlayerList.text = spawner.GetPlayerNames();

        if (GameOwner == spawner.GetLocalPlayerId() && !isGameStarted && GameOwner != "") 
        {
            StartButton.interactable = true;
            StartButton.GetComponentInChildren<Text>().text = "Start Game";
        }
    }

    public void StartGame() 
    {
        isGameStarted = true;
        LobbyUi.SetActive(false);
        Network.StartGame(GameId);   
    }

    public void StartGameFromServer(string bossId) 
    {
        spawner.SpawnPlayers(bossId);
        isGameStarted = true;
        LobbyUi.SetActive(false);
    }


    public void FinishGame(string winner) 
    {
        FinishUi.SetActive(true);
    }

    public void ShowError(string description) 
    {
        LoginUi.SetActive(false);
        LobbyUi.SetActive(false);
        ErrorUi.SetActive(true);
        ErrorLabel.text = description;
    }    

    public void CloseError() 
    {
        LoginUi.SetActive(true);
        ErrorUi.SetActive(false);
    }

    public void Restart() 
    {
        //Player.SetActive(false);
        //FinishUi.SetActive(false);
        //LobbyUi.SetActive(false);
    }
}
