using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryGrid : MonoBehaviour
{
	public Vector2Int Size;
	public int[,] Slots;
	public List<Vector2Int> ItemPos;
	public List<InventoryItem> Items;
	public InventoryItem Fab;

	public bool go;
	public bool Grab;
	public bool r;
	public int id;
	public int idTest;
	public Vector2Int Test;
	public Vector2Int Pos;
	public Vector2Int bpos;
	public Vector2Int rsize;
	public Camera Cam;

	private void Start()
	{
		Slots = new int[Size.x, Size.y];
		for (int i = 0; i < Size.x; i++)
			for (int j = 0; j < Size.y; j++)
				Slots[i, j] = -1;
		Items = new List<InventoryItem>();
		ItemPos = new List<Vector2Int>();
		for (int i = 0; i < 50; i++)
		{
			InventoryItem I = Instantiate(Fab, transform);
			I.Size = new Vector2Int(Random.Range(1, rsize.x), Random.Range(1, rsize.y));
			SpriteRenderer G = I.GetComponent<SpriteRenderer>();
			G.color = Random.ColorHSV();
			G.size = I.Size;
			Items.Add(I);
			ItemPos.Add(-Vector2Int.one);
			for (int j = 0; j < 10; j++)
			{
				if (Place(Items.Count - 1, new Vector2Int(Random.Range(0, Size.x - I.Size.x), Random.Range(0, Size.y - I.Size.y))))
				{
					goto End;
				}
			}
			Items.RemoveAt(Items.Count - 1);
			ItemPos.RemoveAt(ItemPos.Count - 1);
			Destroy(I.gameObject);

		End:;
		}
	}
	private void Update()
	{
		if (go)
		{
			go = false;
			r = Move(id, Pos);
		}
		Vector2 mpos = Cam.ScreenToWorldPoint(Input.mousePosition);
		if (Input.GetMouseButtonDown(0))
		{
			Vector2Int pos = Vector2Int.FloorToInt(transform.InverseTransformPoint(mpos));
			Test = pos;
			if (BoundsCheck(pos, Vector2Int.one))
			{
				id = Slots[pos.x, pos.y];
				idTest = id;
				if (id != -1)
				{
					Grab = true;
					bpos = Vector2Int.FloorToInt(Items[id].transform.localPosition) - pos;
				}
			}
		}

		if (Grab)
		{
			Items[id].transform.position = (Vector2)Vector2Int.FloorToInt(mpos) + bpos;

			if (Input.GetMouseButtonUp(0))
			{
				Grab = false;
				if (!Move(id, bpos + Vector2Int.FloorToInt(transform.InverseTransformPoint(mpos))))
					Items[id].transform.localPosition = (Vector2)ItemPos[id];
			}
		}
	}
	public bool Move(InventoryItem Item, Vector2Int pos)
	{
		int id = Items.IndexOf(Item);
		if (id == -1)
			return false;
		return Move(id, pos);
	}
	public bool Move(int id, Vector2Int pos)
	{
		if (!Remove(id))
			return false;
		if (Place(id, pos))
			return true;
		Place(id, ItemPos[id]);
		return false;
	}
	public bool Place(InventoryItem Item, int id, Vector2Int pos)
	{
		if (!BoundsCheck(pos, Item.Size))
			return false;
		for (int i = 0; i < Item.Size.x; i++)
			for (int j = 0; j < Item.Size.y; j++)
				if (Slots[pos.x + i, pos.y + j] != -1)
					return false;
		ItemPos[id] = pos;
		Item.transform.localPosition = (Vector2)pos;
		for (int i = 0; i < Item.Size.x; i++)
			for (int j = 0; j < Item.Size.y; j++)
				Slots[pos.x + i, pos.y + j] = id;
		return true;
	}
	public bool Place(InventoryItem Item, Vector2Int pos)
	{
		return Place(Item, Items.IndexOf(Item), pos);
	}
	public bool Place(int id, Vector2Int pos)
	{
		return Place(Items[id], id, pos);
	}
	public bool Remove(InventoryItem Item, int id)
	{
		Vector2Int pos = ItemPos[id];

		for (int i = 0; i < Item.Size.x; i++)
			for (int j = 0; j < Item.Size.y; j++)
				if (Slots[pos.x + i, pos.y + j] != id && Slots[pos.x + i, pos.y + j] != -1)
					return false;

		for (int i = 0; i < Item.Size.x; i++)
			for (int j = 0; j < Item.Size.y; j++)
				Slots[pos.x + i, pos.y + j] = -1;
		return true;
	}
	public bool Remove(int id)
	{
		return Remove(Items[id], id);
	}
	public bool Remove(InventoryItem Item)
	{
		return Remove(Item, Items.IndexOf(Item));
	}
	public bool BoundsCheck(Vector2Int pos, Vector2Int size)
	{
		return pos.x >= 0 && pos.y >= 0 && pos.x <= Size.x - size.x && pos.y <= Size.y - size.y;
	}
}
