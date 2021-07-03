using L33t.UI;
using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace L33t.Network
{
	public class NetPlayer : NetworkBehaviour
	{
		public string Name;
		public Color Color;
		public int Team;

		public override void OnStartLocalPlayer()
		{
			base.OnStartLocalPlayer();
			PlayerClient.PC.NP = this;
		}

		public struct PlayerData
		{
			public uint brainid;
			public uint netid;
			public string name;
			public Color color;

			public PlayerData(uint brainid, uint netid, string name, Color color)
			{
				this.brainid = brainid;
				this.netid = netid;
				this.name = name;
				this.color = color;
			}
		}

		private void Start()
		{
			if (isLocalPlayer)
			{
				ColorPicker CP = ColorPicker.CP;
				CmdColor(CP.C);
				CmdName(CP.N.text);
				CmdGetState();
			}
		}

		[Command]
		public void CmdGetState()
		{
			PlayerData[] playerdata = new PlayerData[NetSystem.I.Players.Count];
			int i = 0;
			foreach (var vk in NetSystem.I.Players)
			{
				playerdata[i] = new PlayerData(NetSystem.I.PlayerBrains[vk.Value.netId].netId, vk.Value.netId, vk.Value.Name, vk.Value.Color);
				i++;
			}
			RpcGetState(playerdata);
		}

		[Command]
		public void CmdName(string n)
		{
			Name = n;
			NetSystem.I.PlayerBrains[netId].Body.NamePlate.text = n;
			RpcName(n);
		}

		[Command]
		public void CmdColor(Color c)
		{
			Color = c;
			NetSystem.I.PlayerBrains[netId].Body.GetComponent<SpriteRenderer>().color = c;
			RpcColor(c);
		}
		[TargetRpc]
		public void RpcGetState(PlayerData[] playerdata)
		{
			for (int i = 0; i < playerdata.Length; i++)
			{
				PlayerData PD = playerdata[i];
				if (PD.netid == netId)
					continue;
				Name = PD.name;
				Color = PD.color;
				NetworkIdentity NPI = NetworkIdentity.spawned[PD.netid];
				NetworkIdentity PBI = NetworkIdentity.spawned[PD.brainid];

				PlayerBrain PB = PBI.GetComponent<PlayerBrain>();

				NetSystem.I.PlayerBrains.Add(PD.netid, PB);
				NetSystem.I.Players.Add(PD.netid, NPI.GetComponent<NetPlayer>());

				PB.Body.NamePlate.text = PD.name;
				PB.Body.GetComponent<SpriteRenderer>().color = PD.color;
			}
		}

		[ClientRpc]
		public void RpcName(string n)
		{
			Name = n;
			NetSystem.I.PlayerBrains[netId].Body.NamePlate.text = n;
		}

		[ClientRpc]
		public void RpcColor(Color c)
		{
			Color = c;
			NetSystem.I.PlayerBrains[netId].Body.GetComponent<SpriteRenderer>().color = c;
		}

		[Command]
		public void CmdChat(string M)
		{
			//log ?
			Message msg = new Message(Name, Color, M);
			string str = msg.ParseCommand();
			if (str == "Not Command")
				RpcChat(msg);
			else if (str.Contains("Done"))
			{
				RpcTargetChat(new Message("Command", Color.blue, str));
			}
			else
			{
				RpcTargetChat(new Message("Command", Color.blue, "<color=red>" + str + "</color>"));
			}

		}
		[ClientRpc]
		public void RpcChat(Message M)
		{
			PlayerClient.PC.Chat.AddMessage(M);
		}
		[TargetRpc]
		public void RpcTargetChat(Message M)
		{
			PlayerClient.PC.Chat.AddMessage(M);
		}
	}
}