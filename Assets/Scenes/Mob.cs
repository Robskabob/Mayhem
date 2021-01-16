using Mirror;
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

	public List<Equipment> Equipment;
	public int ActiveSlot;

	public List<Collision2D> Collisions = new List<Collision2D>();

	public int Count;

	public float JumpTime;
	public float JumpWait;

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

	private void FixedUpdate()
	{
		Count = Collisions.Count;
		Vector2 Dir = B.GetDir();
		if (Collisions.Count == 0)
			rb.AddForce(Dir * AirSpeed * Time.fixedDeltaTime);
		else
		{
			if (Dir.y > .5 && JumpWait < 0)
			{
				rb.AddForce(Vector2.up * Jump);
				JumpWait = JumpTime;
			}
			rb.AddForce(Dir * Speed * Time.fixedDeltaTime);
			if (Dir.x == 0)
			{

			}
			else if (rb.velocity.x * Dir.x < MinSpeed)
			{
				rb.velocity = new Vector2(MinSpeed * Dir.x, rb.velocity.y);
			}
		}
		JumpWait -= Time.fixedDeltaTime;
	}

	private void Update()
	{

		ActiveSlot = B.GetSlot(ActiveSlot);

		Equipment[ActiveSlot].transform.right = B.GetLook();

		if (B.isShooting()) 
		{
			Equipment[ActiveSlot].Use(B.GetLook());
		}

		if(Shield < MaxShield)
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
		Equipment[ActiveSlot].Pickup(this);
	}

	private void OnCollisionEnter2D(Collision2D col)
	{
		Collisions.Add(col);
	}

	private void OnCollisionExit2D(Collision2D col)
	{
		Collisions.Remove(col);
	}
}

public abstract class Equipment : NetworkBehaviour
{
	public abstract void Use(Vector2 Pos);
	public abstract void Pickup(Mob M);
	public abstract void Drop();
}
