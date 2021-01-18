using Mirror;
using UnityEngine;

public class ShotGun : Gun 
{
	public int Shots;
	public float Spread;
	public float ForceSpread;

	protected override void Fire(Vector2 Pos)
	{
		for (int i = 0; i < Shots; i++)
		{
			Projectile P = Instantiate(Projectile);
			ProjectileData D = ProjectileData.Clone();
			D.Impulse += Random.Range(-Mathf.Min(D.Impulse/2,ForceSpread), ForceSpread);
			P.Shoot(Holder, (Pos + Random.insideUnitCircle * Spread) - (Vector2)transform.position, D);
		}
	}

	public override void Randomize()
	{
		FireTime = Random.Range(Random.Range(.1f, .5f), Random.Range(.5f, 2f));
		ReloadTime = Random.Range(FireTime * 1.5f, FireTime * 2);

		Shots = Random.Range(2, 15);
		Spread = Random.Range(.5f, Random.Range(1f, 3)) * (Shots / 5);
		Clip = (int)(Random.Range(.5f, 5) / FireTime);
		if (Clip < 1)
			Clip = 1;

		ForceSpread = Random.Range(5, 30f) * Random.Range(5, 30f);
		ProjectileData.Impulse = Random.Range(5, 30f) * Random.Range(5, 30f) * FireTime;
		ProjectileData.LifeTime = (Random.Range(1, 10f) * 100) / ProjectileData.Impulse;
		ProjectileData.Speed = Random.Range(0, 5f) * ReloadTime;
		ProjectileData.Dammage = (Random.Range(5, 50f)) / Clip / Shots * ReloadTime;
		ProjectileData.Health = Random.Range(1, 100f);

		SetStats(new ShotGunStats(this));
	}

	[ClientRpc]
	public void SetStats(ShotGunStats stats)
	{
		stats.Set(this);
	}

	public override string PrintStats()
	{
		return
			$"Damage {ProjectileData.Dammage:f}\n" +
			$"Fire Rate {FireTime:f} Shots {Shots}\n" +
			$"Spread{Spread:f} | {ForceSpread:f}\n" +
			$"Reload {ReloadTime:f} | {Mathf.Max(waitTime,0):f}\n" +
			$"Speed {ProjectileData.Impulse:f} | {ProjectileData.Speed:f}\n" +
			$"Clip {clip} / {Clip}";
	}

	public struct ShotGunStats
	{
		public float ReloadTime;
		public float FireTime;

		public int Clip;
		public int Shots;
		public float Spread;
		public float ForceSpread;

		public float Impulse;
		public float LifeTime;
		public float Speed;
		public float Dammage;
		public float Health;

		public ShotGunStats(ShotGun Base)
		{
			ReloadTime = Base.ReloadTime;
			FireTime = Base.FireTime;

			Clip = Base.Clip;
			Shots = Base.Shots;
			Spread = Base.Spread;
			ForceSpread = Base.ForceSpread;

			Impulse = Base.ProjectileData.Impulse;
			LifeTime = Base.ProjectileData.LifeTime;
			Speed = Base.ProjectileData.Speed;
			Dammage = Base.ProjectileData.Dammage;
			Health = Base.ProjectileData.Health;
		}

		public void Set(ShotGun Base)
		{
			Base.ReloadTime = ReloadTime;
			Base.FireTime = FireTime;

			Base.Clip = Clip;
			Base.Shots = Shots;
			Base.Spread = Spread;
			Base.ForceSpread = ForceSpread;

			Base.ProjectileData.Impulse = Impulse;
			Base.ProjectileData.LifeTime = LifeTime;
			Base.ProjectileData.Speed = Speed;
			Base.ProjectileData.Dammage = Dammage;
			Base.ProjectileData.Health = Health;
		}
	}
}