using System.Collections.Generic;
using Mirror;
using UnityEngine;
using L33t.Equipment;
using L33t.UI;
using L33t.Network;
using L33t;

public abstract class GameMode
{
	public void LoadGame() { }
	public void UnLoadGame() { }
	public void OnPlayerJoin() { }
	public void OnPlayerLeave() { }
	public void OnPlayerDeath() { }
	public void OnPlayerKill() { }
}