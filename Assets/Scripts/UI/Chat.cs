using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using System.ComponentModel;
using L33t.Network;
using L33t.Equipment;

public class Chat : MonoBehaviour 
{
	public List<Message> Messages;
	public List<float> Timeing;

	public InputField IF;
	public Text text;

	private void Start()
	{
		IF.onEndEdit.AddListener((v) => PlayerClient.PC.NP.CmdChat(v));
		//Debug.Log(typeof(PlayerBrain).AssemblyQualifiedName +"\n"+ typeof(GrappleGun).AssemblyQualifiedName);
		StatCommand.Start();
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
			chat += "\n";
			chat += Messages[i].GenerateText();
			if(text.cachedTextGenerator.lineCount > 70) 
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

public static class StatCommand 
{
	public static void Start()
	{
		NetworkClient.RegisterHandler<SetStatMessage>(OnStatSet);
	}
	public static string StatExe(string[] par)
	{
		if (par.Length < 3)
			return "Not All Parameters Filled";
		if (!uint.TryParse(par[1], out uint id))
			return "Failed to Parse ID";
		if (!NetworkIdentity.spawned.TryGetValue(id, out NetworkIdentity NI))
			return "Invalid ID";

		string msg = "";

		if (Step(par, 2,id, NI, ref msg)) 
			return msg + "Done";
		else
			return msg;
	}

	public static bool Step(string[] par, int depth,uint id, UnityEngine.Component Comp, ref string msg) 
	{
		switch (par[depth])
		{
			case "List":
				msg += PrintStats(Comp,par,depth+1);
				break;
			case "Comp":
				if (par.Length < depth + 2) 
				{
					msg = "Missing Parameter at "+par.Length;
					return false;
				}
				UnityEngine.Component Com = GetComponent(Comp,par[depth + 1],ref msg);
				if (Com == null)
				{
					msg = "Component Not Found";
					return false;
				}
				return Step(par,depth+2,id,Com,ref msg);
			case "Set":
				if (par.Length < depth + 3)
				{
					msg = "Missing Parameter at " + par.Length;
					return false;
				}
				return SetStat(id,Comp,par[depth + 1],par[depth + 2],ref msg);
			default:
				msg = "Not an Option";
				return false;
		}
		return true;
	}

	public static bool SetStat(uint NetId, UnityEngine.Component Comp, string Field, string value, ref string msg) 
	{
		Type T = Comp.GetType();
		FieldInfo FI = T.GetField(Field);
		Type FT = FI.FieldType;

		object o = Convert.ChangeType(value,FT);
		//Debug.Log(o);


		FI.SetValue(Comp, o);
		//Debug.Log(FI.Name + " " + FT + " " + FI.GetValue(Comp));
		//Debug.Log(FT.GetMethod("TryParse",BindingFlags.Public | BindingFlags.Static,null,CallingConventions.Any,new Type[] {FT,FT.MakeByRefType()},null).Invoke(FT.TypeInitializer.Invoke(null), new object[]{value,FT.TypeInitializer.Invoke(null)}));

		NetworkServer.SendToAll(new SetStatMessage(NetId,Comp.GetType().Name,Field,value));
		return true;
	}

	public static UnityEngine.Component GetComponent(UnityEngine.Component com, string type, ref string msg) 
	{
		Type T = GetType(type);
		if(T == null) 
		{
			msg = "Not a Type";
			return null;
		}
		var v = com.GetComponent(T);
		return v;
	}

	public static string PrintStats(object o, string[] par, int depth)
	{
		Type T = o.GetType();
		string msg = T.FullName + " : " + T.BaseType.Name + "\n";
		Debug.Log(par.Length + " | " + depth);
		string str;
		if (par.Length <= depth)
			str = "Fields";
		else
			str = par[depth];

		switch (str)
		{
			case "Help":
				msg += "Help\n" +
					"Fields\n" +
					"Members\n" +
					"Properties\n";
				break;
			case "Fields":
				msg += "Fields \n";
				FieldInfo[] FI = T.GetFields();
				for (int i = 0; i < FI.Length; i++)
				{
					msg += FI[i].Name + ": ";
					if (FI[i].FieldType.IsArray)
					{
						System.Collections.IList arr = (System.Collections.IList)FI[i].GetValue(o);

						msg += arr.Count;
						if(arr.Count > 0)
							msg += " [0]" + arr[0] + "\n";
						else
							msg += "\n";
					}
					else
						msg += FI[i].GetValue(o) + "\n";
				}
				break;
			case "Members":
				msg += "Members \n";
				MemberInfo[] MI = T.GetMembers();
				for (int i = 0; i < MI.Length; i++)
				{
					msg += MI[i].Name + ": " + MI[i].MemberType + "\n";
				}
				break;
			case "Properties":
				msg += "Properties \n";
				PropertyInfo[] PI = T.GetProperties();
				for (int i = 0; i < PI.Length; i++)
				{
					msg += PI[i].Name + ": " + PI[i].GetValue(o) + "\n";
				}
				break;
			default:
				msg += str + " is not an option try Help";
				break;
		}
		return msg;
	}

	public static Type GetType(string name)
	{
		Type T = Type.ReflectionOnlyGetType(name + ", Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", false, true);

		switch (name)
		{
			case "PlayerBrain":
				T = typeof(PlayerBrain);
				break;
			case "Mob":
				T = typeof(Mob);
				break;
			case "Gun":
				T = typeof(Gun);
				break;
			case "GrappleGun":
				T = typeof(GrappleGun);
				break;
			case "NetPlayer":
				T = typeof(NetPlayer);
				break;
			case "Projectile":
				T = typeof(Projectile);
				break;
		};
		return T;
	}


	public static void OnStatSet(SetStatMessage Stat)
	{
		NetworkIdentity.spawned.TryGetValue(Stat.NetId,out NetworkIdentity NI);
		Type T = GetType(Stat.type);
		UnityEngine.Component Comp = NI.GetComponent(T);
		FieldInfo FI = T.GetField(Stat.field);
		Debug.Log(FI.Name + " " + FI.FieldType + " " + FI.GetValue(Comp));
		Debug.Log(Stat.value);
		FI.SetValue(Comp, Convert.ChangeType(Stat.value, FI.FieldType));
	}

	public struct SetStatMessage : NetworkMessage
	{
		public uint NetId;
		public string type;
		public string field;
		public string value;

		public SetStatMessage(uint netId, string type, string field, string value)
		{
			NetId = netId;
			this.type = type;
			this.field = field;
			this.value = value;
		}
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
