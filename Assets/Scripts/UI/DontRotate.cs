using UnityEngine;

public class DontRotate : MonoBehaviour
{
	private void Update()
	{
		transform.rotation = Quaternion.identity;
	}
}
