using System.Collections.Generic;
using Mirror;
using UnityEngine;
using L33t.Equipment;
using L33t.UI;
using L33t.Network;
using L33t;
#if UNITY_EDITOR
using UnityEditor;



[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
	GameManager GM;


	void OnEnable()
	{
		GM = target as GameManager;
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if (GUILayout.Button("Start GameMode")) 
		{
			GM.GameMode.StartGame(GM);
		}
	}
}
#endif



public class GameManager : MonoBehaviour
{
	public NetSystem NetSystem;
	public RegionManager RegionManager;

	public Lobby Lobby;
	public GameMode GameMode;
}


public class Lobby
{

}
[System.Serializable]
public class GameStats
{
	public float Time;
	public TeamStats[] Teams;

	[System.Serializable]
	public class TeamStats
    {
		public int Team;
		public int Score;

		public List<PlayerStats> PlayerStats;
	}
	[System.Serializable]
	public class PlayerStats
    {
		public uint PlayerID;
		public string PlayerName;
		public int Kills;
		public int Assists;
		public int Deaths;
		public int Scores;
		public float FlagTime;
		
		public int[] Stats = new int[(int)StatType.Last - 1];
	}

	public PlayerStats GetPlayerStats(int Team, int PlayerID)
    {
		return Teams[Team].PlayerStats[PlayerID];
    }
	public void AddStat(int Team, int PlayerID, StatType statType, int Score)
    {
		Teams[Team].PlayerStats[PlayerID].Stats[(int)statType] += Score;
    }
	public int GetStat(int Team, int PlayerID, StatType statType)
    {
		return Teams[Team].PlayerStats[PlayerID].Stats[(int)statType];
    }

	public enum StatType
    {
		//Kills
		Kill,
		Death,
		Assist,
		NPCKill,
		Suicide,
		TeamKill,
		//Flag
		FlagTime,
		FlagKill,
		FlagDeath,
		FlagScore,
		FlagDefend,
		FlagRetrive,
		//Damage
		DamageDelt,
		FriendlyFire,
		DamageTaken,



		Last
    }

}