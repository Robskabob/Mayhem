using UnityEngine;

public class PlayerClient : MonoBehaviour
{
	public static PlayerClient PC;
	public PlayerBrain PB;
	public NetPlayer NP;
	public ColorPicker CP;
	public Chat Chat;

	private void Start()
	{
		PC = this;
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.T))
		{
			Chat.gameObject.SetActive(!Chat.gameObject.activeSelf);
		}
	}
}
