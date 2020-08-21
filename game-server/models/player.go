package models

type Player struct {
    Id              string          `json:"id"`
    Name            string          `json:"name"`
    SID             string          `json:"sId"`
    Room            string          `json:"room"`
    NickName        string          `json:"nickName"`
    Destination     UnityVector3    `json:"destination"`
    LastPosition    UnityVector3    `json:"lastposition"`
    Role            string          `json:"role"`
    HealPoints      int             `json:"healPoints"`
}

