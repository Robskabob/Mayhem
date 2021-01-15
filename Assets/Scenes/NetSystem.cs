using System.Collections.Generic;
using Mirror;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class NetSystem : NetworkManager
{
	public static NetSystem I;
    public Dictionary<uint, NetPlayer> Players = new Dictionary<uint, NetPlayer>();
    public Dictionary<uint, PlayerBrain> PlayerBrains = new Dictionary<uint, PlayerBrain>();

    public PlayerBrain GamePlayer;

	public override void Start()
	{
		NetworkServer.RegisterHandler<JoinMessage>(OnJoinGame);
		NetworkClient.RegisterHandler<JoinedMessage>(OnJoinedGame);
		base.Start();
		I = this;
	}

	public override void OnClientConnect(NetworkConnection conn)
	{
		base.OnClientConnect(conn);
		JoinMessage Join = new JoinMessage();
		Join.Name = PlayerClient.PC.CP.N.text;
		Join.Color = PlayerClient.PC.CP.C;
		conn.Send(Join);
	}

	public override void OnServerReady(NetworkConnection conn)
	{
		base.OnServerReady(conn);
	}

	public void OnJoinGame(NetworkConnection conn, JoinMessage Join)
	{
		NetPlayer NP = Instantiate(playerPrefab).GetComponent<NetPlayer>();
		NetworkServer.AddPlayerForConnection(conn, NP.gameObject);

		PlayerBrain PB = Instantiate(GamePlayer);
		NetworkServer.Spawn(PB.gameObject, conn);

		//Players.Add(NP.netId, NP);
		//PlayerBrains.Add(NP.netId, PB);

		JoinedMessage joined = new JoinedMessage();
		Debug.Log(NP.netId+"|"+PB.netId);
		joined.id = NP.netId;
		joined.Player = PB.netId;
		NetworkServer.SendToAll(joined);
	}
	public void OnJoinedGame(NetworkConnection conn, JoinedMessage Join)
	{
		Debug.Log(NetworkIdentity.spawned.Count+"C");
		Debug.Log(Join.id+"|"+Join.Player);
		//Debug.Log(NetworkIdentity.spawned.TryGetValue(Join.Player, out) +"PBI");
		NetworkIdentity NPI = NetworkIdentity.spawned[Join.id];
		NetworkIdentity PBI = NetworkIdentity.spawned[Join.Player];
		//Debug.Log(NetworkIdentity.spawned.TryGetValue(Join.id, out NetworkIdentity NPI) + "NPI");
		NetPlayer NP = NPI.GetComponent<NetPlayer>();

		PlayerBrain PB = PBI.GetComponent<PlayerBrain>();

		Players.Add(Join.id, NP);
		PlayerBrains.Add(Join.id, PB);
		Debug.Log("Completed");
	}

	public struct JoinMessage : NetworkMessage
	{
		public string Name;
		public Color Color;
		public int Team;
	}
	public struct JoinedMessage : NetworkMessage
	{
		public uint id;
		public uint Player;
	}
}

#if UNITY_EDITOR

[CustomEditor(typeof(NetSystem))]
public class NetsystemInspector : NetworkManagerEditor 
{
	NetSystem NS;

	private void OnEnable()
	{
		NS = target as NetSystem;
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		DrawDefaultInspector();
		foreach (var kv in NS.PlayerBrains)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.IntField("net id", (int)kv.Key);
			EditorGUILayout.ObjectField("net id", kv.Value, typeof(PlayerBrain), true);
			EditorGUILayout.EndHorizontal();
		}
		foreach (var kv in NS.Players)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.IntField("net id", (int)kv.Key);
			EditorGUILayout.ObjectField("net id", kv.Value, typeof(PlayerBrain), true);
			EditorGUILayout.EndHorizontal();
		}
	}
}
#endif