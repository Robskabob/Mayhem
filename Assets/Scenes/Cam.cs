using UnityEngine;

public class Cam : MonoBehaviour 
{
	public Camera Camera;
	public Rigidbody2D Target;

	public float Drag = .2f;
	public float Power = .2f;

	public float Forward = 1;

	public Vector2 Velocity;
	public Vector2 Acceleration;
	private void FixedUpdate()
	{
		//Vector2 offset = (Target.position - transform.position);
		//float mag = offset.magnitude;
		//offset *= mag;
		//if(mag > .001f)
		//	Acceleration += offset * Power * Time.fixedDeltaTime;
		//Acceleration /= 1 + (Drag * Time.fixedDeltaTime);
	}
    void OnPreRender()
	{
		Vector2 offset = (Target.position + Target.velocity * Forward) - (Vector2)transform.position;
		float mag = offset.magnitude;
		offset *= mag;
		if (mag > .001f)
			Acceleration += offset * Power * Time.deltaTime;
		Acceleration /= 1 + (Drag * Time.deltaTime);


		Velocity += Acceleration * Time.deltaTime;
		Velocity /= 1 + (Drag * Time.deltaTime);
		transform.position += (Vector3)(Velocity * Time.deltaTime);
	}
}
