using Mirror;
using System.Linq;
using UnityEngine;

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

	[ClientRpc]
	public void Spawn(Vector2 Pos, uint equipment) 
	{
		Active = false;
		Target = Pos;
		Contents = NetworkIdentity.spawned[equipment].GetComponent<Equipment>();

		time = 10;
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
					if(isServer)
						time = Random.Range(100,30f);
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
				if (time < 0 && isServer)
				{
					Target += Vector2.up * 100;
					Contents = null;
					Active = false;
				}
			}
		}
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
		Spawn(pos,Registry.Reg.SpawnRandomEquipment());
	}

	private void OnTriggerStay2D(Collider2D col)
	{
		if (isServer && Active)
		{
			Mob M = col.GetComponent<Mob>();
			if (M != null && M.B.isInteracting())
			{
				//Contents.Randomize();
				Contents.Pickup(M);
				Contents = null;
				Active = false;
				Target += Vector2.up * 100;
				time = 10;
			}
		}
	}
}