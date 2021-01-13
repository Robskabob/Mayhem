using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour 
{

}

public abstract class Brain : NetworkBehaviour
{
	public Mob Body;
	public abstract Vector2 GetDir();
	public abstract Vector2 GetLook();
	public abstract int GetSlot(int Current);
	public abstract bool isShooting();
}
