using L33t.Equipment;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mob : MonoBehaviour 
{
	public Text NamePlate;
	public Rigidbody2D rb;
	public Brain B;
	public float Health;
	public float MaxHealth;
	public float Shield;
	public float MaxShield;
	public float ShieldRate;
	public float ShieldTime;
	public float ShieldWait;

	public float MinSpeed;
	public float Speed;
	public float AirSpeed;
	public float Jump;
	public int Team;

	public int MaxItems = 3;

	public List<Equipment> Equipment;
	public List<WeaponEquipment> WeaponEquipment;
	public List<DirectedEquipment> DirectedEquipment;
	public List<ActiveEquipment> ActiveEquipment;
	public List<PasiveEquipment> PassiveEquipment;
	public bool wasDropping;

	public List<Collision2D> Collisions = new List<Collision2D>();

	public int Count;

	public Region Inside;
	public Vector2Int ChunkPos;
	public delegate void MobEvent(Mob M);
	public event MobEvent OnDeath;
	public void Kill() 
	{
		OnDeath?.Invoke(this);
	}

	public float JumpTime;
	public float JumpWait;
	public bool Picked;

	public bool PickUp(Equipment E) 
	{
		if (Picked)
			return false;
		bool full = MaxItems <= Equipment.Count;
		//Debug.Log(full +" " + MaxItems + "<=" + Equipment.Count);
		if (full)
			return Replace(E);
		else
		{
			//Debug.Log("Add");
			switch (E)
			{
				case WeaponEquipment WE:
					WeaponEquipment.Add(WE);
					break;
				case DirectedEquipment DE:
					DirectedEquipment.Add(DE);
					break;
				case ActiveEquipment AE:
					ActiveEquipment.Add(AE);
					break;
				case PasiveEquipment PE:
					PassiveEquipment.Add(PE);
					break;
			}
			Equipment.Add(E);
		}
		Picked = true;
		return true;
	}

	public void Dammage(float value)
	{
		ShieldWait = ShieldTime;
		if (Shield > value)
		{
			Shield -= value;
		}
		else if (Shield > 0)
		{
			//shield Break;
			Health -= value - Shield;
			Shield = 0;
		}
		else
			Health -= value;
	}

	protected virtual void FixedUpdate()
	{
		int ychunk = (int)((transform.position.y - 50) / 100);
		Vector2Int newChunkPos = new Vector2Int((int)((transform.position.x + (50 * (1 - ychunk % 2))) / 100),ychunk);
		if (ChunkPos != newChunkPos) 
		{
			ChunkPos = newChunkPos;
			Debug.DrawLine(transform.position + new Vector3(-1, 1, 0), transform.position + new Vector3(1, -1, 0),Color.white,15);
			Debug.DrawLine(transform.position + new Vector3(-1, -1, 0), transform.position + new Vector3(1, 1, 0), Color.white, 15);
		}
		Count = Collisions.Count;
		Vector2 Dir = B.GetDir();
		if (Collisions.Count == 0)
		{
			rb.AddForce(Dir * AirSpeed * Time.fixedDeltaTime);
			//Aired = true;
		}
		else
		{
			if (Dir.y > .5 && JumpWait < 0 && rb.velocity.y < Jump / 100)
			{
				rb.AddForce(Vector2.up * Jump);
				JumpWait = JumpTime;
				//Aired = false;
			}
			rb.AddForce(Dir * Speed * Time.fixedDeltaTime);
			if (Dir.x == 0)
			{
				rb.velocity = new Vector2(rb.velocity.x / 1.01f, rb.velocity.y);
			}
			else if (rb.velocity.x * Dir.x < MinSpeed)
			{
				rb.velocity = new Vector2(MinSpeed * Dir.x, rb.velocity.y);
			}
		}
		JumpWait -= Time.fixedDeltaTime;
	}

	public void Drop(Equipment E)
	{
		switch (E)
		{
			case WeaponEquipment WE:
				WeaponEquipment.Remove(WE);
				break;
			case DirectedEquipment DE:
				DirectedEquipment.Remove(DE);
				break;
			case ActiveEquipment AE:
				ActiveEquipment.Remove(AE);
				break;
			case PasiveEquipment PE:
				PassiveEquipment.Remove(PE);
				break;
		}
		Equipment.Remove(E);
		E.Drop();
		B.OnDrop();
	}
	public bool Replace(Equipment E)
	{
		switch (E)
		{
			case WeaponEquipment WE:
				if (WeaponEquipment.Count == 0)
					return false;
				WeaponEquipment[B.GetSlotW()].Drop();
				Equipment.Remove(WeaponEquipment[B.GetSlotW()]);
				WeaponEquipment[B.GetSlotW()] = WE;
				break;
			case DirectedEquipment DE:
				if (DirectedEquipment.Count == 0)
					return false;
				DirectedEquipment[B.GetSlotD()].Drop();
				Equipment.Remove(DirectedEquipment[B.GetSlotD()]);
				DirectedEquipment[B.GetSlotD()] = DE;
				break;
			case ActiveEquipment AE:
				if (ActiveEquipment.Count == 0)
					return false;
				ActiveEquipment[B.GetSlotA()].Drop();
				Equipment.Remove(ActiveEquipment[B.GetSlotA()]);
				ActiveEquipment[B.GetSlotA()] = AE;
				break;
			case PasiveEquipment PE:
				if (PassiveEquipment.Count == 0)
					return false;
				PassiveEquipment[B.GetSlotP()].Drop();
				Equipment.Remove(PassiveEquipment[B.GetSlotP()]);
				PassiveEquipment[B.GetSlotP()] = PE;
				break;
		}
		Picked = true;
		Equipment.Add(E);
		return true;
	}

	protected virtual void Update()
	{
		if (!B.isInteracting())
			Picked = false;

		int ActiveSlot = B.GetSlotD();

		if (DirectedEquipment.Count > 0)
		{
			DirectedEquipment[ActiveSlot].transform.right = B.GetLook() - (Vector2)transform.position;

			if (B.isDropping())
			{
				if (!wasDropping && (B as PlayerBrain).Shift)
				{
					Equipment.Remove(DirectedEquipment[ActiveSlot]);
					DirectedEquipment[ActiveSlot].Drop();
					DirectedEquipment.RemoveAt(ActiveSlot);
					wasDropping = true;
					B.OnDrop();
				}
			}
			else
			{
				wasDropping = false;
			}

			if (B.isShootingSide())
			{
				DirectedEquipment[ActiveSlot].Use(B.GetLook());
			}
		}

		ActiveSlot = B.GetSlotA();

		if (ActiveEquipment.Count > 0)
		{
			//ActiveEquipment[ActiveSlot].transform.right = B.GetLook() - (Vector2)transform.position;

			if (B.isDropping())
			{
				if (!wasDropping && (B as PlayerBrain).ALT)
				{
					Equipment.Remove(ActiveEquipment[ActiveSlot]);
					ActiveEquipment[ActiveSlot].Drop();
					ActiveEquipment.RemoveAt(ActiveSlot);
					wasDropping = true;
					B.OnDrop();
				}
			}
			else
			{
				wasDropping = false;
			}

			if (B.isActivate())
			{
				ActiveEquipment[ActiveSlot].Use();
			}
		}

		ActiveSlot = B.GetSlotW();

		if (WeaponEquipment.Count > 0)
		{
			WeaponEquipment[ActiveSlot].transform.right = B.GetLook() - (Vector2)transform.position;

			if (B.isDropping())
			{
				if (!wasDropping)//needs to be last
				{
					Equipment.Remove(WeaponEquipment[ActiveSlot]);
					WeaponEquipment[ActiveSlot].Drop();
					WeaponEquipment.RemoveAt(ActiveSlot);
					wasDropping = true;
					B.OnDrop();
				}
			}
			else
			{
				wasDropping = false;
			}

			if (B.isShooting())
			{
				WeaponEquipment[ActiveSlot].Use(B.GetLook());
			}
		}


		if (Shield < MaxShield)
		{
			if (ShieldWait < 0)
			{
				Shield += Time.deltaTime * ShieldRate;
				if (Shield > MaxShield)
					Shield = MaxShield;
			}
			else
				ShieldWait -= Time.deltaTime;
		}

		if(Health < 0) 
		{
			B.Die();
		}
	}

	private void Start()
	{
		//Equipment[ActiveSlot].Pickup(this);
	}

	private void OnCollisionEnter2D(Collision2D col)
	{
		if(col.gameObject.GetComponent<Projectile>() == null)
			Collisions.Add(col);
	}

	private void OnCollisionExit2D(Collision2D col)
	{
		if (col.gameObject.GetComponent<Projectile>() == null)
			Collisions.Remove(col);
	}
}

public class MadMob : Mob
{

}

public class NetMob : Mob
{
	public Vector2 TargPos;
	public Vector2 TargVel;
	public float strngth;
	public float UpdateInterval;
	public float CorrectionThreshold;

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		Vector2 posdiff = rb.position - TargPos;
		Vector2 veldiff = rb.velocity - TargVel;

		if (posdiff.magnitude > CorrectionThreshold)
		{
			rb.velocity += posdiff * Time.fixedDeltaTime;
		}
		if (veldiff.magnitude > CorrectionThreshold)
		{
			rb.velocity += veldiff * Time.fixedDeltaTime;
		}
	}

	protected override void Update()
	{
		if (!B.isInteracting())
			Picked = false;

		int ActiveSlot = B.GetSlotD();

		if (DirectedEquipment.Count > 0)
		{
			DirectedEquipment[ActiveSlot].transform.right = B.GetLook() - (Vector2)transform.position;

			if (B.isShootingSide())
			{
				DirectedEquipment[ActiveSlot].Use(B.GetLook());
			}
		}

		ActiveSlot = B.GetSlotA();

		if (ActiveEquipment.Count > 0)
		{
			if (B.isActivate())
			{
				ActiveEquipment[ActiveSlot].Use();
			}
		}

		ActiveSlot = B.GetSlotW();

		if (WeaponEquipment.Count > 0)
		{
			WeaponEquipment[ActiveSlot].transform.right = B.GetLook() - (Vector2)transform.position;

			if (B.isShooting())
			{
				WeaponEquipment[ActiveSlot].Use(B.GetLook());
			}
		}


		if (Shield < MaxShield)
		{
			if (ShieldWait < 0)
			{
				Shield += Time.deltaTime * ShieldRate;
				if (Shield > MaxShield)
					Shield = MaxShield;
			}
			else
				ShieldWait -= Time.deltaTime;
		}
	}
}