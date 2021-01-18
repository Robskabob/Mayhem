using Mirror;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemBox : NetworkBehaviour 
{
	public Equipment Contents;
	public SpriteRenderer SR;
	public Vector2 Target;
	public float time;
	public bool Active;
	public Color SpawnColor;
	public Color LandColor;
	public Color AgeColor;

	public override void OnStartClient()
	{
		GetState();
	}

	[Command]
	public void GetState() 
	{
		uint id = 0;
		if (Contents != null)
			id = Contents.netId;
		RpcGetState(id,Target,Active,time,DateTime.UtcNow.ToFileTimeUtc());
	}

	[ClientRpc]
	public void RpcGetState(uint EqId, Vector2 target, bool active,float Time,long fileTime)
	{
		time = Time - (float)(DateTime.UtcNow - DateTime.FromFileTimeUtc(fileTime)).TotalSeconds;
		Active = active;
		Target = target;
		if(EqId != 0)
			Contents = NetworkIdentity.spawned[EqId].GetComponent<Equipment>();
	}

	[ClientRpc]
	public void Spawn(Vector2 Pos, uint equipment) 
	{
		if(time < 0) 
		{
			time = 10;
		}
		Active = false;
		Target = Pos;
		if(Contents != null)
		{
			NetworkServer.Destroy(Contents.gameObject);
		}
		Contents = NetworkIdentity.spawned[equipment].GetComponent<Equipment>();
		//Debug.Log("Contents is " + Contents != null);

		transform.position = Pos + Vector2.up * 100;
	}

	private void Update()
	{
		time -= Time.deltaTime;
		if (!Active) 
		{
			if (Contents == null)
			{
				if(time > 5)
					SR.color = Color.Lerp(SR.color, AgeColor, Time.deltaTime);
				else
					SR.color = Color.Lerp(SpawnColor, AgeColor,(time / 5));
				transform.position = Vector2.Lerp(Target - Vector2.up * 100, Target ,(10 - time) * (10 - time) / 100);
				if (time < 0 && isServer)
					Randomize();
			}
			else
			{
				SR.color = Color.Lerp(LandColor, SpawnColor, ((time - 5) / 5));
				transform.position = Vector2.Lerp(Target, Target + Vector2.up * 100,time * time / 100);
				if (time < 0) 
				{
					Active = true;
					transform.position = Target;
					if (isServer)
					{
						time = Random.Range(100, 30f);
						SetTime(time);
					}
					else 
					{
						time = 60;
					}
				}
			}
		}
		else
		{
			SR.color = Color.Lerp(AgeColor, LandColor, time / 20);
			if (Contents == null)
			{
				Active = false;
			}
			else
			{
				if (time < 0)
				{
					Target += Vector2.up * 100;
					time = 10;
					Active = false;
					Contents.transform.position = transform.position;

					if (isServer)
					{
						SetTime(time);
						Contents.Abandand = true;
						Contents.ExpireTime = 20;
					}
					Contents = null;
				}
			}
		}
	}

	public void SetTime(float Time) 
	{
		time = Time;
		RpcSetTime(Time,DateTime.UtcNow.ToFileTimeUtc());
	}
	[ClientRpc]
	public void RpcSetTime(float Time,long fileTime) 
	{
		time = Time - (float)(DateTime.UtcNow - DateTime.FromFileTimeUtc(fileTime)).TotalSeconds;
	}

	[Server]
	public void Randomize() 
	{
		Vector2 rel;
		if (NetSystem.I.PlayerBrains.Count > 0)
			rel = NetSystem.I.PlayerBrains.ElementAt(Random.Range(0, NetSystem.I.PlayerBrains.Count)).Value.transform.position;
		else
			rel = Vector2.zero;
		Vector2 pos = Physics2D.Raycast(Random.insideUnitCircle * 100 + rel, Vector2.down).point;
		if (pos == Vector2.zero)
			return;
		time = Random.Range(8, 12f);
		SetTime(time);
		Spawn(pos,Registry.Reg.SpawnRandomEquipment());
	}

	private void OnTriggerStay2D(Collider2D col)
	{
		if (Active)
		{
			Mob M = col.GetComponent<Mob>();
			if (M != null && M.B.isInteracting())
			{
				time = 10;
				if (isServer)
				{
					//Contents.Randomize();
					if (!Contents.Pickup(M))
						return;
					SetTime(time);
				}
				Contents = null;
				Active = false;
				Target += Vector2.up * 100;
			}
		}
	}
}