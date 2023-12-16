using Mirror;
using UnityEngine;

namespace L33t.Equipment
{
	public class LaserBeamer : LaserBase
	{
		public float MaxHeat;
		public override void Use(Vector2 Pos)
		{
			if (waitTime < 0)
			{
				Fire(Pos);
				waitTime -= Time.deltaTime;
				if (-waitTime > MaxHeat)
				{
					waitTime = ReloadTime;
				}
			}
		}

		public override void Randomize()
		{
			//FireTime = Random.Range(.05f, 1) * Random.Range(.5f, 2f);
			ReloadTime = Random.Range(.1f, Random.Range(2, 5f));

			MaxHeat = Random.Range(.2f, 5f);
			Damage = Random.Range(5, Random.Range(10, 100f)) * ReloadTime / MaxHeat;

			MaxDistance = Random.Range(5, 50f);

			SetStats(new BeamerStats(this));
		}

		[ClientRpc]
		public void SetStats(BeamerStats stats)
		{
			stats.Set(this);
		}

		public override string PrintStats()
		{
			return
				$"Damage/sec {Damage:f}\n" +
				$"Fire Rate {FireTime:f}\n" +
				$"Reload {Mathf.Max(waitTime, 0):f} / {ReloadTime:f}\n" +
				$"Heat {Mathf.Max(-waitTime, 0):f} / {MaxHeat:f}\n" +
				$"Dist {MaxDistance}";
		}
		public override UI.StatMenu.data[] GetStats()
		{
			return new UI.StatMenu.data[]
			{
				 new UI.StatMenu.data("Damage"   ,Color.red,0,100, Damage)
				,new UI.StatMenu.data("Fire Rate",Color.blue,0,FireTime, waitTime)
				,new UI.StatMenu.data("Reload"   ,new Color(.5f,.25f,0),0,ReloadTime, waitTime)
				,new UI.StatMenu.data("Heat"     ,Color.cyan,0,MaxHeat, -waitTime)
				,new UI.StatMenu.data("Distance" ,Color.cyan,0,100, MaxDistance)
			};
		}

		public struct BeamerStats
		{
			public float ReloadTime;
			public float MaxDistance;
			public float MaxHeat;
			public float Damage;

			public BeamerStats(LaserBeamer Base)
			{
				ReloadTime = Base.ReloadTime;
				Damage = Base.Damage;
				MaxDistance = Base.MaxDistance;

				MaxHeat = Base.MaxHeat;
			}

			public void Set(LaserBeamer Base)
			{
				Base.ReloadTime = ReloadTime;
				Base.Damage = Damage;
				Base.MaxDistance = MaxDistance;

				Base.MaxHeat = MaxHeat;
			}
		}
	}
}