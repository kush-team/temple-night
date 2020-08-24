package main

import (
    "fmt"
    "log"
    "net/http"
    "math/rand"
    "github.com/googollee/go-socket.io"
    "github.com/satori/go.uuid"
)



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
    Role            string          `json:"role"`
    HealPoints      float32         `json:"healPoints"`
    Pickeables[]    *Pickeable      `json:"pickeables"`
}

type PickEvent struct {
    Id              string          `json:"id"`
    PickedBy        string          `json:"pickedBy"`
}

type Pickeable struct {
    Id              string          `json:"id"`
    Type            string          `json:"type"`
    Picked          bool            `json:"picked"`
    Spot            int             `json:"spot"`
}

type Game struct {
    Id              string          `json:"id"`
    Winner          string          `json:"winner"`
    Room            string          `json:"room"`
    Owner           string          `json:"owner"`
    Started         bool            `json:"started"`
    Ended           bool            `json:"ended"`
    Boss            string          `json:"boss"`
    Pickeables[]    *Pickeable      `json:"pickeables"`
}

type RoomJoin struct {
    Room            string          `json:"room"`
    NickName        string          `json:"nickname"`
}

type RegisterUser struct {
    Id              string           `json:"id"` 
}


type ErrorResponse struct {
    Code            int              `json:"code"` 
    Description     string           `json:"description"` 
}

var players[] *Player
var games[]   *Game

var PickeablesTypes = [3]string {"weed", "grinder", "roll-paper"}

const MaxSpots int = 30
const MaxPickeables int = 10



func main() {
    server, err := socketio.NewServer(nil)
    if err != nil {
        log.Fatal(err)
    }

    server.OnConnect("/", func(s socketio.Conn) error {
        fmt.Println("Player Enter")
        uid := uuid.Must(uuid.NewV4())
        players = append(players, &Player{Id: uid.String(), SID: s.ID(), Role: "player", HealPoints: 100, Pickeables: []*Pickeable{}})
        
        dat := &RegisterUser{Id: uid.String()}

        s.SetContext("")
        s.Emit("register", dat)
        return nil
    })



    server.OnEvent("/", "join", func(s socketio.Conn, msg RoomJoin) {
        dat := make(map[string]interface{})
        fmt.Printf("Player Join or Create Game %s", msg.Room)

        var currentPlayerIndex = -1
        for i := range players {
            if players[i].SID == s.ID() {
                players[i].Room = msg.Room
                players[i].NickName = msg.NickName
                currentPlayerIndex = i
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
            game = &Game{Id: uid.String(), Owner: players[currentPlayerIndex].Id, Room: msg.Room, Pickeables: createPickeables()}
            
        }

        games = append(games, game)

        if (game.Started == false) {

            server.BroadcastToRoom("", msg.Room, "spawn", players[currentPlayerIndex])
            
            s.Emit("joined", game)
            
            room := msg.Room

            s.Join(room)

            //server.BroadcastToRoom("", room, "requestPosition")

            for i := range players {
                if (players[i].Id != dat["id"] && players[i].Room == room) {
                    s.Emit("spawn", players[i])
                }
            }
        } else {
            s.Emit("gameError", &ErrorResponse{Code: 05, Description: "The game you are trying to enter is already in progress or finished"})
        }
        
    })    

    server.OnEvent("/", "move", func(s socketio.Conn, msg UnityMovement) {
        room := ""
        fmt.Println(msg);
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
                games[i].Started = true
                pInRoom := getPlayersInRoom(games[i].Room)
                
                n := rand.Int() % len(pInRoom)
                games[i].Boss = pInRoom[n].Id
                server.BroadcastToRoom("", games[i].Room, "gameStart", games[i])
            }
        }
    })

    server.OnEvent("/", "gameFinish", func(s socketio.Conn, msg Game) {
        fmt.Println("Game End")
        for i := range games {
            if games[i].Id == msg.Id {
                games[i].Ended = true
            }
        }       
        server.BroadcastToRoom("", msg.Room, "gameFinish", msg)
    })



    server.OnEvent("/", "pick", func(s socketio.Conn, msg PickEvent) {
        room := ""
        for i := range players {
            if players[i].SID == s.ID() {
                msg.PickedBy = players[i].Id
                room = players[i].Room
                for i := range games {
                    if games[i].Room == room {
                        players[i].Pickeables = append(players[i].Pickeables, getPickeableById(games[i].Pickeables, msg.Id))
                    }
                }          
            }
        }

        server.BroadcastToRoom("", room, "picked", msg)
    })    


    server.OnEvent("/", "updatePosition", func(s socketio.Conn, msg UnityVector3) {
        room := ""
        for i := range players {
            if players[i].SID == s.ID() {
                players[i].LastPosition = msg
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


func createPickeables() []* Pickeable  {
    p := []*Pickeable{}

    spotList := [] int{}
    for i := 0; i < MaxSpots; i++ {
        spotList = append(spotList, i)
    }

    for i := 0; i < MaxPickeables; i++ {
        uid := uuid.Must(uuid.NewV4())
        typeIndex := rand.Int() % len(PickeablesTypes)
        spot := rand.Int() % len(spotList)
        p = append(p, &Pickeable{Type: PickeablesTypes[typeIndex], Id: uid.String(), Picked: false, Spot: spotList[spot]})
        spotList = append(spotList[:spot], spotList[spot+1:]...)
    }
    return p
}
func getPickeableById(pickeables []* Pickeable, id string) *Pickeable {
    p := &Pickeable{}

    for i := range pickeables {
        if pickeables[i].Id == id {
            p = pickeables[i]   
        }
    }  

    return p
}

func removePlayerByIndex(s []*Player, index int) []*Player {
    return append(s[:index], s[index+1:]...)
}


func getPlayersInRoom(roomName string) []*Player {
    var pInRoom[] *Player
    for i := range players {
        if players[i].Room == roomName {
            pInRoom = append(pInRoom, players[i])
        }
    }    
    return pInRoom
}

