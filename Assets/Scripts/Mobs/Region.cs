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
	public bool Active;
	public bool firstload;

	public List<Transform> Objects;
	public MapGenerator.Chunk chunk;

	private void Start()
	{
		if (Defualt == null)
			Defualt = OriginDefualt;
		if(!GlobalRegions.ContainsKey(Vector2Int.RoundToInt(transform.position)))
			GlobalRegions.Add(Vector2Int.RoundToInt(transform.position), this);

		chunk = MapGen.GetNewChunk(Vector2Int.RoundToInt(transform.position));
	}

	public void OnNeighborActivated()
	{
		Active = true;
		SR.color = new Color(0,.5f,0);
		if (!firstload)
		{
			firstload = true;
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
		Active = false;
		SR.color = new Color(.5f, 0, 0);
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
			firstload = false;
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

		MapGen.ResolveChunk(Vector2Int.RoundToInt(transform.position),ref chunk, GetChunkNeighbors());
	}

	public void GeneateNew(int slot) 
	{
		if (!TryGet(slot, out Region R))
		{
			Debug.DrawLine(transform.position, transform.position + (Vector3)Offsets[slot], Color.red,5);
			R = Instantiate(Defualt, transform.position + (Vector3)Offsets[slot], Quaternion.identity, transform.parent);
			GlobalRegions.Add(Vector2Int.RoundToInt((Vector2)transform.position + Offsets[slot]),R);
			R.Active = false;
			R.firstload = false;
			R.Neighbors = new Region[6];
			R.Players = new List<PlayerBrain>();
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
