using UnityEngine;

public class SingleRegionManager : RegionManager
{
    public override Vector2Int AbstractGetChunkPos(Vector2 pos)
    {
		return Vector2Int.zero;
    }

    private void Start()
    {
        ActiveRegionManager = this;
    }
}
