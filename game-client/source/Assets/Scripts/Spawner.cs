using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Cinemachine;

public class Spawner : MonoBehaviour {

    public GameObject Boss;
    public GameObject Player;
    public GameObject PickeableWeed;
    public GameObject PickeableGrinder;
    public GameObject PickeablePaper;
    public Camera VCam;

    private Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();

    private Dictionary<string, GameObject> pickeables = new Dictionary<string, GameObject>();

    private string playerId;

    private GameObject playerObject;
    private string playerName;

    private Dictionary<string, string> netPlayers = new Dictionary<string, string>();


    public string GetLocalPlayerName()
    {
        return playerName;
    }

    public void SetLocalPlayerName(string name)
    {
        playerName = name;
    }


    public GameObject GetLocalPlayer()
    {
        return playerObject;
    }

    public string GetLocalPlayerId()
    {
        return playerId;
    }

    public void CreateLocalPlayer(string id)
    {
        playerId = id;
    }




    public void AddNetPlayer(string id, string nickname, float healPoints)
    {
        netPlayers.Add(id, nickname);
    }


    public void SpawnPlayers(string bossId)
    {
        var prefab = Player;
        var spawnSlot = "spot_player1";

        if (bossId == playerId)
        {
            prefab = Boss;
            spawnSlot = "spot_boss";
        }

        if (GameObject.Find("Me") == null) 
        {
            playerObject = Instantiate(prefab, GameObject.Find(spawnSlot).transform.position, Quaternion.identity) as GameObject;
            playerObject.AddComponent<PlayerController>();
            playerObject.name = "Me";
            playerObject.GetComponent<NetworkEntity>().id = playerId;
            playerObject.GetComponent<PlayerController>().VCam = VCam;

            if (bossId == playerId) 
            {
                playerObject.GetComponent<NetworkEntity>().role = "boss";
            }  
                      
            AddPlayer(playerId, playerObject);


            var c = GameObject.FindWithTag("VirtualCamera");
            c.GetComponent<CinemachineFreeLook>().m_Follow = playerObject.transform;
            c.GetComponent<CinemachineFreeLook>().m_LookAt = playerObject.transform;
            var pIndex = 2;

            foreach(KeyValuePair<string, string> entry in netPlayers)
            {   
                if (bossId == entry.Key)
                {
                    prefab = Boss;
                    spawnSlot = "spot_boss";
                } 
                else
                {
                    prefab = Player;
                    spawnSlot = "spot_player" + pIndex;  
                    pIndex++;
                }

                var netPlayer = Instantiate(prefab, GameObject.Find(spawnSlot).transform.position, Quaternion.identity) as GameObject;
                netPlayer.AddComponent<NetPlayer>();
                netPlayer.GetComponent<NetworkEntity>().id = entry.Key;
                netPlayer.GetComponent<NetworkEntity>().nickName = entry.Value;
                if (bossId == entry.Key) 
                {
                    netPlayer.GetComponent<NetworkEntity>().role = "boss";
                }
                AddPlayer(entry.Key, netPlayer);
            }          
        }

    }


    public void SpawnPickeable(string id, string type, float spot, bool picked)
    {
        int spotIndex = (int)spot;
        Vector3 loadPosition =  GameObject.Find("spot" + spotIndex).transform.position;

        var pickeableType = PickeableWeed;

        switch (type) {
            case "grinder":
                pickeableType = PickeableGrinder;
                break;
            case "roll-paper":
                pickeableType = PickeablePaper;
                break;

            default:
                pickeableType = PickeableWeed;
                break;
        }

        var pickeable = GameObject.Instantiate(pickeableType, loadPosition, Quaternion.identity) as GameObject;
        pickeable.GetComponent<NetWorkPickeable>().id = id;
        AddPickeable(id, pickeable);
    }   

    public void AddPickeable(string id, GameObject pickeable)
    {
        pickeables.Add(id, pickeable);
    }

    public GameObject GetPickeable(string id)
    {
        return pickeables[id];
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

    public void RemovePickeable(string id)
    {
        var pickeable = pickeables[id];
        Destroy(pickeable);
        pickeables.Remove(id);
    }

    public string GetPlayerNames()
    {
        var str = "Player List:\n" + playerName;
        foreach(KeyValuePair<string, string> entry in netPlayers)
        {
             str += "\n" + entry.Value;
        }       
        return str;
    }
}
