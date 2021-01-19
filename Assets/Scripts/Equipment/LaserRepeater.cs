using Mirror;
using UnityEngine;

public class LaserRepeater : LaserBase
{
	public float BurnTime;
	public int Clip;
	public int clip;


	public override void Use(Vector2 Pos)
	{
		if (waitTime < 0 && !Shoot)
		{
			if (clip <= 0)
			{
				waitTime = ReloadTime;
				clip = Clip;
			}
			else
			{
				Shoot = true;
				clip--;
			}
		}
	}

	protected override void Update()
	{
		if (waitTime >= 0)
			waitTime -= Time.deltaTime;
		//
		if (Shoot)
		{
			Fire(Holder.B.GetLook());
			waitTime -= Time.deltaTime;
			if (-waitTime > BurnTime)
			{
				waitTime = FireTime;
				Shoot = false;
				LaserVis.enabled = false;
			}
		}
		//LaserVis
	}
	public override void Randomize()
	{
		FireTime = Random.Range(.05f, 1) * Random.Range(.5f, 2f);
		BurnTime = Random.Range(.05f, 1) * Random.Range(.5f, 2f);
		ReloadTime = Random.Range(.1f, Random.Range(2, 5f));

		Damage = Random.Range(5, Random.Range(10, 100f)) * ReloadTime;

		Clip = (int)(Random.Range(5, 15f) / FireTime / BurnTime);
		if (Clip < 1)
			Clip = 1;

		MaxDistance = Random.Range(5, 50f);

		SetStats(new RepeaterStats(this));
	}

	[ClientRpc]
	public void SetStats(RepeaterStats stats)
	{
		stats.Set(this);
	}

	public override string PrintStats()
	{
		return
			$"Damage/sec {Damage:f}\n" +
			$"Fire Rate {FireTime:f}\n" +
			$"Reload {Mathf.Max(waitTime, 0):f} / {ReloadTime:f}\n" +
			$"Burn {Mathf.Max(-waitTime, 0):f} / {BurnTime:f}\n" +
			$"Dist {MaxDistance}\n" +
			$"Clip {clip} / {Clip}";
	}

	public struct RepeaterStats
	{
		public float ReloadTime;
		public float FireTime;
		public float MaxDistance;
		public float Damage;

		public int Clip;

		public RepeaterStats(LaserRepeater Base)
		{
			ReloadTime = Base.ReloadTime;
			Damage = Base.Damage;
			MaxDistance = Base.MaxDistance;
			FireTime = Base.FireTime;
			Clip = Base.Clip;
		}

		public void Set(LaserRepeater Base)
		{
			Base.ReloadTime = ReloadTime;
			Base.Damage = Damage;
			Base.MaxDistance = MaxDistance;
			Base.FireTime = FireTime;
			Base.Clip = Clip;
			Base.clip = Clip;
		}
	}
}
