using UnityEngine;

public class Projectile : MonoBehaviour 
{
	public Rigidbody2D rb;
	public Vector2 Dir;
	public Mob Owner;
	public float Health;

	public ProjectileData Data;
	public float LifeTime;

	public void Shoot(Mob owner,Vector2 dir ,ProjectileData data) 
	{
		GetComponent<SpriteRenderer>().color = owner.GetComponent<SpriteRenderer>().color;
		Owner = owner;
		LifeTime = data.LifeTime;
		Health = data.Health;
		Data = data;
		Dir = dir.normalized;
		transform.right = Dir;
		transform.position = owner.transform.position + (Vector3)Dir;
		rb.AddForce(data.Impulse*Dir);
	}

	private void FixedUpdate()
	{
		LifeTime -= Time.fixedDeltaTime;
		rb.AddForce(Data.Speed * Dir * Time.fixedDeltaTime);
		if (LifeTime < 0 || Health < 0 || (LifeTime < (Data.LifeTime-.5f) && rb.velocity.magnitude < 1f))
			Destroy(gameObject);
	}

	private void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.GetComponent<Mob>() is Mob M)
		{
			M.Dammage(Data.Dammage);
			LifeTime--;
		}
	}
	private void OnCollisionStay2D(Collision2D col)
	{
		if (col.gameObject.GetComponent<Mob>() is Mob M)
		{
			LifeTime--;
		}

		if (col.gameObject.GetComponent<Projectile>() is Projectile P)
		{
			if(P.Owner != Owner)
				P.Health -= Data.Dammage;
		}
	}
}

//[CreateAssetMenu(fileName = "Projectile Data", menuName = "Game Data/Projectile Data")]
[System.Serializable]
public class ProjectileData 
{
	public float LifeTime;
	public float Impulse;
	public float Speed;
	public float Dammage;
	public float Health;

	public ProjectileData Clone() 
	{
		ProjectileData PD = new ProjectileData();
		PD.LifeTime = LifeTime;
		PD.Impulse = Impulse;
		PD.Speed = Speed;
		PD.Dammage = Dammage;
		PD.Health = Health;
		return PD;
	}
}