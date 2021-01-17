using Mirror;
using System;
using UnityEngine;

public class Player : NetworkBehaviour 
{

}

public abstract class Brain : NetworkBehaviour
{
	public Mob Body;
	public abstract Vector2 GetDir();
	public abstract Vector2 GetLook();
	public abstract int GetSlot();
	public abstract bool isShooting();
	public abstract bool isInteracting();
	public abstract bool isDropping();
	public abstract void Die();
}
