using Mirror;
using System;
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

	public void AddMessage(Message M)
	{
		PlayerClient.PC.Chat.Messages.Add(M);
		PlayerClient.PC.Chat.Timeing.Add(15);
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

	public string ParseCommand()
	{
		if (!content.StartsWith("/"))
			return "Not Command";
		string[] arr = content.TrimStart('/').Split(' ');
		string Name = arr[0];
		Debug.Log("Command Name: " + Name);

		int id = -1;

		for(int i = 0; i < Registry.Reg.Commands.Count; i++)
		{
			if (Registry.Reg.Commands[i].Name == Name) 
			{
				id = i;
				continue;
			}
		}

		Debug.Log("Command ID: " + id);

		if (id == -1)
		{
			Debug.Log("Not a Command");
			return "Not Command";
		}

		Command C = Registry.Reg.Commands[id];

		return C.Action.Invoke(arr);
	}
}

public class Registry 
{
	public static Registry Reg = new Registry();
	public List<Command> Commands; //make dict?

	Registry() 
	{
		Commands = new List<Command>()
		{
			new Command("Help","Tells you whats up!","Help -command(string)",(par) =>
			{
				string msg = "";
				if(par.Length == 1)
				{
					for(int i = 0; i < Commands.Count; i++)
					{
						msg += Commands[i].Name + " ";
						msg += Commands[i].Discription +"\n";
					}
				}
				else
				{
					for(int i = 0; i < Commands.Count; i++)
					{
						if(Commands[i].Name == par[1])
						{
							msg += Commands[i].Discription +" ";
							msg += Commands[i].Useage + "\n";
							continue;
						}
					}
				}
				return "Done\n" + msg;
				//send to owner;
			}),
			new Command("List","List player names and ids","List",(par) =>
			{
				string msg = "";
				foreach(var v in NetSystem.I.Players)
				{
					msg += v.Value.Name;
					msg += ": " + v.Key + "\n";
				}
				return "Done\n" + msg;
				//send to owner;
			}),
			new Command("SetName","Sets the Name of a Player","SetName PlayerID(uint) Name(string)",(par) =>
			{
				if(par.Length < 3)
					return "Not All Parameters Filled";
				if(uint.TryParse(par[1],out uint id))
					if (NetSystem.I.Players.TryGetValue(id, out NetPlayer NP))
						NP.RpcName(par[2]);
					else
						return "Invalid ID";
				else
					return "Failed to Parse ID";
				return "Done";
			}),
			new Command("SetColor","Sets the Color of a Player","SetColor PlayerID(uint) Color(Color)",(par) =>
			{
				if(par.Length < 3)
					return "Not All Parameters Filled";
				if (ColorUtility.TryParseHtmlString(par[2],out Color C))
					if(uint.TryParse(par[1],out uint id))
						if (NetSystem.I.Players.TryGetValue(id, out NetPlayer NP))
							NP.RpcColor(C);
						else
							return "Invalid ID";
					else
						return "Failed to Parse ID";
				else
					return "Failed to Parse Color";
				return "Done";
			}),
			new Command("Kill","Kills the selected Player","Kill PlayerID(uint)",(par) =>
			{
				if(par.Length < 2)
					return "Not All Parameters Filled";
				if(uint.TryParse(par[1],out uint id))
					if (NetSystem.I.PlayerBrains.TryGetValue(id, out PlayerBrain PB))
						PB.CmdDie();
					else
						return "Invalid ID";
				else
					return "Failed to Parse ID";
				return "Done";
			})
		};
	}
}

public class Command 
{
	public string Name;
	public string Discription;
	public string Useage;

	public Exe Action;

	public Command(string name, string discription, string useage, Exe action)
	{
		Name = name;
		Discription = discription;
		Useage = useage;
		Action = action;
	}

	public delegate string Exe(string[] Parameters);
}