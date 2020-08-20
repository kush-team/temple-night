package main

import (
    "fmt"
    "log"
    "net/http"
    "github.com/googollee/go-socket.io"
    "github.com/satori/go.uuid"
)


type Game struct {
    Id              string          `json:"id"`
    Winner          string          `json:"winner"`
    Room            string          `json:"room"`
    Owner           string          `json:"owner"`
}

type UnityVector3 struct {
    Id              string          `json:"id"`
    X               float32         `json:"x"`
    Y               float32         `json:"y"`
    Z               float32         `json:"z"`
}


type UnityQuaternion struct {
    Id              string          `json:"id"`
    W               float32         `json:"w"`
    X               float32         `json:"x"`
    Y               float32         `json:"y"`
    Z               float32         `json:"z"`
}


type UnityMovement struct {
    Id              string          `json:"id"`    
    R               UnityVector3    `json:"r"`
    D               UnityVector3    `json:"d"`
    Walking         bool            `json:"walking"`
    Running         bool            `json:"running"`    
    Jumping         bool            `json:"jumping"`    
}

type Player struct {
    Id              string          `json:"id"`
    Name            string          `json:"name"`
    SID             string          `json:"sId"`
    Room            string          `json:"room"`
    NickName        string          `json:"nickName"`
    Destination     UnityVector3    `json:"destination"`
    LastPosition    UnityVector3    `json:"lastposition"`
}

type RoomJoin struct {
    Room            string          `json:"room"`
    NickName        string          `json:"nickname"`
}

type RegisterUser struct {
    Id              string           `json:"id"` 
}


var players[] *Player
var games[]   *Game




func main() {
    server, err := socketio.NewServer(nil)
    if err != nil {
        log.Fatal(err)
    }

    server.OnConnect("/", func(s socketio.Conn) error {
        fmt.Println("Player Enter")
        uid := uuid.Must(uuid.NewV4())
        players = append(players, &Player{Id: uid.String(), SID: s.ID()})
        
        dat := &RegisterUser{Id: uid.String()}

        s.SetContext("")
        s.Emit("register", dat)
        return nil
    })



    server.OnEvent("/", "join", func(s socketio.Conn, msg RoomJoin) {
        dat := make(map[string]interface{})
        fmt.Printf("Player Join or Create Game %s", msg.Room)

        var owner = ""
        for i := range players {
            if players[i].SID == s.ID() {
                players[i].Room = msg.Room
                players[i].NickName = msg.NickName
                owner = players[i].Id
                server.BroadcastToRoom("", msg.Room, "spawn", players[i])
            }
        }


        game := &Game{}

        for i := range games {
            if games[i].Room == msg.Room {
                game = games[i]
            }
        }

        if (game.Id == "") {
            uid := uuid.Must(uuid.NewV4())
            game = &Game{Id: uid.String(), Owner: owner, Room: msg.Room}
        }


        s.Emit("joined", game)

        games = append(games, game)


        room := msg.Room
        s.Join(room)

        
        server.BroadcastToRoom("", room, "requestPosition")
        
        for i := range players {
            if (players[i].Id != dat["id"] && players[i].Room == room) {
                s.Emit("spawn", players[i])
            }
        }        
    })    

    server.OnEvent("/", "move", func(s socketio.Conn, msg UnityMovement) {
        room := ""

        for i := range players {
            if players[i].SID == s.ID() {
                msg.Id = players[i].Id
                room = players[i].Room
            }
        }
        server.BroadcastToRoom("", room, "move", msg)
    })


    
    server.OnEvent("/", "gameStart", func(s socketio.Conn, msg Game) {
        fmt.Println("Game Start")
        for i := range games {
            if games[i].Id == msg.Id {
                server.BroadcastToRoom("", games[i].Room, "gameStart", games[i])
            }
        }
    })

    server.OnEvent("/", "gameFinish", func(s socketio.Conn, msg Game) {
        server.BroadcastToRoom("", msg.Room, "gameFinish", msg)
    })


    server.OnEvent("/", "updatePosition", func(s socketio.Conn, msg UnityVector3) {
        room := ""
        for i := range players {
            if players[i].SID == s.ID() {
                msg.Id = players[i].Id
                room = players[i].Room
            }
        }

        server.BroadcastToRoom("", room, "updatePosition", msg)
    })    


    server.OnError("/", func(s socketio.Conn, e error) {
        fmt.Println("meet error:", e)
    })

    server.OnDisconnect("/", func(s socketio.Conn, reason string) {
        dat := make(map[string]interface{})
        itr := -1
        room := ""
        for i := range players {
            if players[i].SID == s.ID() {
                itr = i
                dat["id"] = players[i].Id
                room = players[i].Room
            }
        }
        if (itr >= 0)  {
            players = removePlayerByIndex(players, itr)
            server.BroadcastToRoom("", room, "disconnected", dat)
            fmt.Println("closed", reason)
        }
    })

    go server.Serve()
    defer server.Close()

    http.Handle("/socket.io/", server)
    http.Handle("/", http.FileServer(http.Dir("./asset")))
    log.Println("Serving at localhost:8000...")
    log.Fatal(http.ListenAndServe(":8000", nil))
}


func removePlayerByIndex(s []*Player, index int) []*Player {
    return append(s[:index], s[index+1:]...)
}

