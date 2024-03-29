﻿using UnityEngine;
using Mirror;

namespace L33t.Equipment
{
	public class JetPack : ActiveEquipment
	{
		public Mob M;

		public float Force;
		public float MaxFuel;
		public float Fuel;
		public float FuelRate;
		public float FuelTime;
		public float FuelWait;


		[Command]
		public override void Drop()
		{
			PickUpAble = true;
			ExpireTime = 30;
			Abandand = true;
			transform.parent = null;
			netIdentity.RemoveClientAuthority();
			M = null;
			RpcDrop();
		}
		[ClientRpc]
		public void RpcDrop()
		{
			PickUpAble = true;
			transform.parent = null;
			M = null;
		}

		public override bool Pickup(Mob M)
		{
			if (!M.PickUp(this))
				return false;
			Abandand = false;
			transform.parent = M.transform;
			transform.localPosition = Vector3.zero;
			this.M = M;

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
				this.M = M;
				if (!M.PickUp(this))
					Debug.LogError("Cant pick up but valid on Server?");
			}
		}

		protected override void Update()
		{
			base.Update();
			FuelWait -= Time.deltaTime;
			if (FuelWait < 0 && Fuel < MaxFuel && (M == null || M.Collisions.Count != 0))
			{
				Fuel += Time.deltaTime;
			}
		}

		public override void Use()
		{
			//move to fixed?
			if (Fuel > 0)
			{
				Fuel -= Time.deltaTime;
				M.rb.AddForce(Vector2.up * Force * Time.deltaTime);
				FuelWait = FuelTime;
			}
		}

		public override string PrintStats()
		{
			return
				$"Force {Force:f}\n" +
				$"Fuel {Fuel:f} / {MaxFuel:f} : {FuelRate:f}\n" +
				$"Delay {Mathf.Max(0, FuelWait):f} / {FuelTime:f}";
		}

		public override UI.StatMenu.data[] GetStats()
		{
			return new UI.StatMenu.data[]
			{
				 new UI.StatMenu.data("Force",Color.blue,0,100, Force)
				,new UI.StatMenu.data("Fuel" ,new Color(.5f,.25f,0),0,MaxFuel, Fuel)
				,new UI.StatMenu.data("Fuel" ,Color.cyan,0,FuelTime, FuelWait)
			};
		}
		public override void Randomize()
		{
			Force = Random.Range(1000, Random.Range(2500, 6000f));
			MaxFuel = Random.Range(1.5f, 15) / (Force / 1000);
			FuelTime = Random.Range(.1f, 2);
			FuelRate = Random.Range(.5f, 5) * (FuelTime + .5f);

			SetStats(new JetStats(this));
		}

		[ClientRpc]
		public void SetStats(JetStats stats)
		{
			stats.Set(this);
		}


		public struct JetStats
		{
			public float Force;
			public float MaxFuel;
			public float FuelRate;
			public float FuelTime;

			public JetStats(JetPack Base)
			{
				Force = Base.Force;
				MaxFuel = Base.MaxFuel;
				FuelRate = Base.FuelRate;
				FuelTime = Base.FuelTime;
			}

			public void Set(JetPack Base)
			{
				Base.Force = Force;
				Base.Fuel = MaxFuel;
				Base.MaxFuel = MaxFuel;
				Base.FuelRate = FuelRate;
				Base.FuelTime = FuelTime;
			}
		}
	}
}