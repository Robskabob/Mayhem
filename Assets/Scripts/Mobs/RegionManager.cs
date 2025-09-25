using UnityEngine;

public abstract class RegionManager : MonoBehaviour
{
	public static RegionManager ActiveRegionManager;

	public static Vector2Int GetChunkPos(Vector2 pos)
	{
		return ActiveRegionManager.AbstractGetChunkPos(pos);
	}

	public abstract Vector2Int AbstractGetChunkPos(Vector2 pos);
}
