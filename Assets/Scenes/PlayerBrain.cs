using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class PlayerBrain : Brain
{
	public Cam Cam;
	public override void OnStartAuthority()
	{
		Cam = FindObjectOfType<Cam>();
		Cam.enabled = true;
		Cam.Target = Body.rb;
	}
	public Vector2 Dir;
	public Vector2 Look;
	public int Slot;
	public bool Shooting;
	public Vector2 vec;

	//public NetPlayer LocalPlayer;
	public uint LocalPlayerID;

	private void Start()
	{
		//if (!NetSystem.I.PlayerBrains.ContainsKey(netId))
		//	NetSystem.I.PlayerBrains.Add(netId,this);
		if (hasAuthority)
		{
			PlayerClient.PC.PB = this;
			PlayerClient.PC.OnStartClient();
		}
	}

	public void Update()
	{
		if (Dir != vec)
		{
			vec = Dir;
		}

		if (hasAuthority) 
		{
			Vector2 V = Vector2.zero;

			if (Input.GetKey(KeyCode.W))
			{
				V += Vector2.up;
			}
			if (Input.GetKey(KeyCode.A))
			{
				V += Vector2.left;
			}
			if (Input.GetKey(KeyCode.S))
			{
				V += Vector2.down;
			}
			if (Input.GetKey(KeyCode.D))
			{
				V += Vector2.right;
			}

			if(Dir != V) 
			{
				Dir = V;
				CmdDir(V);
			}

			Vector2 lookPos = Cam.Camera.ScreenToWorldPoint(Input.mousePosition); 
			if (Look != lookPos)
			{
				Look = lookPos;
				CmdLook(lookPos);
			}

			if (Input.GetKeyDown(KeyCode.Tab))
			{
				Slot += 1;
				if (Slot >= Body.Equipment.Count)
					Slot = 0;
				CmdSlot(Slot);
			}

			if (Shooting != Input.GetMouseButton(0))
			{
				Shooting = Input.GetMouseButton(0);
				CmdShoot(Shooting);
			}

			if(Time.time / updates > 1)
			{
				updates++;
				//Debug.Log(Time.time / (updates-1));
				CmdPos(transform.position, Body.rb.velocity);
			}
		}
	}

	int updates;

	[Command]
	public void CmdPos(Vector2 pos, Vector2 vel)
	{
		transform.position = pos;
		Body.rb.velocity = vel;
		RpcPos(pos,vel);
	}
	[ClientRpc(excludeOwner = true)]
	public void RpcPos(Vector2 pos, Vector2 vel)
	{
		transform.position = pos;
		Body.rb.velocity = vel;
	}
	[Command]
	public void CmdDir(Vector2 dir)
	{
		Dir = dir;
		RpcDir(dir);
	}
	[ClientRpc(excludeOwner = true)]
	public void RpcDir(Vector2 dir)
	{
		Dir = dir;
	}
	[Command]
	public void CmdLook(Vector2 look)
	{
		Look = look;
		RpcLook(look);
	}
	[ClientRpc(excludeOwner = true)]
	public void RpcLook(Vector2 look)
	{
		Look = look;
	}
	[Command]
	public void CmdSlot(int slot)
	{
		Slot = slot;
		RpcSlot(slot);
	}
	[ClientRpc(excludeOwner = true)]
	public void RpcSlot(int slot)
	{
		Slot = slot;
	}
	[Command]
	public void CmdShoot(bool shoot)
	{
		Shooting = shoot;
		RpcShoot(shoot);
	}
	[ClientRpc(excludeOwner = true)]
	public void RpcShoot(bool shoot)
	{
		Shooting = shoot;
	}


	public override Vector2 GetDir()
	{
		return Dir;
	}

	public override Vector2 GetLook()
	{
		return Look;
	}

	public override int GetSlot(int Current)
	{
		return Slot;
	}

	public override bool isShooting()
	{
		return Shooting;
	}

	public override void Die()
	{
		if (hasAuthority)
			CmdDie();
	}
	[Command]
	public void CmdDie() 
	{
		RpcSpawn(Physics2D.Raycast((Random.insideUnitCircle + Vector2.up) * 50 ,Vector2.down).point + Vector2.up);
	}
	[ClientRpc]
	public void RpcSpawn(Vector2 V) 
	{
		transform.position = V;
		Body.Health = Body.MaxHealth;
		Body.Shield = Body.MaxShield;
	}
}
