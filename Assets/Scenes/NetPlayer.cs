using Mirror;
using System.Collections.Generic;
using UnityEngine;

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
	private void CmdGetState()
	{
		PlayerData[] playerdata = new PlayerData[NetSystem.I.Players.Count];
		int i = 0;
		foreach (var vk in NetSystem.I.Players)
		{
			playerdata[i] = new PlayerData(NetSystem.I.PlayerBrains[vk.Value.netId].netId,vk.Value.netId, vk.Value.Name, vk.Value.Color);
			i++;
		}
		RpcGetState(playerdata);
	}

	[Command]
	private void CmdName(string n)
	{
		Name = n;
		NetSystem.I.PlayerBrains[netId].Body.NamePlate.text = n;
		RpcName(n);
	}

	[Command]
	private void CmdColor(Color c)
	{
		Color = c;
		NetSystem.I.PlayerBrains[netId].Body.GetComponent<SpriteRenderer>().color = c;
		RpcColor(c);
	}
	[TargetRpc]
	private void RpcGetState(PlayerData[] playerdata)
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

			NetSystem.I.PlayerBrains.Add(PD.netid,PB);
			NetSystem.I.Players.Add(PD.netid,NPI.GetComponent<NetPlayer>());

			PB.Body.NamePlate.text = PD.name;
			PB.Body.GetComponent<SpriteRenderer>().color = PD.color;
		}
	}

	[ClientRpc]
	private void RpcName(string n)
	{
		Name = n;
		NetSystem.I.PlayerBrains[netId].Body.NamePlate.text = n;
	}

	[ClientRpc]
	private void RpcColor(Color c)
	{
		Color = c;
		NetSystem.I.PlayerBrains[netId].Body.GetComponent<SpriteRenderer>().color = c;
	}

	[Command]
	public void CmdChat(string M)
	{
		//log ?
		RpcChat(new Message(Name,Color,M));
	}
	[ClientRpc]
	public void RpcChat(Message M)
	{
		PlayerClient.PC.Chat.Messages.Add(M);
		PlayerClient.PC.Chat.Timeing.Add(15);
	}
}
