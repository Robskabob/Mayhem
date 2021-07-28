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
	public int SlotW;
	public int SlotD;
	public int SlotA;
	public int SlotP;
	public bool Shooting;
	public bool ShootingSide;
	public bool Activate;
	public bool Interacting;
	public bool Dropping;
	public bool Shift;
	public bool ALT;
	public bool CTRL;

	public bool Hiding;

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

	public void BodySwap(Mob body) 
	{
		if (!Hiding)
		{
			Hiding = true;
			Body.rb.constraints = RigidbodyConstraints2D.FreezeAll;
			GetComponent<SpriteRenderer>().enabled = false;
			GetComponent<Collider2D>().isTrigger = true;
			GetComponent<Mob>().enabled = false;

		}
		//Cam.Target = body.rb;
		transform.SetParent(body.transform);
		transform.localPosition = Vector3.zero;
		Brain b = body.B;
		b.enabled = false;
		body.B = this;
		Body.B = b;
		b.Body = body;
		Body = body;
	}

	public void Update()
	{
		//if (Hiding) 
		//{
		//	transform.localPosition = Vector3.zero;
		//}
		//if (Input.GetKeyDown(KeyCode.G)) 
		//{
		//	Collider2D[] cols = Physics2D.OverlapCircleAll(Cam.Camera.ScreenToWorldPoint(Input.mousePosition),5);
		//	for(int i = 0; i < cols.Length; i++) 
		//	{
		//		Mob M = cols[i].GetComponent<Mob>();
		//		if (M == null)
		//			continue;
		//		if (M == Body)
		//			continue;
		//		BodySwap(M);
		//		break;
		//	}
		//}

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
					SlotW -= 1;
				else
					SlotW += 1;

				if (SlotW >= Body.WeaponEquipment.Count)
					SlotW = 0;
				else if (SlotW < 0)
					SlotW = Body.WeaponEquipment.Count-1;
				CmdSlot(SlotW,1);
			}
			int slot = -1;
			if (Input.GetKeyDown(KeyCode.Alpha1)) slot = 0;
			else if (Input.GetKeyDown(KeyCode.Alpha2)) slot = 1;
			else if (Input.GetKeyDown(KeyCode.Alpha3)) slot = 2;
			else if (Input.GetKeyDown(KeyCode.Alpha4)) slot = 3;
			else if (Input.GetKeyDown(KeyCode.Alpha5)) slot = 4;
			else if (Input.GetKeyDown(KeyCode.Alpha6)) slot = 5;
			else if (Input.GetKeyDown(KeyCode.Alpha7)) slot = 6;
			else if (Input.GetKeyDown(KeyCode.Alpha8)) slot = 7;
			else if (Input.GetKeyDown(KeyCode.Alpha9)) slot = 8;
			else if (Input.GetKeyDown(KeyCode.Alpha0)) slot = 9;

			if(slot != -1)
			{
				if (Input.GetKey(KeyCode.LeftShift) && slot < Body.DirectedEquipment.Count)
					SlotD = slot;
				else if (Input.GetKey(KeyCode.LeftAlt) && slot < Body.ActiveEquipment.Count)
					SlotA = slot;
				else if (Input.GetKey(KeyCode.LeftControl) && slot < Body.PassiveEquipment.Count)
					SlotP = slot;
				else if (slot < Body.WeaponEquipment.Count)
					SlotW = slot;
			}

			if (Shooting != Input.GetMouseButton(0))
			{
				Shooting = Input.GetMouseButton(0);
				CmdShoot(Shooting);
			}

			if (ShootingSide != Input.GetMouseButton(1))
			{
				ShootingSide = Input.GetMouseButton(1);
				CmdShootS(ShootingSide);
			}

			if (Activate != Input.GetKey(KeyCode.Space))
			{
				Activate = Input.GetKey(KeyCode.Space);
				CmdActivate(Activate);
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

			if (Shift != Input.GetKey(KeyCode.LeftShift))
			{
				Shift = Input.GetKey(KeyCode.LeftShift);
				CmdMod(Shift, 2);
			}
			if (ALT != Input.GetKey(KeyCode.LeftAlt))
			{
				ALT = Input.GetKey(KeyCode.LeftAlt);
				CmdMod(ALT, 3);
			}
			if (CTRL != Input.GetKey(KeyCode.LeftControl))
			{
				CTRL = Input.GetKey(KeyCode.LeftControl);
				CmdMod(CTRL, 4);
			}

			if (Time.time / updates > 1)
			{
				updates++;
				//Debug.Log(Time.time / (updates-1));
				CmdPos(transform.position, Body.rb.velocity);
			}
		}
	}
	public override void OnDrop()
	{
		if (SlotW >= Body.WeaponEquipment.Count)
			SlotW = 0;
		if (SlotD >= Body.DirectedEquipment.Count)
			SlotD = 0;
		if (SlotA >= Body.ActiveEquipment.Count)
			SlotA = 0;
		if (SlotP >= Body.PassiveEquipment.Count)
			SlotP = 0;
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
	public void CmdSlot(int slot, int n)
	{
		switch (n)
		{
			case 1:
				SlotW = slot;
				break;
			case 2:
				SlotD = slot;
				break;
			case 3:
				SlotA = slot;
				break;
			case 4:
				SlotP = slot;
				break;
		}
		RpcSlot(slot, n);
	}
	[ClientRpc(excludeOwner = true)]
	public void RpcSlot(int slot, int n)
	{
		switch (n)
		{
			case 1:
				SlotW = slot;
				break;
			case 2:
				SlotD = slot;
				break;
			case 3:
				SlotA = slot;
				break;
			case 4:
				SlotP = slot;
				break;
		}
	}
	[Command]
	public void CmdMod(bool state, int n)
	{
		switch (n)
		{
			case 2:
				Shift = state;
				break;
			case 3:
				ALT = state;
				break;
			case 4:
				CTRL = state;
				break;
		}
		RpcMod(state, n);
	}
	[ClientRpc(excludeOwner = true)]
	public void RpcMod(bool state, int n)
	{
		switch (n)
		{
			case 2:
				Shift = state;
				break;
			case 3:
				ALT = state;
				break;
			case 4:
				CTRL = state;
				break;
		}
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
	public void CmdShootS(bool shoot)
	{
		ShootingSide = shoot;
		RpcShootS(shoot);
	}
	[ClientRpc(excludeOwner = true)]
	public void RpcShootS(bool shoot)
	{
		ShootingSide = shoot;
	}
	[Command]
	public void CmdActivate(bool ac)
	{
		Activate = ac;
		RpcActivate(ac);
	}
	[ClientRpc(excludeOwner = true)]
	public void RpcActivate(bool ac)
	{
		Activate = ac;
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
		RpcDrop(Drop);
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

	public override int GetSlotW()
	{
		return SlotW;
	}
	public override int GetSlotD()
	{
		return SlotD;
	}
	public override int GetSlotA()
	{
		return SlotA;
	}
	public override int GetSlotP()
	{
		return SlotP;
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

	public override bool isShootingSide()
	{
		return ShootingSide;
	}

	public override bool isActivate()
	{
		return Activate;
	}
}
