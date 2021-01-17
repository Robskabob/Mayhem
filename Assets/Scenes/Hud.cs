using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour 
{
	public Image Shield;
	public Image Health;


	public Text Altimeter;
	public Text Slot;
	public Text Stats;

	public Mob Body;

	private void Update()
	{
		Shield.fillAmount = Body.Shield / Body.MaxShield;
		Health.fillAmount = Body.Health / Body.MaxHealth;

		Altimeter.text = (int)Body.transform.position.y +"";
		if (Body.Equipment.Count > 0)
		{
			Slot.text = Body.Equipment[Body.B.GetSlot()].GetType().Name + " - " + Body.B.GetSlot();
			Stats.text = Body.Equipment[Body.B.GetSlot()].PrintStats();
		}
		else
		{
			Slot.text = "No Equipment";
			Stats.text = "No Stats";
		}
	}
}
