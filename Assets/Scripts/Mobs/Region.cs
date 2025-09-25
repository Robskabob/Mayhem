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

	public static HexRegionManager HexRegionManager;
	//public static Dictionary<Vector2Int, Region> GlobalRegions = new Dictionary<Vector2Int, Region>();
	//public MapGenerator MapGen;

	//public Region OriginDefualt;
	//public static Region Defualt;

	public SpriteRenderer SR;
	public Region[] Neighbors = new Region[6];
	public List<PlayerBrain> Players;
	public List<Mob> Mobs;
	public bool Active;
	public LoadLevel loadLevel;

	public enum LoadLevel
	{
		MinLoad = 0,
		Preloaded = 1,
		loaded = 2,
		Entered = 3,		
		UnLoaded = 4,
		PlayerLeft = 5,
	}

	public List<Transform> Objects;
	public Transform Contents;
	public MapGenerator.Chunk chunk;

	private void Start()
	{
		if(!HexRegionManager.GlobalRegions.ContainsKey(RegionManager.GetChunkPos(transform.position)))
        {
            HexRegionManager.GlobalRegions.Add(RegionManager.GetChunkPos(transform.position), this);
            Debug.LogError($"at:{transform.position},{RegionManager.GetChunkPos(transform.position)} | Chunk Created");
        }
        //     else
        //     {
        //Debug.LogError($"at:{transform.position},{RegionManager.GetChunkPos(transform.position)} | Chunk Created in already occupied chunk");
        //gameObject.SetActive(false);
        //     }
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
		if(loadLevel < LoadLevel.loaded)
        {
			Debug.LogError("UnloadUnGened");
			return;
        }

		if (loadLevel == LoadLevel.loaded)
			loadLevel = LoadLevel.UnLoaded;
		else if (loadLevel == LoadLevel.Entered)
			loadLevel = LoadLevel.PlayerLeft;
		//else
		//	Debug.Log("Wut " + loadLevel);

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
		if (loadLevel < LoadLevel.loaded)
		{
			ResolveNeighbors();
		}
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

	public void MobDied(Mob body, DamageSource damageSource)
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
		if (loadLevel < LoadLevel.loaded) 
		{
			Debug.Log("Resort");
			ResolveNeighbors();
		}
		loadLevel = LoadLevel.Entered;
		OnNeighborActivated();
		for (int i = 0; i < Neighbors.Length; i++)
		{
			if (Neighbors[i] != null)
				Neighbors[i].OnNeighborActivated();
		}
	}
	public void OnPlayerExit(PlayerBrain PB)
	{
		if (loadLevel < LoadLevel.Entered)
		{
			Debug.LogError("LeftNotEnterd");
			ResolveNeighbors();
		}
		loadLevel = LoadLevel.PlayerLeft;

		Players.Remove(PB);
		if (Players.Count == 0)
		{
			OnNeighborDeActivated();
			for (int i = 0; i < Neighbors.Length; i++)
			{
				if (Neighbors[i] != null || Neighbors[i].loadLevel < LoadLevel.loaded)
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

	public void LoadNeighbors()
	{
		for (int i = 0; i < Neighbors.Length; i++)
		{
			if (Neighbors[i].loadLevel < LoadLevel.loaded)
			{
				Neighbors[i].ResolveNeighbors();
			}
			else
				Debug.DrawLine(transform.position, Neighbors[i].transform.position, Color.green, 5);
		}
	}
	public void ResolveNeighbors()
	{
		Debug.Log(transform.position);
		for (int i = 0; i < Neighbors.Length; i++)
		{
			if (Neighbors[i].loadLevel < LoadLevel.Preloaded)
			{
				Neighbors[i].GenNeighbors();
			}
			else
				Debug.DrawLine(transform.position, Neighbors[i].transform.position, Color.green, 5);
		}
		HexRegionManager.MapGen.ResolveChunk(Vector2Int.RoundToInt(transform.position),ref chunk, GetChunkNeighbors(),this);
		loadLevel = LoadLevel.loaded;
	}
	public void GenNeighbors() 
	{

		//if (Players.Count == 0)
		//{
		//	Debug.LogError("No Player");
		//	//firstload = false;
		//	//return;
		//}
		//else
		//if(Vector2.Distance(Players[0].transform.position,transform.position) > 300)
		//{
		//	Debug.LogError("Too far but has Player? Player Check removed");
		//	loadLevel = 0;
		//	return;
		//}
		if (loadLevel >= LoadLevel.Preloaded)
		{
			Debug.LogError("Already PreLoaded");
		}
		for (int i = 0; i < Neighbors.Length; i++)
		{
			if (Neighbors[i] == null)
			{
				GeneateNew(i);
			}
			else
			Debug.DrawLine(transform.position, Neighbors[i].transform.position,Color.green,5);
		}
		//this needs more then just here
	}

	public void GeneateNew(int slot) 
	{
		if (!TryGet(slot, out Region R))
		{
			Debug.DrawLine(transform.position, transform.position + (Vector3)Offsets[slot], Color.red,5);
			R = Instantiate(HexRegionManager.Default, transform.position + (Vector3)Offsets[slot], Quaternion.identity, transform.parent);
			HexRegionManager.GlobalRegions.Add(RegionManager.GetChunkPos((Vector2)transform.position + Offsets[slot]),R);
			R.gameObject.SetActive(true);
			R.chunk = HexRegionManager.MapGen.GetNewChunk(Vector2Int.RoundToInt((Vector2)transform.position + Offsets[slot]));
			R.Active = false;
			R.Objects = new List<Transform>();
			R.loadLevel = LoadLevel.MinLoad;
			R.Neighbors = new Region[6];
			R.Players = new List<PlayerBrain>();
			R.Mobs = new List<Mob>();
			R.SR.color = Color.gray;
			SR.color = Color.yellow;
			loadLevel = LoadLevel.Preloaded;
		}
		else
			Debug.DrawLine(transform.position, R.transform.position, Color.yellow, 5);

		Neighbors[slot] = R;
		Neighbors[slot].Neighbors[(slot + 3) % 6] = this;
	}

	public bool TryGet(int slot, out Region R) 
	{
		HexRegionManager.GlobalRegions.TryGetValue(RegionManager.GetChunkPos((Vector2)transform.position + Offsets[slot]),out R);

		if (R == null)
		{
			return false;
		}
		return R != null;
	}
}
