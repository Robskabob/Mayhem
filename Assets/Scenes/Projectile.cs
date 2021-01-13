using UnityEngine;

public class Projectile : MonoBehaviour 
{
	public Rigidbody2D rb;
	public float LifeTime;
	public float Speed;
	public Vector2 Dir;
	public Mob Owner;

	public void Shoot(Mob owner,Vector2 dir ,float impulse, float life,float speed) 
	{
		Owner = owner;
		LifeTime = life;
		Speed = speed;
		Dir = dir.normalized;
		transform.right = Dir;
		transform.position = owner.transform.position + (Vector3)Dir;
		rb.AddForce(impulse*Dir);
	}

	private void FixedUpdate()
	{
		LifeTime -= Time.fixedDeltaTime;
		rb.AddForce(Speed * Dir * Time.fixedDeltaTime);
		if (LifeTime < 0)
			Destroy(gameObject);
	}
}