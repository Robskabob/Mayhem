using L33t.Network;
using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class Region : MonoBehaviour 
{
	public static readonly Vector2[] Offsets = new Vector2[6]
	{
		new Vector2(50,100),
		new Vector2(100,0),
		new Vector2(50,-100),
		new Vector2(-50,-100),
		new Vector2(-100,0),
		new Vector2(-50,100)
	};

	public static RegionManager RegionManager;
	//public static Dictionary<Vector2Int, Region> GlobalRegions = new Dictionary<Vector2Int, Region>();
	//public MapGenerator MapGen;

	//public Region OriginDefualt;
	//public static Region Defualt;

	public SpriteRenderer SR;
	public Region[] Neighbors = new Region[6];
	public List<PlayerBrain> Players;
	public List<Mob> Mobs;
	public bool Active;
	public int loadLevel;

	public List<Transform> Objects;
	public Transform Contents;
	public MapGenerator.Chunk chunk;

	private void Start()
	{
		if(!RegionManager.GlobalRegions.ContainsKey(Vector2Int.RoundToInt(transform.position)))
			RegionManager.GlobalRegions.Add(Vector2Int.RoundToInt(transform.position), this);
	}

	public void OnActivate() 
	{
		Active = true;
		SR.color = new Color(0, .5f, 0);
		Contents.gameObject.SetActive(true);

		for (int i = 0; i < Mobs.Count; i++) 
		{
			//Mobs[i].enabled = true;
			Mobs[i].rb.simulated = true;
			Mobs[i].gameObject.SetActive(true);
		}
	}
	public void OnDeActivate()
	{
		Active = false;
		SR.color = new Color(.5f, 0, 0);
		Contents.gameObject.SetActive(false);

		for (int i = 0; i < Mobs.Count; i++)
		{
			Mobs[i].rb.simulated = false;
			Mobs[i].gameObject.SetActive(false);
			//Mobs[i].enabled = false;
		}
	}
	public void OnNeighborActivated()
	{
		OnActivate();
		if (loadLevel < 10)
		{
			FirstEnter();
		}
		else
			loadLevel = 15;
	}
	public void OnNeighborDeActivated()
	{
		if (Players.Count != 0)
			return;
		for (int i = 0; i < Neighbors.Length; i++)
		{
			if (Neighbors[i] != null && Neighbors[i].Players.Count != 0)
			{
				return;
			}
		}
		OnDeActivate();
	}

	public void MobEnter(Mob body)
	{
		//if (body.Inside != null) 
		//{
		//	Debug.Log("double in");
		//	body.Inside.MobExit(body);
		//}
		//body.Inside = this;
		body.OnDeath += MobDied;
		Mobs.Add(body);
		if (!Active)
		{
			body.enabled = false;
			body.rb.simulated = false;
		}
	}

	public void MobExit(Mob body)
	{
		//if (body.Inside == this)
		//	body.Inside = null;
		//else
		//	Debug.Log("was in other");
		body.OnDeath -= MobDied;
		Mobs.Remove(body);
	}

	public void MobDied(Mob body)
	{
		Mobs.Remove(body);
		body.OnDeath -= MobDied;

		if (!gameObject.activeSelf)
			gameObject.SetActive(true);
		enabled = true;
		body.rb.simulated = true;
	}
	public void OnPlayerEnter(PlayerBrain PB)
	{
		Players.Add(PB);

		if (Players.Count == 0)
		{
			for (int i = 0; i < Neighbors.Length; i++)
			{
				if (Neighbors[i] != null)
					Neighbors[i].OnNeighborActivated();
			}
		}
		OnNeighborActivated();
	}
	public void OnPlayerExit(PlayerBrain PB)
	{
		Players.Remove(PB);
		if (Players.Count == 0)
		{
			OnNeighborDeActivated();
			for (int i = 0; i < Neighbors.Length; i++)
			{
				if (Neighbors[i] != null)
					Neighbors[i].OnNeighborDeActivated();
			}
		}
		return;
	}
	private void OnTriggerEnter2D(Collider2D col)
	{
		//NetworkBehaviour NB = col.gameObject.GetComponent<NetworkBehaviour>();
		//if (NB != null)
		//{
		//	Objects.Add(NB);
		//}
		Objects.Add(col.transform);
		//PlayerBrain PB = col.gameObject.GetComponent<PlayerBrain>();
		//if (PB != null)
		//{
		//	if (Players.Count == 0)
		//	{
		//		for (int i = 0; i < Neighbors.Length; i++)
		//		{
		//			if(Neighbors[i] != null)
		//				Neighbors[i].OnNeighborActivated();
		//		}
		//		Players.Add(PB);
		//		OnNeighborActivated();
		//	}
		//	else
		//		Players.Add(PB);
		//	return;
		//}
		//Mob body = col.gameObject.GetComponent<Mob>();
		//if (body != null)
		//{
		//	MobEnter(body);
		//}
	}
	private void OnTriggerExit2D(Collider2D col)
	{
		//NetworkBehaviour NB = col.gameObject.GetComponent<NetworkBehaviour>();
		//if (NB != null)
		//{
		//	Objects.Remove(NB);
		//}
		Objects.Remove(col.transform);
		//PlayerBrain PB = col.gameObject.GetComponent<PlayerBrain>();
		//if (PB != null)
		//{
		//	Players.Remove(PB);
		//	if (Players.Count == 0)
		//	{
		//		OnNeighborDeActivated();
		//		for (int i = 0; i < Neighbors.Length; i++)
		//		{
		//			if(Neighbors[i] != null)
		//				Neighbors[i].OnNeighborDeActivated();
		//		}
		//	}
		//	return;
		//}
		//Mob body = col.gameObject.GetComponent<Mob>();
		//if (body != null)
		//{
		//	MobExit(body);
		//}
	}

	public MapGenerator.Chunk[] GetChunkNeighbors() 
	{
		MapGenerator.Chunk[] Chunk = new MapGenerator.Chunk[6];

		for(int i = 0; i < Neighbors.Length; i++) 
		{
			Chunk[i] = Neighbors[i].chunk;
		}

		return Chunk;
	}

	public void FirstEnter()
	{
		loadLevel = 10;
		if (Players.Count == 0)
		{
			//Debug.LogError("No Player");
			//firstload = false;
			//return;
		}
		else if (Vector2.Distance(Players[0].transform.position, transform.position) > 300)
		{
			Debug.LogError("Too far but has Player?");
			loadLevel = 0;
			return;
		}
		UpdateNear();

		RegionManager.MapGen.ResolveChunk(Vector2Int.RoundToInt(transform.position), ref chunk, GetChunkNeighbors(), this);
	}

	private void UpdateNear()
	{
		loadLevel = 3;
		for (int i = 0; i < Neighbors.Length; i++)
		{
			if (Neighbors[i] == null)
			{
				Neighbors[i].GenNear();
			}
			else
				Debug.DrawLine(transform.position, Neighbors[i].transform.position, Color.cyan, 5);
		}
	}
	private void GenNear()
	{
		loadLevel = 2;
		for (int i = 0; i < Neighbors.Length; i++)
		{
			if (Neighbors[i] == null)
			{
				GeneateNew(i);
			}
			else
				Debug.DrawLine(transform.position, Neighbors[i].transform.position, Color.green, 5);
		}
	}

	public void GeneateNew(int slot) 
	{
		if (!TryGet(slot, out Region R))
		{
			Debug.DrawLine(transform.position, transform.position + (Vector3)Offsets[slot], Color.red,5);
			R = Instantiate(RegionManager.Default, transform.position + (Vector3)Offsets[slot], Quaternion.identity, transform.parent);
			RegionManager.GlobalRegions.Add(Vector2Int.RoundToInt((Vector2)transform.position + Offsets[slot]),R);
			Debug.Log("Added: " + Vector2Int.RoundToInt((Vector2)transform.position + Offsets[slot]));
			R.gameObject.SetActive(true);
			R.chunk = RegionManager.MapGen.GetNewChunk(Vector2Int.RoundToInt((Vector2)transform.position + Offsets[slot]));
			R.Active = false;
			R.Objects = new List<Transform>();
			R.loadLevel = 0;
			R.Neighbors = new Region[6];
			R.Players = new List<PlayerBrain>();
			R.Mobs = new List<Mob>();
			R.SR.color = Color.gray;
			SR.color = Color.yellow;
		}
		else
			Debug.DrawLine(transform.position, R.transform.position, Color.yellow, 5);

		Neighbors[slot] = R;
		Neighbors[slot].Neighbors[(slot + 3) % 6] = this;
	}

	public bool TryGet(int slot, out Region R) 
	{
		RegionManager.GlobalRegions.TryGetValue(Vector2Int.RoundToInt((Vector2)transform.position + Offsets[slot]),out R);

		if (R == null)
		{
			return false;
		}
		return R != null;
	}
}
