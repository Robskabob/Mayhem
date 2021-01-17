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
	public bool Interacting;
	public bool Dropping;

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
				if (Input.GetKey(KeyCode.LeftShift))
					Slot -= 1;
				else
					Slot += 1;

				if (Slot >= Body.Equipment.Count)
					Slot = 0;
				else if (Slot < 0)
					Slot = Body.Equipment.Count-1;
				CmdSlot(Slot);
			}

			if (Shooting != Input.GetMouseButton(0))
			{
				Shooting = Input.GetMouseButton(0);
				CmdShoot(Shooting);
			}

			if (Interacting != Input.GetKey(KeyCode.E))
			{
				Interacting = Input.GetKey(KeyCode.E);
				CmdInteract(Interacting);
			}

			if (Dropping != Input.GetKey(KeyCode.Q))
			{
				Dropping = Input.GetKey(KeyCode.Q);
				CmdDrop(Dropping);
			}

			if (Time.time / updates > 1)
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
	[Command]
	public void CmdInteract(bool Interact)
	{
		Interacting = Interact;
		RpcInteract(Interact);
	}
	[ClientRpc(excludeOwner = true)]
	public void RpcInteract(bool Interact)
	{
		Interacting = Interact;
	}
	[Command]
	public void CmdDrop(bool Drop)
	{
		Dropping = Drop;
		RpcInteract(Drop);
	}
	[ClientRpc(excludeOwner = true)]
	public void RpcDrop(bool Drop)
	{
		Dropping = Drop;
	}


	public override Vector2 GetDir()
	{
		return Dir;
	}

	public override Vector2 GetLook()
	{
		return Look;
	}

	public override int GetSlot()
	{
		return Slot;
	}

	public override bool isShooting()
	{
		return Shooting;
	}
	public override bool isInteracting()
	{
		return Interacting;
	}
	public override bool isDropping()
	{
		return Dropping;
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
