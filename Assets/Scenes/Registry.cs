using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class Registry 
{
	public static Registry Reg = new Registry();
	public List<Command> Commands; //make dict?
	public List<Equipment> Equipment; //make dict?

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
				string str;
				if(par.Length == 1)
				{
					str = "Players";
				}
				else
				{
					str = par[1];
				}

				switch(str)
				{
					case "Help":
						msg += "Help\n" +
							"Players\n" +
							"All\n" +
							"Properties\n";
						break;

					case "Players":
						foreach(var v in NetSystem.I.Players)
						{
							msg += v.Value.Name;
							msg += ": " + v.Key + "\n";
						}
						break;

					case "All":
						foreach(var v in NetworkIdentity.spawned)
						{
							msg += v.Value.name;
							msg += ": " + v.Key + "\n";
						}
						break;

					default:
						msg += str + " is not an option try Help";
						break;
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
			}),
			new Command("Tp","Teleports the selected Player","TP PlayerID(uint) {[PlayerID(uint)][x(float) y(float)]}",(par) =>
			{
				if(par.Length < 3)
					return "Not All Parameters Filled";
				if(!uint.TryParse(par[1],out uint id))
					return "Failed to Parse ID";
				if (!NetSystem.I.PlayerBrains.TryGetValue(id, out PlayerBrain PB))
						return "Invalid ID";

				if(par.Length > 3)
				{
					if (!float.TryParse(par[2], out float x))
						return "Failed to Parse float x";
					if (!float.TryParse(par[3], out float y))
						return "Failed to Parse float y";

					PB.CmdPos(new Vector2(x,y),Vector2.zero);
				}
				else
				{
					if(!uint.TryParse(par[1],out uint id2))
						return "Failed to Parse ID";
					if (!NetSystem.I.PlayerBrains.TryGetValue(id2, out PlayerBrain PB2))
						return "Invalid ID";

					PB.CmdPos(PB2.transform.position,Vector2.zero);
				}

				return "Done";
			}),
			new Command("Stat","Manipulates Stats","SetStat NetID(uint) Option[List{-Option[Help...]},Comp{Component(Type) Loop},Set{Field(string) Value(primitive)}]",StatCommand.StatExe)
		};
	}

	[Server]
	public uint SpawnRandomEquipment()
	{
		Equipment E = Object.Instantiate(Equipment[Random.Range(0, Equipment.Count)]);
		NetworkServer.Spawn(E.gameObject);
		E.Randomize();
		return E.netId;
	}
}
