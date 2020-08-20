using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameController : MonoBehaviour
{
	public GameObject netWorkPlayer;
    public InputField Room;
    public InputField NickName;
    
    public Spawner spawner;
    public Button JoinButton;
    public Button StartButton;
    
    public GameObject LoginUi;
    public GameObject OwnerUi;
    public GameObject FinishUi;
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
        OwnerUi.SetActive(false);
        JoinButton.onClick.AddListener(JoinGame);
        StartButton.onClick.AddListener(StartGame);
    }

    void JoinGame() 
    {
        Network.Join(Room.text, NickName.text);   
        LoginUi.SetActive(false);
    }

    void Update() 
    {   
        if (GameOwner == Player.GetComponent<NetworkEntity> ().id && !isGameStarted && GameOwner != "") 
        {
            OwnerUi.SetActive(true);
        }
    }

    public void StartGame() 
    {
        isGameStarted = true;
        OwnerUi.SetActive(false);
        Player.GetComponent<PlayerController>().enabled = true;
        Network.StartGame(GameId);   
    }

    public void StartGameFromServer() 
    {
        isGameStarted = true;
        OwnerUi.SetActive(false);
        Player.GetComponent<PlayerController>().enabled = true;
    }


    public void FinishGame(string winner) 
    {
        FinishUi.SetActive(true);
    }

    public void Restart() 
    {
        //Player.SetActive(false);
        //FinishUi.SetActive(false);
        //OwnerUi.SetActive(false);
    }
}
