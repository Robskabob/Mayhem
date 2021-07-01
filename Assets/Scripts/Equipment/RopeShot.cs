using UnityEngine;
using Mirror;

public class RopeShot : DirectedEquipment
{
	public Rigidbody2D rb;
	public Transform Shot;
	public LineRenderer RopeVis;

	public Vector2 Pos;

	public float MaxDistance;
	public float BreakDist;
	public float Force;
	public float Speed;
	public float MaxHealth;
	public float Health;

	public bool inUse;
	public bool inUseLast;
	public bool inAir;
	public bool Latched;

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
		if (inUse == false && !Latched && !inAir)
		{
			RaycastHit2D r = Physics2D.Raycast(transform.parent.position, pos - (Vector2)transform.parent.position, MaxDistance);
			if (r.point == Vector2.zero)
				return;
			if (r.collider != null && r.collider.GetComponent<Projectile>() is Projectile P)
			{
				return;
			}
			Health = MaxHealth;

			Pos = r.point;
			RopeVis.enabled = true;
			Shot.parent = null;

			if (hasAuthority)
				CmdLatch(Pos);

			inUse = true;
			inAir = true;
			Latched = true;
		}
		inUseLast = true;
	}

	[Command]
	public void CmdLatch(Vector2 pos)
	{
		Pos = pos;
		RpcLatch(pos);
	}
	[ClientRpc(excludeOwner = true)]
	public void RpcLatch(Vector2 pos)
	{
		Pos = pos;
	}
	public float Distance;
	private void FixedUpdate()
	{
		if (inUse)
		{
			Distance = Vector2.Distance(transform.position, Pos);
			if (Distance < BreakDist)
			{
				//Debug.Log("Short");
				inUse = false;
				return;
			}
			RaycastHit2D r = Physics2D.Raycast(transform.position, Pos - (Vector2)transform.position, Distance - .01f);
			if (r.collider != null && r.collider.GetComponent<Projectile>() is Projectile P)
			{
				Debug.Log("Shot dead");
				Health -= P.Data.Dammage;
				if (Health < 0)
					inUse = false;
				return;
			}

			if (inAir)
			{
				Shot.transform.position += (Vector3)((Pos - (Vector2)Shot.position).normalized * Speed * Time.fixedDeltaTime);
				//Debug.Log("To " + ((Pos - (Vector2)Shot.position).normalized * Speed * Time.fixedDeltaTime).magnitude);
				if (Vector2.Distance(Pos, Shot.position) < Speed * Time.fixedDeltaTime)
				{
					inAir = false;
					Shot.transform.position = Pos;
				}
			}
			else
			{
				if (r.point != Vector2.zero)
				{
					//if (Vector2.Distance(r.point, Pos) > .1f)
					//{
					inUse = false;
					return;
					//}
				}
				rb.AddForce((Pos - (Vector2)transform.parent.position).normalized * Force * Time.fixedDeltaTime);
			}
		}
		else if (inAir)
		{
			Shot.transform.position += (Vector3)(((Vector2)transform.position - (Vector2)Shot.position).normalized * Speed * Time.fixedDeltaTime);
			//Debug.Log("Back "+ (((Vector2)transform.position - (Vector2)Shot.position).normalized * Speed * Time.fixedDeltaTime).magnitude);
			if (Vector2.Distance(transform.position, Shot.position) < Speed * Time.fixedDeltaTime)
			{
				//Debug.Log($" {Vector2.Distance(transform.position, Shot.position)} | {Speed * Time.fixedDeltaTime}");
				inAir = false;
				Latched = false;
				Shot.parent = transform;
				Shot.localPosition = new Vector3(.65f, 0);
			}
		}
	}

	protected override void Update()
	{
		base.Update();
		if (!inUse)
		{
			if (Latched)
			{
				inAir = true;
			}
			else
			{
				RopeVis.enabled = false;
			}
		}
		if (!inUseLast)
		{
			//Rope.enabled = false;
			inUse = false;
			if (inAir)
			{
				RopeVis.enabled = true;
				Latched = false;
			}
			else
				RopeVis.enabled = false;
		}

		inUseLast = false;

		RopeVis.SetPositions(new Vector3[] { transform.position, Shot.position });
	}

	public override string PrintStats()
	{
		return
			$"Max Dist {MaxDistance:f}\n" +
			$"Break Dist {BreakDist:f}\n" +
			$"Force {Force:f}\n" +
			$"Speed {Speed:f}\n" +
			$"Health {Health:f} / {MaxHealth:f}";
	}

	public override void Randomize()
	{
		MaxHealth = Random.Range(5, 100f);
		MaxDistance = Random.Range(5, 50f);

		Speed = Random.Range(10, 50f) * Random.Range(.5f, 3);
		Force = Random.Range(1000, 5000f) * Random.Range(.5f, 1);
		BreakDist = Random.Range(.1f, MaxDistance / 10);

		SetStats(new RopeStats(this));
	}

	[ClientRpc]
	public void SetStats(RopeStats stats)
	{
		stats.Set(this);
	}

	public struct RopeStats
	{
		public float MaxDistance;
		public float MaxHealth;
		public float Force;
		public float Speed;
		public float BreakDist;

		public RopeStats(RopeShot Base)
		{
			MaxDistance = Base.MaxDistance;
			MaxHealth = Base.MaxHealth;
			Force = Base.Force;
			Speed = Base.Speed;
			BreakDist = Base.BreakDist;
		}

		public void Set(RopeShot Base)
		{
			Base.MaxDistance = MaxDistance;
			Base.MaxHealth = MaxHealth;
			Base.Force = Force;
			Base.Speed = Speed;
			Base.BreakDist = BreakDist;
		}
	}
}
