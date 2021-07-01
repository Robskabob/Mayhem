using Mirror;
using System.Linq;
using UnityEngine;

public class PlatBrain : Brain
{
	public bool dir;
	public float wait;
	public float time;

	private void FixedUpdate()
	{
		if (wait < 0 && Physics2D.Raycast(transform.position, Vector2.down + GetDir(), 1.1f).point == Vector2.zero) 
		{
			dir = !dir;
			wait = time;
		}
		wait -= Time.fixedDeltaTime;
		if((Time.frameCount + GetHashCode()) % 30 == 0) 
		{
			foreach(PlayerBrain PB in NetSystem.I.PlayerBrains.Values)
			{
				if (Vector2.Distance(PB.transform.position, transform.position) < 100)
					return;
			}
			Die();
		}
	}

	private void OnDisable()
	{
		Die();
		if(!gameObject.activeSelf)
			gameObject.SetActive(true);
		enabled = true;
		Body.rb.simulated = true;
	}

	public override void Die()
	{
		if (isServer)
		{

			Vector2 rel;
			if (NetSystem.I.PlayerBrains.Count > 0)
				rel = NetSystem.I.PlayerBrains.ElementAt(Random.Range(0, NetSystem.I.PlayerBrains.Count)).Value.transform.position;
			else
				rel = Vector2.zero;
			Vector2 pos = Physics2D.Raycast(Random.insideUnitCircle * 100 + rel, Vector2.down).point;

			RpcSpawn(Physics2D.Raycast((Random.insideUnitCircle + Vector2.up) * 50 + pos, Vector2.down).point + Vector2.up);
		}
	}

	[ClientRpc]
	public void RpcSpawn(Vector2 V)
	{
		transform.position = V;
		Body.Health = Body.MaxHealth;
		Body.Shield = Body.MaxShield;
	}

	public override Vector2 GetDir()
	{
		if (dir)
			return Vector2.left;
		else
			return Vector2.right;
	}

	public override Vector2 GetLook()
	{
		return Vector2.zero;
	}

	public override int GetSlotA()
	{
		return 0;
	}

	public override int GetSlotD()
	{
		return 0;
	}

	public override int GetSlotP()
	{
		return 0;
	}

	public override int GetSlotW()
	{
		return 0;
	}

	public override bool isActivate()
	{
		return false;
	}

	public override bool isDropping()
	{
		return false;
	}

	public override bool isInteracting()
	{
		return false;
	}

	public override bool isShooting()
	{
		return false;
	}

	public override bool isShootingSide()
	{
		return false;
	}

	public override void OnDrop()
	{

	}
}