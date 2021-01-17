using Mirror;
using UnityEngine;

public class Gun : Equipment
{
	public float ReloadTime;
	public float FireTime;
	public float waitTime;

	public int Clip;
	public int clip;

	//public float Impulse;
	//public float Life;
	//public float Speed;

	public Projectile Projectile;
	public ProjectileData ProjectileData;

	public Mob Holder;
	public override void Drop()
	{
		netIdentity.RemoveClientAuthority();
		transform.parent = null;
		Holder = null;
		RpcDrop();
	}
	[ClientRpc]
	public void RpcDrop()
	{
		transform.parent = null;
		Holder = null;
	}

	public override void Pickup(Mob M)
	{
		transform.parent = M.transform;
		transform.localPosition = Vector3.zero;
		Holder = M;

		netIdentity.AssignClientAuthority(M.B.netIdentity.connectionToClient);
		if (isServerOnly)
			M.Equipment.Add(this);

		RpcPickup(M.B.netId);
	}

	[ClientRpc]
	public void RpcPickup(uint MobId)
	{
		Mob M = NetworkIdentity.spawned[MobId].GetComponent<Brain>().Body;
		transform.parent = M.transform;
		transform.localPosition = Vector3.zero;
		Holder = M;
		M.Equipment.Add(this);
	}

	public override void Use(Vector2 Pos)
	{
		if (waitTime < 0)
		{
			if (clip <= 0)
			{
				waitTime = ReloadTime;
				clip = Clip;
			}
			else
			{
				waitTime = FireTime;
				clip--;
				Fire(Pos);
			}
		}
	}

	void Fire(Vector2 Pos)
	{
		Projectile P = Instantiate(Projectile);
		P.Shoot(Holder, Pos - (Vector2)transform.position, ProjectileData);
	}

	private void Update()
	{
		waitTime -= Time.deltaTime;
	}

	public override void Randomize()
	{
		FireTime = Random.Range(.005f, 1);
		ReloadTime = Random.Range(FireTime*2, 5);

		Clip = (int)(Random.Range(.5f, 5) / FireTime);
		if (Clip < 1)
			Clip = 1;

		ProjectileData.Impulse = Random.Range(10, 1000f) * FireTime;
		ProjectileData.LifeTime = (Random.Range(1, 100f) * 100) / ProjectileData.Impulse;
		ProjectileData.Speed = Random.Range(0, 5f) * ReloadTime;
		ProjectileData.Dammage = (Random.Range(5, 30f)) / Clip * ReloadTime;

		SetStats(new GunStats(this));
	}

	[ClientRpc]
	public void SetStats(GunStats stats)
	{
		stats.Set(this);
	}

	public override string PrintStats()
	{
		return
			$"Damage {ProjectileData.Dammage}\n" +
			$"Fire Rate {FireTime}\n" +
			$"Reload {ReloadTime}\n" +
			$"Impulse {ProjectileData.Impulse}\n" +
			$"Clip {Clip}";
	}

	public struct GunStats
	{
		public float ReloadTime;
		public float FireTime;

		public int Clip;

		public float Impulse;
		public float LifeTime;
		public float Speed;
		public float Dammage;

		public GunStats(Gun Base)
		{
			ReloadTime = Base.ReloadTime;
			FireTime = Base.FireTime;

			Clip = Base.Clip;

			Impulse = Base.ProjectileData.Impulse;
			LifeTime = Base.ProjectileData.LifeTime;
			Speed = Base.ProjectileData.Speed;
			Dammage = Base.ProjectileData.Dammage;
		}

		public void Set(Gun Base)
		{
			Base.ReloadTime = ReloadTime;
			Base.FireTime = FireTime;

			Base.Clip = Clip;

			Base.ProjectileData.Impulse = Impulse;
			Base.ProjectileData.LifeTime = LifeTime;
			Base.ProjectileData.Speed = Speed;
			Base.ProjectileData.Dammage = Dammage;
		}
	}
}