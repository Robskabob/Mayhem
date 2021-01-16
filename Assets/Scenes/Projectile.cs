using UnityEngine;

public class Projectile : MonoBehaviour 
{
	public Rigidbody2D rb;
	public Vector2 Dir;
	public Mob Owner;

	public ProjectileData Data;
	public float LifeTime;

	public void Shoot(Mob owner,Vector2 dir ,ProjectileData data) 
	{
		Owner = owner;
		LifeTime = data.LifeTime;
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
		if (LifeTime < 0 || (LifeTime < (Data.LifeTime*.9f) && rb.velocity.magnitude < .5f))
			Destroy(gameObject);
	}

	private void OnCollisionEnter2D(Collision2D col)
	{
		if(col.gameObject.GetComponent<Mob>() is Mob M) 
		{
			M.Dammage(Data.Dammage);
			LifeTime--;
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
}