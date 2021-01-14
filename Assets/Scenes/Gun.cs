using UnityEngine;

public class Gun : Equipment
{
	public float ReloadTime;
	public float FireTime;
	public float waitTime;

	public float Clip;
	public float clip;

	public float Impulse;
	public float Life;
	public float Speed;

	public Projectile Projectile;
	public ProjectileData ProjectileData;

	public Mob Holder;
	public override void Drop()
	{
		transform.parent = null;
		Holder = null;
	}

	public override void Pickup(Mob M)
	{
		transform.parent = M.transform;
		Holder = M;
	}

	public override void Use(Vector2 Pos)
	{
		if(waitTime < 0) 
		{
			if(clip <= 0) 
			{
				waitTime = ReloadTime;
				clip = Clip;
			}
			else 
			{
				waitTime = FireTime;
				clip--;
				Fire(Pos);
			}
		}
	}

	void Fire(Vector2 Pos) 
	{
		Projectile P = Instantiate(Projectile);
		P.Shoot(Holder,Pos-(Vector2)transform.position,ProjectileData);
	}

	private void Update()
	{
		waitTime -= Time.deltaTime;
	}
}
