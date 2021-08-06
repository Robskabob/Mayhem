using System.Collections.Generic;
using Mirror;
using UnityEngine;
using L33t.Equipment;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace L33t.Network
{
	public class NetSystem : NetworkManager
	{
		public static NetSystem I;
		public Dictionary<uint, NetPlayer> Players = new Dictionary<uint, NetPlayer>();
		public Dictionary<uint, PlayerBrain> PlayerBrains = new Dictionary<uint, PlayerBrain>();

		public PlayerBrain GamePlayer;

		public List<Equipment.Equipment> Equipment;//shouldent be here
		public ItemDrop ItemBox;
		public PlatBrain PlatGuy;

		public override void Start()
		{
			Registry.Reg.Equipment = Equipment;
			ClientScene.RegisterPrefab(GamePlayer.gameObject, SpawnPlayerBrain, UnSpawn);
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

		//abuse
		public void OnJoinGame(NetworkConnection conn, JoinMessage Join)
		{
			NetPlayer NP = Instantiate(playerPrefab).GetComponent<NetPlayer>();
			NetworkServer.AddPlayerForConnection(conn, NP.gameObject);

			PlayerBrain PB = Instantiate(GamePlayer);
			NetworkServer.Spawn(PB.gameObject, conn);

			for (int i = 0; i < 25; i++)
			{
				ItemDrop IB = Instantiate(ItemBox);
				IB.Phase = 2;
				NetworkServer.Spawn(IB.gameObject);
			}
			for (int i = 0; i < 5; i++)
			{
				PlatBrain PlB = Instantiate(PlatGuy);
				PlB.transform.position = new Vector3(300, 300, 0) * Random.insideUnitCircle;
				NetworkServer.Spawn(PlB.gameObject);
				PlB.Die();
			}

			//Players.Add(NP.netId, NP);
			//PlayerBrains.Add(NP.netId, PB);

		JoinedMessage joined = new JoinedMessage();
		//Debug.Log(NP.netId+"|"+PB.netId);
		joined.id = NP.netId;
		joined.Player = PB.netId;
		NetworkServer.SendToAll(joined);
	}
	public void OnJoinedGame(NetworkConnection conn, JoinedMessage Join)
	{
		//Debug.Log(NetworkIdentity.spawned.Count+"C");
		//Debug.Log(Join.id+"|"+Join.Player);
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

		GameObject SpawnPlayerBrain(SpawnMessage msg)
		{
			PlayerBrain PB = Instantiate(GamePlayer);

			if (PB.hasAuthority)
			{
				Mob M = PB.gameObject.AddComponent<Mob>();
				M.B = PB;
			}
			else
			{
				NetMob NM = PB.gameObject.AddComponent<NetMob>();
				NM.B = PB;
			}

			return PB.gameObject;
		}

		void UnSpawn(GameObject O)
		{
			Destroy(O);
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
}