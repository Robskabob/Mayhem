using System;
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

	private void Start()
	{
		gameObject.SetActive(false);
	}

	private void Update()
	{
		Shield.fillAmount = Body.Shield / Body.MaxShield;
		Health.fillAmount = Body.Health / Body.MaxHealth;

		Altimeter.text = (int)Body.transform.position.y +"";
		Slot.text = "";
		Stats.text = "";
		if (Body.WeaponEquipment.Count > 0)
		{
			Slot.text += Body.WeaponEquipment[Body.B.GetSlotW()].GetType().Name + " - " + (Body.B.GetSlotW() + 1) + "/" + Body.WeaponEquipment.Count + "\n";
			Stats.text += Body.WeaponEquipment[Body.B.GetSlotW()].PrintStats() + "\n\n";
		}
		else
		{
			Slot.text += "No Equipment\n";
			Stats.text += "No Stats\n";
		}
		if (Body.DirectedEquipment.Count > 0)
		{
			Slot.text += Body.DirectedEquipment[Body.B.GetSlotD()].GetType().Name + " - " + (Body.B.GetSlotD() + 1) + "/" + Body.DirectedEquipment.Count + "\n";
			Stats.text += Body.DirectedEquipment[Body.B.GetSlotD()].PrintStats() + "\n\n";
		}
		else
		{
			Slot.text += "No Grapple\n";
		}
		if (Body.ActiveEquipment.Count > 0)
		{
			Slot.text += Body.ActiveEquipment[Body.B.GetSlotA()].GetType().Name + " - " + (Body.B.GetSlotA() + 1) + "/" + Body.ActiveEquipment.Count + "\n";
			Stats.text += Body.ActiveEquipment[Body.B.GetSlotA()].PrintStats() + "\n\n";
		}
		else
		{
			Slot.text += "No Active\n";
		}
		if (Body.PassiveEquipment.Count > 0)
		{
			Slot.text += Body.PassiveEquipment[Body.B.GetSlotP()].GetType().Name + " - " + (Body.B.GetSlotP() + 1) + "/" + Body.PassiveEquipment.Count + "\n";
			Stats.text += Body.PassiveEquipment[Body.B.GetSlotP()].PrintStats();
		}
		else
		{
			Slot.text += "No Passive";
		}
	}
}
