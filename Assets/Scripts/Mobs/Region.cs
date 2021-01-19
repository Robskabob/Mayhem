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
	public Region Defualt;

	public SpriteRenderer SR;
	public Region[] Neighbors = new Region[6];
	public List<PlayerBrain> Players;
	public bool Active;
	public bool firstload;

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
			if (Neighbors[i].Players.Count != 0)
			{
				return;
			}
		}
		Active = false;
		SR.color = new Color(.5f, 0, 0);
	}

	private void OnTriggerEnter2D(Collider2D col)
	{
		PlayerBrain PB = col.gameObject.GetComponent<PlayerBrain>();
		if (PB != null)
		{
			if (Players.Count == 0)
			{
				OnNeighborActivated();
				for (int i = 0; i < Neighbors.Length; i++)
				{
					Neighbors[i].OnNeighborActivated();
				}
			}
			Players.Add(PB);
		}
	}
	private void OnTriggerExit2D(Collider2D col)
	{
		PlayerBrain PB = col.gameObject.GetComponent<PlayerBrain>();
		if (PB != null)
		{
			Players.Remove(PB);
			if (Players.Count == 0)
			{
				OnNeighborDeActivated();
				for (int i = 0; i < Neighbors.Length; i++)
				{
					Neighbors[i].OnNeighborDeActivated();
				}
			}
		}
	}

	public void FirstEnter() 
	{
		for (int i = 0; i < Neighbors.Length; i++)
		{
			if (Neighbors[i] == null)
				GeneateNew(i);
		}
	}

	public void GeneateNew(int slot) 
	{
		if(!TryGet(slot, out Region R))
			R = Instantiate(Defualt,transform.position + (Vector3)Offsets[slot],Quaternion.identity,transform.parent);

		Neighbors[slot] = R;
		Neighbors[slot].Neighbors[(slot + 3) % 6] = this;
		if (slot % 2 == 0)
		{
			Neighbors[slot].Neighbors[(slot + 1) % 6] = Neighbors[(slot + 1) % 6];
			Neighbors[slot].Neighbors[(slot - 1) % 6] = Neighbors[(slot - 1) % 6];
		}
		else
		{
			Neighbors[slot].Neighbors[(slot + 1) % 6] = Neighbors[(slot - 1) % 6];
			Neighbors[slot].Neighbors[(slot - 1) % 6] = Neighbors[(slot + 1) % 6];
		}
	}

	public bool TryGet(int slot, out Region R) 
	{
		Collider2D col = Physics2D.OverlapPoint(transform.position + (Vector3)Offsets[slot],128);
		R = col.GetComponent<Region>();
		return R != null;
	}
}