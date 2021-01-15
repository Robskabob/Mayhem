using UnityEngine;
using UnityEngine.UI;

public class Altimeter : MonoBehaviour 
{
	public Text Text;

	public void Update()
	{
		if(PlayerClient.PC.PB != null)
		Text.text = (int)PlayerClient.PC.PB.transform.position.y+"";
	}
}