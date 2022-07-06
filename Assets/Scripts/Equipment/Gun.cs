using Mirror;
using UnityEngine;

namespace L33t.Equipment
{
	public class ModulusGun// : WeaponEquipment 
	{
		public class loader
		{
			
		}

		public class FireMode 
		{
			
		}

		public class dammager 
		{
		
		}
	}
	public class Gun : WeaponEquipment
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
		[Command]
		public override void Drop()
		{
			PickUpAble = true;
			ExpireTime = 30;
			Abandand = true;
			netIdentity.RemoveClientAuthority();
			transform.parent = null;
			Holder = null;
			RpcDrop();
		}
		[ClientRpc]
		public void RpcDrop()
		{
			PickUpAble = true;
			transform.parent = null;
			Holder = null;
		}

		public override bool Pickup(Mob M)
		{
			if (!M.PickUp(this))
				return false;
			Abandand = false;
			transform.parent = M.transform;
			transform.localPosition = Vector3.zero;
			Holder = M;

			netIdentity.AssignClientAuthority(M.B.netIdentity.connectionToClient);

			RpcPickup(M.B.netId);
			return true;
		}

		[ClientRpc]
		public void RpcPickup(uint MobId)
		{
			Abandand = false;
			if (!isServer)
			{
				Mob M = NetworkIdentity.spawned[MobId].GetComponent<Brain>().Body;
				transform.parent = M.transform;
				transform.localPosition = Vector3.zero;
				Holder = M;
				if (!M.PickUp(this))
					Debug.LogError("Cant pick up but valid on Server?");
			}
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

		protected virtual void Fire(Vector2 Pos)
		{
			Projectile P = Instantiate(Projectile);
			P.Shoot(Holder, Pos - (Vector2)transform.position, ProjectileData);
		}

		protected override void Update()
		{
			base.Update();
			waitTime -= Time.deltaTime;
		}

		public override void Randomize()
		{
			FireTime = Random.Range(.05f, 1) * Random.Range(.5f, 2f);
			ReloadTime = Random.Range(FireTime * 2, Random.Range(FireTime * 2, 5));

			Clip = (int)(Random.Range(.5f, 5) / FireTime);
			if (Clip < 1)
				Clip = 1;

			ProjectileData.Impulse = Random.Range(10, 1000f) * FireTime;
			ProjectileData.LifeTime = (Random.Range(1, 50f) * 100) / ProjectileData.Impulse;
			ProjectileData.Speed = Random.Range(0, 5f) * ReloadTime;
			ProjectileData.Dammage = (Random.Range(5, 30f)) / Clip * ReloadTime;
			ProjectileData.Health = Random.Range(1, 100f);

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
				$"Damage {ProjectileData.Dammage:f}\n" +
				$"Fire Rate {FireTime:f}\n" +
				$"Reload {ReloadTime:f} | {Mathf.Max(waitTime, 0):f}\n" +
				$"Speed {ProjectileData.Impulse:f} | {ProjectileData.Speed:f}\n" +
				$"Clip {clip} / {Clip}";
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
			public float Health;

			public GunStats(Gun Base)
			{
				ReloadTime = Base.ReloadTime;
				FireTime = Base.FireTime;

				Clip = Base.Clip;

				Impulse = Base.ProjectileData.Impulse;
				LifeTime = Base.ProjectileData.LifeTime;
				Speed = Base.ProjectileData.Speed;
				Dammage = Base.ProjectileData.Dammage;
				Health = Base.ProjectileData.Health;
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
				Base.ProjectileData.Health = Health;
			}
		}
	}


	public abstract class LaserBase : WeaponEquipment
	{
		public LineRenderer LaserVis;

		public float ReloadTime;
		public float FireTime;
		public float waitTime;

		//public int Clip;
		//public int clip;

		public float MaxDistance;
		public float Damage;

		public Vector2 HitPos;

		public bool Shoot;

		public Mob Holder;
		[Command]
		public override void Drop()
		{
			PickUpAble = true;
			ExpireTime = 30;
			Abandand = true;
			netIdentity.RemoveClientAuthority();
			transform.parent = null;
			Holder = null;
			RpcDrop();
		}
		[ClientRpc]
		public void RpcDrop()
		{
			PickUpAble = true;
			transform.parent = null;
			Holder = null;
		}

		public override bool Pickup(Mob M)
		{
			if (!M.PickUp(this))
				return false;
			Abandand = false;
			Color C = M.GetComponent<SpriteRenderer>().color;
			LaserVis.endColor = C;
			LaserVis.startColor = C;
			transform.parent = M.transform;
			transform.localPosition = Vector3.zero;
			Holder = M;

			netIdentity.AssignClientAuthority(M.B.netIdentity.connectionToClient);

			RpcPickup(M.B.netId);
			return true;
		}

		[ClientRpc]
		public void RpcPickup(uint MobId)
		{
			Abandand = false;
			if (!isServer)
			{
				Mob M = NetworkIdentity.spawned[MobId].GetComponent<Brain>().Body;
				Color C = M.GetComponent<SpriteRenderer>().color;
				LaserVis.endColor = C;
				LaserVis.startColor = C;
				transform.parent = M.transform;
				transform.localPosition = Vector3.zero;
				Holder = M;
				if (!M.PickUp(this))
					Debug.LogError("Cant pick up but valid on Server?");
			}
		}

		public override void Use(Vector2 Pos)
		{
			//if (waitTime < 0)
			//{
			//	if (clip <= 0)
			//	{
			//		waitTime = ReloadTime;
			//		clip = Clip;
			//	}
			//	else
			//	{
			//		waitTime = FireTime;
			//		clip--;
			//		Fire(Pos);
			//	}
			//}
		}



		//[Command(ignoreAuthority = true)]
		public void SetVis(bool val)
		{
			LaserVis.enabled = val;
			Shoot = val;
			//if (isServer)
			//	SetVisRPC(val);
		}
		//[Command(ignoreAuthority = true)]
		public void SetVisPos(Vector3 pos, Vector2 hitpos)
		{
			LaserVis.SetPositions(new Vector3[] { pos, hitpos });
			if(isServer)
				SetVisPosRPC(pos, hitpos);
		}
		[ClientRpc]
		public void SetVisRPC(bool val)
		{
			LaserVis.enabled = val;
			Shoot = val;
		}
		[ClientRpc]
		public void SetVisPosRPC(Vector3 pos, Vector2 hitpos)
		{
			LaserVis.SetPositions(new Vector3[] {pos, hitpos});
		}
		protected virtual void Fire(Vector2 Pos)
		{
			RaycastHit2D r = Physics2D.Raycast(transform.parent.position, Pos - (Vector2)transform.parent.position, MaxDistance, 6656);//layers 9,11,12
			SetVis(true);
			if (r.point == Vector2.zero)
			{
				HitPos = (Pos - (Vector2)transform.parent.position).normalized * MaxDistance + (Vector2)transform.position;
			}
			else
			{
				HitPos = r.point;
				if (r.collider != null)
				{
					if (r.collider.GetComponent<Projectile>() is Projectile P)
					{
						P.Health -= Damage * Time.deltaTime;
					}
					if (r.collider.GetComponent<Mob>() is Mob M)
					{
						M.Dammage(Damage * Time.deltaTime);
					}
				}
			}
			SetVisPos(transform.position, HitPos);
		}

		protected override void Update()
		{
			if (waitTime >= 0)
				waitTime -= Time.deltaTime;
			else if (!Shoot)
				waitTime += Time.deltaTime;
			LaserVis.enabled = Shoot;
			Shoot = false;
			//LaserVis
		}

		//public override void Randomize()
		//{
		//	FireTime = Random.Range(.05f, 1) * Random.Range(.5f, 2f);
		//	ReloadTime = Random.Range(FireTime * 2, Random.Range(FireTime * 2, 5));
		//
		//	Clip = (int)(Random.Range(.5f, 5) / FireTime);
		//	if (Clip < 1)
		//		Clip = 1;
		//
		//
		//	SetStats(new GunStats(this));
		//}

		//ClientRpc]
		//ublic void SetStats(GunStats stats)
		//
		//	stats.Set(this);
		//

		//public override string PrintStats()
		//{
		//	return
		//		$"Damage {ProjectileData.Dammage:f}\n" +
		//		$"Fire Rate {FireTime:f}\n" +
		//		$"Reload {ReloadTime:f} | {Mathf.Max(waitTime, 0):f}\n" +
		//		$"Speed {ProjectileData.Impulse:f} | {ProjectileData.Speed:f}\n" +
		//		$"Clip {clip} / {Clip}";
		//}
	}
}