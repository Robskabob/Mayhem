using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour 
{
	public Image Shield;
	public Image Health;

	public Mob Body;

	private void Update()
	{
		Shield.fillAmount = Body.Shield / Body.MaxShield;
		Health.fillAmount = Body.Health / Body.MaxHealth;
	}
}
