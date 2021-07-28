using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
	public Transform Platform;

	public int Seed;
	public float Scale;
	public float Density;
	public class Chunk 
	{
		public int Level;
		//L 1
		public Vector2[] PlatformCandidates;
		//public List<Vector2> StructureCandidates;
		//L 2
		public bool[] PlatformValid;
		public List<Vector2> PlatformPos;
		public List<Vector2> PlatformScale;

		//public List<Vector2> StructurePos;
		//public List<int> ID;
	}

	public int GetSeed(Vector2Int Pos) 
	{
		return (Seed ^ Pos.x) * (Pos.y);
	}

	public Chunk GetNewChunk(Vector2Int Pos)
	{
		Chunk C = new Chunk();

		Random.InitState(GetSeed(Pos));

		Mathf.PerlinNoise(Pos.x / Scale, Pos.y / Scale);

		C.PlatformCandidates = new Vector2[Random.Range(10, (int)Density)];

		for(int i = 0; i < C.PlatformCandidates.Length; i++) 
		{
			C.PlatformCandidates[i] = new Vector2(Random.Range(-50, 50f), Random.Range(-50, 50f)) + (Vector2)Pos;
		}


		return C;
	}
	public void ResolveChunk(Vector2Int Pos,ref Chunk Base, Chunk[] Neighbors, Region region)
	{
		Random.InitState(GetSeed(Pos));

		Base.PlatformValid = new bool[Base.PlatformCandidates.Length];

		for (int i = 0; i < Base.PlatformCandidates.Length; i++)
		{
			//Debug.Log(Base.PlatformCandidates[i]);
			Vector2 plat = Base.PlatformCandidates[i];
			float distx = 99;
			float disty = 99;


			for (int j = i; j < Base.PlatformCandidates.Length; j++)
			{
				Vector2 diff = Base.PlatformCandidates[i] - plat;

				if (Mathf.Abs(diff.x) < distx)
					distx = Mathf.Abs(diff.x);

				if (Mathf.Abs(diff.y) < disty)
					disty = Mathf.Abs(diff.y);

				if(distx < (100 / Density) || disty < (100 / Density))
				{

					continue;
				}
			}

			for (int k = 0; k < Neighbors.Length; k++)
			{
				for (int j = i; j < Base.PlatformCandidates.Length; j++)
				{
					Vector2 diff = Base.PlatformCandidates[i] - plat;

					if (Mathf.Abs(diff.x) < distx)
						distx = Mathf.Abs(diff.x);

					if (Mathf.Abs(diff.y) < disty)
						disty = Mathf.Abs(diff.y);

					if (distx < (1 / Density) || disty < (1 / Density))
					{
						continue;
					}
				}
			}

			Transform T = Instantiate(Platform,Base.PlatformCandidates[i],Quaternion.identity,region.Contents.transform);
			T.localScale = Random.insideUnitCircle * 5;
			if(T.localScale.magnitude < .5f)
				T.localScale *= 1 / T.localScale.magnitude;
		}
	}

	
	public void GenerateChunk(Chunk Base, Region region) { }
}