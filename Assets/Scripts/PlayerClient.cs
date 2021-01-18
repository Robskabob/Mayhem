using UnityEngine;

public class PlayerClient : MonoBehaviour
{
	public static PlayerClient PC;
	public PlayerBrain PB;
	public NetPlayer NP;
	public ColorPicker CP;
	public Chat Chat;
	public Hud HUD;

	private void Start()
	{
		PC = this;
	}

	public void OnStartClient() 
	{
		HUD.gameObject.SetActive(true);
		HUD.Body = PB.Body;
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Tilde))
		{
			Chat.gameObject.SetActive(!Chat.gameObject.activeSelf);
		}
	}
}
