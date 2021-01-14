using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetSystem : NetworkManager
{
	public static NetSystem I;
    public List<NetPlayer> Players;
    public Dictionary<uint,PlayerBrain> PlayerBrains = new Dictionary<uint, PlayerBrain>();

    public PlayerBrain GamePlayer;

	private void Start()
	{
		I = this;
	}

	public override void OnClientConnect(NetworkConnection conn)
	{
		base.OnClientConnect(conn);
	}

	public override void OnServerReady(NetworkConnection conn)
	{
		base.OnServerReady(conn);
		PlayerBrain PB = Instantiate(GamePlayer);
		NetworkServer.Spawn(PB.gameObject,conn);
		PlayerBrains.Add(PB.netId,PB);
	}
}
