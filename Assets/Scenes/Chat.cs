using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour 
{
	public List<Message> Messages;
	public List<float> Timeing;

	public InputField IF;
	public Text text;

	private void Start()
	{
		IF.onEndEdit.AddListener((v) => PlayerClient.PC.NP.CmdChat(v));
	}

	private void Update()
	{
		string chat = "";
		for (int i = 0; i < Messages.Count; i ++) 
		{
			chat += Messages[i].GenerateText();
			chat += "\n";
			if(Messages.Count > 5) 
			{
				Timeing[i]-=Time.deltaTime;
				if(Timeing[i] < 0)
				{
					Messages.RemoveAt(i);
					Timeing.RemoveAt(i);
					i--;
				}
			}
		}
		text.text = chat;
	}
}

[System.Serializable]
public struct Message 
{
	public string user;
	public Color color;
	public string content;

	public Message(string user, Color color, string content)
	{
		this.user = user;
		this.color = color;
		this.content = content;
	}

	public string GenerateText() 
	{
		return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>({user}) </color>{content}";
	}
}