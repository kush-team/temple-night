using System;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class Network : MonoBehaviour
{

    static SocketIOComponent socket;

    public GameObject gameController;
    public Spawner spawner;

    void Start()
    {
        socket = GetComponent<SocketIOComponent>();
        socket.On("open", OnConnected);
        socket.On("register", OnRegister);
        socket.On("spawn", OnSpawn);
        socket.On("move", OnMove);
        socket.On("requestPosition", OnRequestPosition);
        socket.On("gameStart", OnGameStart);
        socket.On("gameFinish", OnGameFinish);
        socket.On("joined", OnJoined);
        socket.On("leaved", OnLeave);
        socket.On("updatePosition", OnUpdatePosition);
        socket.On("disconnected", OnDisconnected);
        socket.On("gameError", OnGameError);
        socket.On("picked", OnPicked);
        socket.On("error", OnError);
        socket.On("close", OnClose);
    }
    
    private void OnError(SocketIOEvent obj)
    {
        Debug.Log("Error Socket");
    }
    
    private void OnClose(SocketIOEvent obj)
    {
        Debug.Log("Close Socket");
    }

    private void OnGameError(SocketIOEvent obj)
    {
        gameController.GetComponent<GameController>().ShowError(obj.data["description"].str);
    }


    private void OnConnected(SocketIOEvent obj)
    {
        Debug.Log("conected");
    }

    private void OnJoined(SocketIOEvent obj)
    {
        gameController.GetComponent<GameController>().GameId = obj.data ["id"].str;
        gameController.GetComponent<GameController>().GameOwner = obj.data ["owner"].str;
        gameController.GetComponent<GameController>().LoginUi.SetActive(false);

        foreach (JSONObject pickable in obj.data["pickeables"].list)
        {
            spawner.SpawnPickeable(pickable["id"].str, pickable["type"].str, pickable["spot"].n, pickable["picked"].b);
        }
    }    

    private void OnLeave(SocketIOEvent obj)
    {

    }

    private void OnGameStart(SocketIOEvent obj)
    {
        gameController.GetComponent<GameController>().StartGameFromServer(obj.data["boss"].str);
    }

    private void OnGameFinish(SocketIOEvent obj)
    {
        gameController.GetComponent<GameController>().FinishGame(obj.data ["winner"].str);
    }    

    private void OnRegister(SocketIOEvent obj)
    {
        spawner.CreateLocalPlayer(obj.data["id"].str);
    }
    
    private void OnSpawn(SocketIOEvent obj)
    {
        if (spawner.GetLocalPlayerId() != obj.data["id"].str) 
        {
            spawner.AddNetPlayer(obj.data["id"].str, obj.data["nickName"].str, obj.data["healPoints"].n);
        }
    }

    private void OnMove(SocketIOEvent obj)
    {

        if (spawner.GetLocalPlayerId() != obj.data["id"].str) 
        {

            var position = GetVectorFromJson(obj.data["d"]);
            var rotation = GetVectorFromJson(obj.data["r"]);
            var player = spawner.GetPlayer(obj.data["id"].str);


            player.GetComponent<NetPlayer>().walking = obj.data["walking"].b;
            player.GetComponent<NetPlayer>().running = obj.data["running"].b;
            player.GetComponent<NetPlayer>().jumping = obj.data["jumping"].b;
            player.GetComponent<NetPlayer>().destination = position;     
            player.GetComponent<NetPlayer>().rotation = rotation;   
        }
    }


    private void OnUpdatePosition(SocketIOEvent obj)
    {
        var position = GetVectorFromJson(obj.data);
        var player = spawner.GetPlayer(obj.data["id"].str);
        player.transform.position = position;
    }

    private void OnRequestPosition(SocketIOEvent obj)
    {
        //if (socket.IsConnected) socket.Emit("updatePosition", VectorToJson(spawner.GetLocalPlayer().transform.position));
    }


    private void OnDisconnected(SocketIOEvent obj)
    {
        var disconnectedId = obj.data["id"].str;
        spawner.Remove(disconnectedId);
    }
    

    private void OnPicked(SocketIOEvent obj)
    {
        var pickedBy = obj.data["pickedBy"].str;
        var pickeableId = obj.data["id"].str;
        spawner.RemovePickeable(pickeableId);
        if (spawner.GetLocalPlayerId() == pickedBy)
        {
            //TO-DO Player pickup item
        }
    }

    private static Quaternion GetQuaternionFromJson(JSONObject obj)
    {
        return new Quaternion(float.Parse(obj["w"].str), float.Parse(obj["x"].str), float.Parse(obj["y"].str), float.Parse(obj["z"].str));
    }

    private static Vector3 GetVectorFromJson(JSONObject obj)
    {
        return new Vector3(float.Parse(obj["x"].str), float.Parse(obj["y"].str), float.Parse(obj["z"].str));
    }


    public static JSONObject VectorToJson(Vector3 vector)
    {
        JSONObject jsonObject = new JSONObject(JSONObject.Type.OBJECT);
        jsonObject.AddField("x", vector.x.ToString("F2"));
        jsonObject.AddField("y", vector.y.ToString("F2"));
        jsonObject.AddField("z", vector.z.ToString("F2"));
        return jsonObject;
    }

   public static JSONObject QuaternionToJson(Quaternion quaternion)
    {
        JSONObject jsonObject = new JSONObject(JSONObject.Type.OBJECT);
        jsonObject.AddField("w", quaternion.w.ToString("F2"));
        jsonObject.AddField("x", quaternion.x.ToString("F2"));
        jsonObject.AddField("y", quaternion.y.ToString("F2"));
        jsonObject.AddField("z", quaternion.z.ToString("F2"));
        return jsonObject;
    }    

    public static JSONObject PlayerIdToJson(string id)
    {
        JSONObject jsonObject = new JSONObject(JSONObject.Type.OBJECT);
        jsonObject.AddField("targetId", id);
        return jsonObject;
    }


    public static void Pick(string pickeableId, string pickedBy) 
    {
        JSONObject jsonObject = new JSONObject(JSONObject.Type.OBJECT);
        jsonObject.AddField("id", pickeableId);
        jsonObject.AddField("pickedBy", pickedBy);
        if (socket.IsConnected) socket.Emit("pick", jsonObject);
    }


    public static void Move(Vector3 rotation, Vector3 destination, bool walking, bool running, bool jumping)
    {
		JSONObject jsonObject = new JSONObject(JSONObject.Type.OBJECT);
		jsonObject.AddField("r", Network.VectorToJson(rotation));
		jsonObject.AddField("d", Network.VectorToJson(destination));
        jsonObject.AddField("walking", walking);
        jsonObject.AddField("running", running);
        jsonObject.AddField("jumping", jumping);
		if (socket.IsConnected) socket.Emit("move", jsonObject);
    }

    public static void Join(string roomName, string nickName) 
    {
        
        JSONObject jsonObject = new JSONObject(JSONObject.Type.OBJECT);
        jsonObject.AddField("room", roomName);
        jsonObject.AddField("nickName", nickName);
        if (socket.IsConnected) socket.Emit("join", jsonObject);
    }

    public static void StartGame(string gameId)
    {
        JSONObject jsonObject = new JSONObject(JSONObject.Type.OBJECT);
        jsonObject.AddField("id", gameId);
        if (socket.IsConnected) socket.Emit("gameStart", jsonObject);   
    }

    public static void Leave() 
    {
        JSONObject jsonObject = new JSONObject(JSONObject.Type.OBJECT);
        if (socket.IsConnected) socket.Emit("leave", jsonObject);   
    }

    public static void FinishGame(string winner) 
    {
        JSONObject jsonObject = new JSONObject(JSONObject.Type.OBJECT);
        jsonObject.AddField("winner", winner);
        if (socket.IsConnected) socket.Emit("gameFinish", jsonObject); 
    }

}
