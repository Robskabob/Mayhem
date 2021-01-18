using Mirror;
using System;
using UnityEngine;

public abstract class Brain : NetworkBehaviour
{
	public Mob Body;
	public abstract Vector2 GetDir();
	public abstract Vector2 GetLook();
	public abstract int GetSlotW();
	public abstract int GetSlotD();
	public abstract int GetSlotA();
	public abstract int GetSlotP();
	public abstract bool isShooting();
	public abstract bool isShootingSide();
	public abstract bool isActivate();
	public abstract bool isInteracting();
	public abstract bool isDropping();
	public abstract void OnDrop();
	public abstract void Die(); 
}
