using UnityEngine;
using System.Collections;

public class NetworkEntity : MonoBehaviour
{
    public string id;
    public string nickName;
    public float healPoints;
    public string role;



    public bool isDead() 
    {
    	return healPoints <= 0f;
    }


    public bool isBoss()
    {
    	return role == "boss";
    }	
}
