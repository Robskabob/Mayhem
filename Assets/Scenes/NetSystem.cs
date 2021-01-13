using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetSystem : NetworkManager
{
    public List<NetPlayer> Players;

    public PlayerBrain GamePlayer;

	public override void OnClientConnect(NetworkConnection conn)
	{
		base.OnClientConnect(conn);
	}

	public override void OnServerReady(NetworkConnection conn)
	{
		base.OnServerReady(conn);
		PlayerBrain PB = Instantiate(GamePlayer);
		NetworkServer.Spawn(PB.gameObject,conn);
	}
}
