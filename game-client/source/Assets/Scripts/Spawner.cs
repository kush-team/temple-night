using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using SocketIO;

public class Spawner : MonoBehaviour {

    public GameObject networkPlayer;
    public GameObject currentPlayer;
    public SocketIOComponent socket;

    private Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();

    public GameObject SpawnPlayer(string id, string nickname, float healPoints)
    {
        var player = Instantiate(networkPlayer, Vector3.zero, Quaternion.identity) as GameObject;

        player.GetComponent<NetworkEntity>().id = id;
        player.GetComponent<NetworkEntity>().name = nickname;
        player.GetComponent<NetworkEntity>().healPoints = healPoints;
        player.SetActive(true);

        AddPlayer(id, player);

        return player;
    }

    public string GetBossId() 
    {
        var id = "";
        foreach(KeyValuePair<string, GameObject> entry in players)
        {
             if (entry.Value.GetComponent<NetworkEntity>().role == "boss") 
             {
                id = entry.Value.GetComponent<NetworkEntity>().id;
             }
        }          
        return id;
    }

    public GameObject GetPlayer(string id)
    {
        return players[id];
    }

    public void AddPlayer(string id, GameObject player)
    {
        players.Add(id, player);
    }

    public void Remove(string id)
    {
        var player = players[id];
        Destroy(player);
        players.Remove(id);
    }


    public string GetPlayerNames()
    {
        var str = "Player List:";
        foreach(KeyValuePair<string, GameObject> entry in players)
        {
             str += "\n" + entry.Value.GetComponent<NetworkEntity>().name;
        }       
        return str;
    }
}
