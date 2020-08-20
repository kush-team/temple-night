using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameController : MonoBehaviour
{
	public GameObject netWorkPlayer;
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
    public GameObject Player;

    public string GameId;
    public string GameOwner;

    private bool isGameStarted = false;



    // Start is called before the first frame update
    void Start()
    {
        netWorkPlayer =  GameObject.Find("NetWorkPlayer");
        netWorkPlayer.SetActive(false);
        Player.GetComponent<PlayerController>().enabled = false;
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
        Player.GetComponent<NetworkEntity> ().name = NickName.text;
        Network.Join(Room.text, NickName.text);   
        LobbyUi.SetActive(true);
    }

    void Update() 
    {   
        PlayerList.text = spawner.GetPlayerNames();

        if (GameOwner == Player.GetComponent<NetworkEntity> ().id && !isGameStarted && GameOwner != "") 
        {
            StartButton.interactable = true;
        }
        if (spawner.GetBossId() != "")
        {
            if (spawner.GetBossId() == Player.GetComponent<NetworkEntity> ().id) 
            {
                var scaleChange = new Vector3(1.2f, 1.2f, 1.2f);
                Player.transform.localScale = scaleChange;
            }
        }
    }

    public void StartGame() 
    {
        isGameStarted = true;
        LobbyUi.SetActive(false);
        Player.GetComponent<PlayerController>().enabled = true;
        Network.StartGame(GameId);   
    }

    public void StartGameFromServer() 
    {
        isGameStarted = true;
        LobbyUi.SetActive(false);
        Player.GetComponent<PlayerController>().enabled = true;
    }


    public void FinishGame(string winner) 
    {
        FinishUi.SetActive(true);
    }

    public void ShowError(string description) 
    {
        LoginUi.SetActive(false);
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
