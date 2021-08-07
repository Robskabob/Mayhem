using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace L33t.Equipment
{
	public class GrappleGun : DirectedEquipment
	{
		//public DistanceJoint2D Rope;
		public Rigidbody2D rb;
		public LineRenderer RopeVis;
		public List<Vector2> PosStack;
		//public Vector2 Pos;
		public float MaxDistance = 25;
		public float FullDistance;
		public float Distance;

		public bool inUse;
		public bool inUseLast;

		public float Force;
		public float BounceRange;
		public float NormalRange;
		public float MaxBounceMult;

		[Command]
		public override void Drop()
		{
			PickUpAble = true;
			ExpireTime = 30;
			Abandand = true;
			transform.parent = null;
			rb = null;
			netIdentity.RemoveClientAuthority();
			RpcDrop();
		}
		[ClientRpc]
		public void RpcDrop()
		{
			PickUpAble = true;
			transform.parent = null;
			rb = null;
		}

		public override bool Pickup(Mob M)
		{
			if (!M.PickUp(this))
				return false;
			Abandand = false;
			transform.parent = M.transform;
			transform.localPosition = Vector3.zero;
			rb = M.rb;

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
				rb = M.rb;
				if (!M.PickUp(this))
					Debug.LogError("Cant pick up but valid on Server?");
			}
		}

		public override void Use(Vector2 pos)
		{
			if (inUse == false && !Latched)
			{
				LineDistance = 0;
				RaycastHit2D r = Physics2D.Raycast(transform.parent.position, pos - (Vector2)transform.parent.position, MaxDistance, 1 << 9);
				if (r.point == Vector2.zero)
					return;
				if (r.collider != null && r.collider.GetComponent<Projectile>() is Projectile P)
				{
					return;
				}
				Health = MaxHealth;
				PosStack = new List<Vector2>() { r.point };
				SignStack = new List<bool>();
				//Rope.anchor = Pos;
				//Rope.enabled = true;
				RopeVis.enabled = true;
				FullDistance = r.distance;
				if (hasAuthority)
					CmdGrapple(r.point, FullDistance);
				//Rope.distance = Vector2.Distance(transform.position, Pos);
				//Rope.anchor = Pos;// - (Vector2)transform.position;
				//Rope.connectedAnchor = Pos;
				//Rope.target = Pos;
				Latched = true;
				inUse = true;
			}
			//Rope.distance -= Time.deltaTime * Real;
			inUseLast = true;
		}

		[Command]
		public void CmdGrapple(Vector2 pos, float dist)
		{
			RpcGrapple(pos, dist);
		}
		[ClientRpc(includeOwner = false)]
		public void RpcGrapple(Vector2 pos, float dist)
		{
			PosStack = new List<Vector2>() { pos };
			FullDistance = dist;
		}

		public float DistOff;
		public Vector2 point;
		public float LineDistance;
		public float Angle;
		public float Angle2;
		public List<bool> SignStack;
		public float Health;
		public float MaxHealth;
		private void FixedUpdate()
		{
			if (inUse)
			{
				Distance = Vector2.Distance(transform.position, PosStack[PosStack.Count - 1]);
				RaycastHit2D r = Physics2D.Raycast(transform.position, PosStack[PosStack.Count - 1] - (Vector2)transform.position, Distance - .01f, 1 << 9);
				if (r.collider != null && r.collider.GetComponent<Projectile>() is Projectile P)
				{
					Health -= P.Data.Dammage;
					if (Health < 0)
						inUse = false;
					return;
				}
				if (r.point != Vector2.zero)
				{
					PosStack.Add(r.collider.bounds.ClosestPoint(r.point));
					LineDistance += Vector2.Distance(PosStack[PosStack.Count - 2], PosStack[PosStack.Count - 1]);
					//SignStack.Add(Vector2.SignedAngle((Vector2)transform.position - PosStack[PosStack.Count - 1], PosStack[PosStack.Count - 2] - PosStack[PosStack.Count - 1])<0);
				}
				else if (PosStack.Count > 1)
				{
					r = Physics2D.Raycast(transform.position, PosStack[PosStack.Count - 2] - (Vector2)transform.position, Vector2.Distance(PosStack[PosStack.Count - 2], transform.position) - .01f, 1 << 9);
					if (r.collider != null && r.collider.GetComponent<Projectile>() is Projectile P2)
					{
						Health -= P2.Data.Dammage;
						if (Health < 0)
							inUse = false;
						return;
					}
					DistOff = Vector2.Distance(transform.position, PosStack[PosStack.Count - 2]) - r.distance;
					point = r.point;
					Angle = Vector2.SignedAngle((Vector2)transform.position - PosStack[PosStack.Count - 1], PosStack[PosStack.Count - 2] - PosStack[PosStack.Count - 1]);
					Angle2 = Vector2.Angle((Vector2)transform.position - PosStack[PosStack.Count - 1], PosStack[PosStack.Count - 2] - PosStack[PosStack.Count - 1]);
					//Debug.Log($"Distance:{Vector2.Distance(PosStack[PosStack.Count - 2], PosStack[PosStack.Count - 1])} RayDist:{r.distance} RayPoint:{r.point}");
					if (r.point == Vector2.zero)
					//angle checker
					//if (SignStack[SignStack.Count - 1] ? Angle > 0 : Angle < 0)
					{
						LineDistance -= Vector2.Distance(PosStack[PosStack.Count - 2], PosStack[PosStack.Count - 1]);
						PosStack.RemoveAt(PosStack.Count - 1);
						//	SignStack.RemoveAt(SignStack.Count - 1);
					}
				}
				Distance += LineDistance;
				float Diff = Distance - FullDistance;
				if (Diff > 0)
				{
					Vector2 impulse = ((PosStack[PosStack.Count - 1] - (Vector2)transform.position)).normalized * Force * Time.fixedDeltaTime;
					if (Diff > NormalRange) // > 2
					{
						//Debug.DrawLine(transform.position, (Vector2)transform.position + impulse / 10, Color.yellow);
						Debug.DrawLine((Vector2)transform.position + impulse / 10, (Vector2)transform.position + (impulse * Mathf.Min(MaxBounceMult, Diff - NormalRange + BounceRange)) / 10, Color.red);
						rb.AddForce(impulse * Mathf.Min(MaxBounceMult, Diff - NormalRange + BounceRange));
					}
					else if (Diff > BounceRange)// 1 - 2
					{
						Debug.DrawLine(transform.position, (Vector2)transform.position + impulse / 10, Color.yellow);
						rb.AddForce(impulse);
					}
					else // 0 - 1
					{
						Debug.DrawLine(transform.position, (Vector2)transform.position + impulse * (Diff / BounceRange) / 10, Color.green);
						rb.AddForce(impulse);
					}
				}

			}
		}
		public bool Latched;
		protected override void Update()
		{
			base.Update();
			if (!inUse)
			{
				RopeVis.enabled = false;
			}
			if (!inUseLast)
			{
				//Rope.enabled = false;
				RopeVis.enabled = false;
				inUse = false;
				Latched = false;
			}

			inUseLast = false;

			Vector3[] LineArray = new Vector3[PosStack.Count + 1];
			LineArray[0] = transform.position;
			for (int i = 0; i < PosStack.Count; i++)
			{
				LineArray[PosStack.Count - i] = PosStack[i];
			}
			RopeVis.positionCount = PosStack.Count + 1;
			RopeVis.SetPositions(LineArray);
		}
		public override string PrintStats()
		{
			return
				$"Max Dist {MaxDistance:f}\n" +
				$"Force {Force:f}\n" +
				$"Health {Health:f} / {MaxHealth:f} \n" +
				$"R {BounceRange:f} | {NormalRange:f}\n" +
				$"Mult {MaxBounceMult:f}";
		}

		public override void Randomize()
		{
			MaxHealth = Random.Range(5, 100f);
			MaxDistance = Random.Range(10, 50f);

			Force = Random.Range(500, 5000f) * Random.Range(1, 10f);
			BounceRange = Random.Range(.1f, MaxDistance / 10);
			NormalRange = Random.Range(BounceRange, MaxDistance / 5);
			MaxBounceMult = Random.Range(1, 10f);

			SetStats(new GrappleStats(this));
		}

		[ClientRpc]
		public void SetStats(GrappleStats stats)
		{
			stats.Set(this);
		}

		public struct GrappleStats
		{
			public float MaxDistance;
			public float MaxHealth;
			public float Force;
			public float BounceRange;
			public float NormalRange;
			public float MaxBounceMult;

			public GrappleStats(GrappleGun Base)
			{
				MaxDistance = Base.MaxDistance;
				MaxHealth = Base.MaxHealth;
				Force = Base.Force;
				BounceRange = Base.BounceRange;
				NormalRange = Base.NormalRange;
				MaxBounceMult = Base.MaxBounceMult;
			}

			public void Set(GrappleGun Base)
			{
				Base.MaxDistance = MaxDistance;
				Base.MaxHealth = MaxHealth;
				Base.Force = Force;
				Base.BounceRange = BounceRange;
				Base.NormalRange = NormalRange;
				Base.MaxBounceMult = MaxBounceMult;
			}
		}
	}
}