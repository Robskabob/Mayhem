using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using L33t.Equipment;
using L33t.UI;
using L33t.Network;
using L33t;

[System.Serializable]
public class GameMode
{
	//settings
	public int TeamCount = 2;
	public int RoundCount = 3;
	public int ScoreTarget = 15;
	public float RoundTimeLimit = 3600;
	public int DeathLimit = -1;
	public float RespwanWait = 5;
	public Color[] TeamColors;

	//PointValues
	public int Kill = 1;
	public int Death = 0;


	//RunTime
	//public List<Team> Teams;
	public Dictionary<uint,int> PlayerIDs;
	public int CurrentRound;
	public int Score;

	public GameStats Stats;
    public bool isActive = false;

    public void StartGame(GameManager GM)
	{
		//for (int i = 0; i < GM.NetSystem.PlayerBrains.Count; i++)
		//      {
		//	GM.NetSystem.PlayerBrains[i].tea

		//}
		PlayerIDs = new Dictionary<uint, int>();

		Stats = new GameStats();
		Stats.Teams = new GameStats.TeamStats[TeamCount];
		TeamColors = new Color[TeamCount];

		int i;
		for (i = 0; i < TeamCount; i++)
		{
			Stats.Teams[i] = new GameStats.TeamStats();
			Stats.Teams[i].Team = i;
			Stats.Teams[i].PlayerStats = new List<GameStats.PlayerStats>();

			TeamColors[i] = Color.HSVToRGB(1f/TeamCount*i,1,1);
        }


		i = 0;
		foreach (var vk in GM.NetSystem.Players)
		{
			PlayerBrain PB = GM.NetSystem.PlayerBrains[vk.Value.netId];
			NetPlayer NP = GM.NetSystem.Players[vk.Value.netId];

			PB.OnDeath += OnPlayerDeath;
			PB.OnDamageDelt += OnPlayerDamageDelt;
			PB.OnDamageTaken += OnPlayerDamageTaken;

			int Team = i % TeamCount;
			PB.Team = Team;

			NP.Team = Team;
			NP.CmdColor(TeamColors[Team]);


			GameStats.PlayerStats stat = new GameStats.PlayerStats();

			stat.PlayerID = vk.Value.netId;
			stat.PlayerName = NP.Name;

			Debug.Log($"IDs Add ID:{vk.Value.netId}, #{Stats.Teams[i].PlayerStats.Count}");
			PlayerIDs.Add(vk.Value.netId,Stats.Teams[i].PlayerStats.Count);
			Stats.Teams[i].PlayerStats.Add(stat);

			i++;
		}
		isActive = true;
	}


	public void LoadGame() { }
	public void UnLoadGame() { }
	public void OnPlayerJoin() { }
	public void OnPlayerLeave() { }

	//Game Events
	public void OnPlayerDeath(PlayerBrain player, DamageSource damageSource)
	{
		Stats.Teams[player.Team].Score += Death;
		Stats.Teams[player.Team].PlayerStats[PlayerIDs[player.NetPlayerID]].Deaths++;
		Stats.AddStat(player.Team, PlayerIDs[player.NetPlayerID], GameStats.StatType.Death, 1);
		if (damageSource.Source.B is PlayerBrain PB)
        {
			if(PB.Team == player.Team)
            {
				if(PB.netId == player.netId)
				{
					//suicide
					Stats.AddStat(player.Team, PlayerIDs[player.NetPlayerID], GameStats.StatType.Suicide, 1);
				}
                else
                {
					//Team kill
					Stats.AddStat(player.Team, PlayerIDs[player.NetPlayerID], GameStats.StatType.TeamKill, 1);
				}
            }
            else
            {
				Stats.Teams[PB.Team].Score += Kill;
				Stats.Teams[PB.Team].PlayerStats[PlayerIDs[player.NetPlayerID]].Kills++;
				Stats.AddStat(player.Team, PlayerIDs[player.NetPlayerID], GameStats.StatType.Kill, 1);
			}
        }
	}
	public void OnPlayerDamageDelt(PlayerBrain player, DamageSource damageSource, float damage)
	{
		//Stats.Teams[player.Team].Score += damage;
		if (damageSource.Source.B is PlayerBrain PB)
		{
			if (PB.Team == player.Team)
			{
				if (PB.netId == player.netId)
				{
					//suicide
				}
				else
				{
					Stats.AddStat(player.Team, PlayerIDs[player.NetPlayerID], GameStats.StatType.FriendlyFire, (int)(damage * 100));
				}
			}
			else
			{
				Stats.AddStat(player.Team, PlayerIDs[player.NetPlayerID], GameStats.StatType.DamageDelt, (int)(damage * 100));
			}
		}
        else
        {
			//From AI
			Stats.AddStat(player.Team,PlayerIDs[player.NetPlayerID],GameStats.StatType.DamageDelt, (int)(damage * 100));
        }
	}
	public void OnPlayerDamageTaken(PlayerBrain player, DamageSource damageSource, float damage)
	{
		//Stats.Teams[player.Team].Score += damage;
		if (damageSource.Source.B is PlayerBrain PB)
		{
			if (PB.Team == player.Team)
			{
				if (PB.netId == player.netId)
				{
					//suicide
				}
				else
				{
					Stats.AddStat(player.Team, PlayerIDs[player.NetPlayerID], GameStats.StatType.FriendlyFire, (int)(damage * 100));
				}
			}
			else
			{
				Stats.AddStat(player.Team, PlayerIDs[player.NetPlayerID], GameStats.StatType.DamageTaken, (int)(damage * 100));
			}
		}
		else
		{
			//From AI
			Stats.AddStat(player.Team, PlayerIDs[player.NetPlayerID], GameStats.StatType.DamageTaken, (int)(damage * 100));
		}
	}

	public void OnPlayerKill() { }
	public void OnPlayerAssist() { }
	public void OnPlayerScoreFlag() { }
	public void OnPlayerPickupFlag() { }
	public void OnPlayerDropFlag() { }
}

public class FlagItem : ActiveEquipment
{
	public Mob M;

	public int Team;
	public float Timer;



		[Command]
		public override void Drop()
		{
			PickUpAble = true;
			ExpireTime = 30;
			Abandand = true;
			transform.parent = null;
			netIdentity.RemoveClientAuthority();
			M = null;
			RpcDrop();
		}
		[ClientRpc]
		public void RpcDrop()
		{
			PickUpAble = true;
			transform.parent = null;
			M = null;
		}

		public override bool Pickup(Mob M)
		{
			if (!M.PickUp(this))
				return false;
			Abandand = false;
			transform.parent = M.transform;
			transform.localPosition = Vector3.zero;
			this.M = M;

			netIdentity.AssignClientAuthority(M.B.netIdentity.connectionToClient);

			RpcPickup(M.B.netId);
			return true;
		}
		[ClientRpc]
		public void RpcPickup(uint MobId)
		{
			Abandand = false;
			if (!isServer)
			{
				Mob M = NetworkIdentity.spawned[MobId].GetComponent<Brain>().Body;
				transform.parent = M.transform;
				transform.localPosition = Vector3.zero;
				this.M = M;
				if (!M.PickUp(this))
					Debug.LogError("Cant pick up but valid on Server?");
			}
		}

		protected override void Update()
		{
			base.Update();

		}

		public override void Use()
		{
			//AOE or Boost?
		}

		public override string PrintStats()
		{
			return
				$"Flag\n" +
				$"Flag\n" +
				$"Flag";
		}

		public override void Randomize()
		{
			SetStats(new FlagStats(this));
		}

		[ClientRpc]
		public void SetStats(FlagStats stats)
		{
			stats.Set(this);
		}


	public struct FlagStats
	{
		public FlagStats(FlagItem Base)
		{
		
		}

		public void Set(FlagItem Base)
		{

		}
	}
}

public class ObjectiveTargets
{

}

public class SpawnPoint
{
	public int Team;
	public int Round;
}