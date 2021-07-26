using System.Collections.Generic;
using UnityEngine;

public class RegionManager : MonoBehaviour
{
	public Region Default;
	public Dictionary<Vector2Int, Region> GlobalRegions = new Dictionary<Vector2Int, Region>();
	public MapGenerator MapGen;

	private void Start()
	{
		Region C = Instantiate(Default, new Vector3(0,0,5), Quaternion.identity, transform);
		GlobalRegions.Add(Vector2Int.zero, C);
		C.chunk = MapGen.GetNewChunk(Vector2Int.zero);

		for (int i = 0; i < C.Neighbors.Length; i++)
		{
			if (C.Neighbors[i] == null)
			{
				C.GeneateNew(i);

				for (int j = 0; j < C.Neighbors.Length; j++)
				{
					if (C.Neighbors[i].Neighbors[j] == null)
					{
						C.Neighbors[i].GeneateNew(j);
					}
					else
						Debug.DrawLine(transform.position, C.Neighbors[i].Neighbors[j].transform.position, Color.green, 5);
				}

				MapGen.ResolveChunk(Vector2Int.RoundToInt(transform.position), ref C.chunk, C.Neighbors[i].GetChunkNeighbors(), C.Neighbors[i]);
			}
			else
				Debug.DrawLine(transform.position, C.Neighbors[i].transform.position, Color.green, 5);
		}

		MapGen.ResolveChunk(Vector2Int.RoundToInt(transform.position), ref C.chunk, C.GetChunkNeighbors(), C);
	}
}
