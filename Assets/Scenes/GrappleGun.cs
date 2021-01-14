using System.Collections.Generic;
using UnityEngine;

public class GrappleGun : Equipment
{
	//public DistanceJoint2D Rope;
	public Rigidbody2D rb;
	public LineRenderer RopeVis;
	public List<Vector2> PosStack;
	//public Vector2 Pos;
	public float Dist;
	public float Distance;

	public bool inUse;
	public bool inUseLast;

	public float Real;
	public float Force;

	public override void Drop()
	{
		transform.parent = null;
		//Destroy(Rope);
		rb = null;
	}

	public override void Pickup(Mob M)
	{
		transform.parent = M.transform;
		rb = M.rb;
		//Rope = M.gameObject.AddComponent<DistanceJoint2D>();
		//Rope.autoConfigureConnectedAnchor = false;
		//Rope.autoConfigureDistance = false;
		//Rope.enableCollision = true;
		//Rope.maxDistanceOnly = true;
		//Rope.maxForce = 35;
		//Rope.connectedBody = M.rb;
	}

	public override void Use(Vector2 pos)
	{
		if(inUse == false) 
		{
			LineDistance = 0;
			RaycastHit2D r = Physics2D.Raycast(transform.parent.position, pos - (Vector2)transform.parent.position,25);
			if (r.point == Vector2.zero)
				return;
			PosStack = new List<Vector2>() { r.point };
			SignStack = new List<bool>();
			//Rope.anchor = Pos;
			//Rope.enabled = true;
			RopeVis.enabled = true;
			Dist = r.distance;
			//Rope.distance = Vector2.Distance(transform.position, Pos);
			//Rope.anchor = Pos;// - (Vector2)transform.position;
			//Rope.connectedAnchor = Pos;
			//Rope.target = Pos;
		}
		//Rope.distance -= Time.deltaTime * Real;
		inUseLast = true;
		inUse = true;
	}
	Vector2 velocity;
	public float DistOff;
	public Vector2 point;
	public float LineDistance;
	public float Angle;
	public float Angle2;
	public List<bool> SignStack;
	public float Health;
	private void FixedUpdate()
	{
		if (inUse)
		{
			Distance = Vector2.Distance(transform.position, PosStack[PosStack.Count - 1]);
			RaycastHit2D r = Physics2D.Raycast(transform.position, PosStack[PosStack.Count - 1] - (Vector2)transform.position, Distance - .01f);
			if(r.collider != null && r.collider.GetComponent<Projectile>() is Projectile P)
			{
				Health -= P.Data.Dammage;
				if (Health < 0)
					inUseLast = false;
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
				r = Physics2D.Raycast(transform.position, PosStack[PosStack.Count - 2] - (Vector2)transform.position, Vector2.Distance(PosStack[PosStack.Count - 2], transform.position) - .01f);
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
			if (Dist < Distance) 
			{
				velocity += (PosStack[PosStack.Count - 1] - (Vector2)transform.position) * Force * (Distance - Dist);
			}
			else 
			{
				velocity *= 0;
			}
			velocity /= 2;
			rb.AddForce(velocity* Time.fixedDeltaTime);
		
		}
		//if (inUse)
		//{
		//	Distance = Vector2.Distance(transform.position, Pos);
		//	if(Dist < Distance) 
		//	{
		//		rb.AddForce(Vector2.Dot(rb.velocity, ((Vector2)transform.position - Pos).));
		//	}
		//
		//}
	}

	private void Update()
	{
		if (inUse)
		{
			if (!inUseLast)
			{
				//Rope.enabled = false;
				RopeVis.enabled = false;
				inUse = false;
			}
			//Distance = Vector2.Distance(transform.position, Pos);
			////if(Dist < Distance) 
			////{
			//	rb.AddForce((Pos - (Vector2)transform.position) * Force * (Distance - Dist));
			////}

		}
		inUseLast = false;
		Vector3[] LineArray = new Vector3[PosStack.Count+1];
		LineArray[0] = transform.position;
		for(int i = 0; i < PosStack.Count; i++) 
		{
			LineArray[PosStack.Count-i] = PosStack[i];
		}
		RopeVis.positionCount = PosStack.Count + 1;
		RopeVis.SetPositions(LineArray);
	}
}
