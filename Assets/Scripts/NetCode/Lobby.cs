using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lobby : MonoBehaviour
{
    public NetSystem NetSystem;
    public List<RoomPlayer> RoomPlayers;

    public void StartGame() 
    {
    
    }
    public void EndGame() 
    {
    
    }
}

public class Map 
{

}
public class GameMode 
{
    public int MaxPlayers;
    public int Teams;

    public void SetupTeams() { }
}
