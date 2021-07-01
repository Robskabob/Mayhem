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

	public static Dictionary<Vector2Int, Region> GlobalRegions = new Dictionary<Vector2Int, Region>();
	public MapGenerator MapGen;

	public Region OriginDefualt;
	public static Region Defualt;

	public SpriteRenderer SR;
	public Region[] Neighbors = new Region[6];
	public List<PlayerBrain> Players;
	public List<PlatBrain> Mobs;
	public bool Active;
	public int loadLevel;

	public List<Transform> Objects;
	public Transform Contents;
	public MapGenerator.Chunk chunk;

	private void Start()
	{
		chunk = MapGen.GetNewChunk(Vector2Int.RoundToInt(transform.position));

		if(!GlobalRegions.ContainsKey(Vector2Int.RoundToInt(transform.position)))
			GlobalRegions.Add(Vector2Int.RoundToInt(transform.position), this);
		
		if (Defualt == null)
		{
			Defualt = OriginDefualt;
			for (int i = 0; i < Neighbors.Length; i++)
			{
				if (Neighbors[i] == null)
				{
					GeneateNew(i);

					for (int j = 0; j < Neighbors.Length; j++)
					{
						if (Neighbors[i].Neighbors[j] == null)
						{
							Neighbors[i].GeneateNew(j);
						}
						else
							Debug.DrawLine(transform.position, Neighbors[i].Neighbors[j].transform.position, Color.green, 5);
					}

					Neighbors[i].MapGen.ResolveChunk(Vector2Int.RoundToInt(transform.position), ref chunk, Neighbors[i].GetChunkNeighbors(),Neighbors[i]);
				}
				else
					Debug.DrawLine(transform.position, Neighbors[i].transform.position, Color.green, 5);
			}

			MapGen.ResolveChunk(Vector2Int.RoundToInt(transform.position), ref chunk, GetChunkNeighbors(),this);
		}
	}

	public void OnActivate() 
	{
		Active = true;
		SR.color = new Color(0, .5f, 0);
		Contents.gameObject.SetActive(true);

		for (int i = 0; i < Mobs.Count; i++) 
		{
			Mobs[i].enabled = true;
			Mobs[i].Body.rb.simulated = true;
		}
	}
	public void OnDeActivate()
	{
		Active = false;
		SR.color = new Color(.5f, 0, 0);
		Contents.gameObject.SetActive(false);

		for (int i = 0; i < Mobs.Count; i++)
		{
			Mobs[i].Body.rb.simulated = false;
			Mobs[i].enabled = false;
		}
	}
	public void OnNeighborActivated()
	{
		OnActivate();
		if (loadLevel < 1)
		{
			loadLevel ++;
			FirstEnter();
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

	public void MobEnter(PlatBrain PlB)
	{
		if (PlB.Inside != null)
			Debug.Log("double in");
		PlB.Inside = this;
		PlB.OnDeath += MobDied;
		Mobs.Add(PlB);
		if (!Active)
		{
			PlB.enabled = false;
			PlB.Body.rb.simulated = false;
		}
	}

	public void MobExit(PlatBrain PlB)
	{
		if (PlB.Inside == this)
			PlB.Inside = null;
		else
			Debug.Log("was in other");
		PlB.OnDeath -= MobDied;
		Mobs.Remove(PlB);
	}

	public void MobDied(PlatBrain PlB)
	{
		Mobs.Remove(PlB);
		PlB.OnDeath -= MobDied;
	}

	private void OnTriggerEnter2D(Collider2D col)
	{
		//NetworkBehaviour NB = col.gameObject.GetComponent<NetworkBehaviour>();
		//if (NB != null)
		//{
		//	Objects.Add(NB);
		//}
		Objects.Add(col.transform);
		PlayerBrain PB = col.gameObject.GetComponent<PlayerBrain>();
		if (PB != null)
		{
			if (Players.Count == 0)
			{
				for (int i = 0; i < Neighbors.Length; i++)
				{
					if(Neighbors[i] != null)
						Neighbors[i].OnNeighborActivated();
				}
				Players.Add(PB);
				OnNeighborActivated();
			}
			else
				Players.Add(PB);
			return;
		}
		PlatBrain PlB = col.gameObject.GetComponent<PlatBrain>();
		if (PlB != null)
		{
			MobEnter(PlB);
		}
	}
	private void OnTriggerExit2D(Collider2D col)
	{
		//NetworkBehaviour NB = col.gameObject.GetComponent<NetworkBehaviour>();
		//if (NB != null)
		//{
		//	Objects.Remove(NB);
		//}
		Objects.Remove(col.transform);
		PlayerBrain PB = col.gameObject.GetComponent<PlayerBrain>();
		if (PB != null)
		{
			Players.Remove(PB);
			if (Players.Count == 0)
			{
				OnNeighborDeActivated();
				for (int i = 0; i < Neighbors.Length; i++)
				{
					if(Neighbors[i] != null)
						Neighbors[i].OnNeighborDeActivated();
				}
			}
			return;
		}
		PlatBrain PlB = col.gameObject.GetComponent<PlatBrain>();
		if (PlB != null)
		{
			MobExit(PlB);
		}
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

		if (Players.Count == 0)
		{
			//Debug.LogError("No Player");
			//firstload = false;
			//return;
		}
		else if(Vector2.Distance(Players[0].transform.position,transform.position) > 300)
		{
			Debug.LogError("Too far but has Player?");
			loadLevel = 0;
			return;
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

		MapGen.ResolveChunk(Vector2Int.RoundToInt(transform.position),ref chunk, GetChunkNeighbors(),this);
	}

	public void GeneateNew(int slot) 
	{
		if (!TryGet(slot, out Region R))
		{
			Debug.DrawLine(transform.position, transform.position + (Vector3)Offsets[slot], Color.red,5);
			R = Instantiate(Defualt, transform.position + (Vector3)Offsets[slot], Quaternion.identity, transform.parent);
			GlobalRegions.Add(Vector2Int.RoundToInt((Vector2)transform.position + Offsets[slot]),R);
			R.Active = false;
			R.Objects = new List<Transform>();
			R.loadLevel = 0;
			R.Neighbors = new Region[6];
			R.Players = new List<PlayerBrain>();
			R.Mobs = new List<PlatBrain>();
			R.SR.color = Color.gray;
			SR.color = Color.yellow;
		}
		else
			Debug.DrawLine(transform.position, R.transform.position, Color.yellow, 5);

		Neighbors[slot] = R;
		Neighbors[slot].Neighbors[(slot + 3) % 6] = this;
		//Doesn't Work \/\/
		//if (slot % 2 == 0)
		//{
		//	//Debug.Log(((slot + 1) % 6)+" | "+((slot + 5) % 6));
		//	R.Neighbors[(slot + 1) % 6] = Neighbors[(slot + 5) % 6];
		//	R.Neighbors[(slot + 5) % 6] = Neighbors[(slot + 1) % 6];
		//}
		//else
		//{
		//	R.Neighbors[(slot + 1) % 6] = Neighbors[(slot + 1) % 6];
		//	R.Neighbors[(slot + 5) % 6] = Neighbors[(slot + 5) % 6];
		//}
	}

	public bool TryGet(int slot, out Region R) 
	{
		GlobalRegions.TryGetValue(Vector2Int.RoundToInt((Vector2)transform.position + Offsets[slot]),out R);

		if (R == null)
		{
			return false;
		}
		return R != null;
	}
}
