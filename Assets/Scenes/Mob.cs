using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class Mob : MonoBehaviour 
{
	public Rigidbody2D rb;
	public Brain B;
	public float Speed;
	public int Team;

	public List<Equipment> Equipment;
	public int ActiveSlot;

	private void FixedUpdate()
	{
		Vector2 Dir = B.GetDir();
		rb.AddForce(Dir * Speed * Time.fixedDeltaTime);		
	}

	private void Update()
	{

		ActiveSlot = B.GetSlot(ActiveSlot);

		Equipment[ActiveSlot].transform.right = B.GetLook();

		if (B.isShooting()) 
		{
			Equipment[ActiveSlot].Use(B.GetLook());
		}
	}

	private void Start()
	{
		Equipment[ActiveSlot].Pickup(this);
	}
}

public abstract class Equipment : NetworkBehaviour
{
	public abstract void Use(Vector2 Pos);
	public abstract void Pickup(Mob M);
	public abstract void Drop();
}
