using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using L33t.Equipment;
using System;
using Mirror;
using L33t.Network;
using System.Linq;
using Random = UnityEngine.Random;

//[ExecuteAlways]
public class ItemDrop : NetworkBehaviour
{
    public Transform ItemBox;
    public Equipment Item;
    public SpriteRenderer Drop;
    public SpriteRenderer Box;
    public Collider2D Trigger;
    public Sprite wep;
    public Sprite sec;
    public Sprite act;
    public int Phase;
    public float Eta;
    public long EtaTime;

    void Start()
    {
        if(isServer)
		{
            NetworkServer.Spawn(gameObject);
		}
    }

    private void OnEnable()
    {
        //Debug.Log($"UtcNow:{DateTime.UtcNow.Ticks} UtcNow+1:{DateTime.UtcNow.AddSeconds(1).Ticks} Diff{DateTime.UtcNow.AddMilliseconds(1).Ticks - DateTime.UtcNow.Ticks}Diffsec{DateTime.UtcNow.AddSeconds(1).Ticks - DateTime.UtcNow.Ticks}");
        //Debug.Log($"tick: {DateTime.UtcNow.Ticks} file: {DateTime.UtcNow.ToFileTimeUtc()} out{DateTime.FromFileTimeUtc(DateTime.UtcNow.ToFileTimeUtc())} diff{DateTime.UtcNow.Ticks-DateTime.UtcNow.ToFileTimeUtc()}");
    }
	public void UpdatePhase(int phase, float Time)
	{
        Phase = phase;
        Eta = Time;
        EtaTime = DateTime.UtcNow.Ticks + (long)(Time * 10000000);//1sec = 10000000
        if (isServer)
            RpcUpdateTime(EtaTime, Phase);
    }
    [ClientRpc]
    public void RpcUpdateTime(long eta,int phase) 
    {
        EtaTime = eta;
        Eta = (eta - DateTime.UtcNow.Ticks) / 10000000.0f;
        Phase = phase;
    }
    
    [ClientRpc]
    public void RpcGetItem(long eta,int phase) 
    {
        EtaTime = eta;
        Eta = (eta - DateTime.UtcNow.Ticks) / 10000000.0f;
        Phase = phase;
    }
    [Server]
    public void Randomize(Vector2 pos)
    {
        transform.position = pos;
        UpdatePhase(0,Random.Range(8, 12f));
        Drop.enabled = true;
        if (Item == null) 
        {
            SetItem(Registry.Reg.SpawnRandomEquipment());
        }
        else
            Item.Randomize();
    }

    [ClientRpc]
    public void SetItem(uint netId)
    {
    
        Item = NetworkIdentity.spawned[netId].GetComponent<Equipment>();
        Item.PickUpAble = false;
        Item.transform.parent = Box.transform;
        Item.transform.localPosition = Vector3.up;
        switch (Item)
        {
            case WeaponEquipment _:
                Box.sprite = wep;
                break;
            case DirectedEquipment _:
                Box.sprite = sec;
                break;
            case ActiveEquipment _:
                Box.sprite = act;
                break;
            case PasiveEquipment _:
                Debug.LogError("there aren't any passives");
                break;
        }
    }
    void Update()
    {
        switch (Phase)
        {
            case 0:
                Eta -= Time.deltaTime;
                if (Eta < 0)
                {
                    Trigger.offset = new Vector3(0, 0, -.1f);
                    ItemBox.localPosition = new Vector3(0, 0, -.1f);
                    UpdatePhase(1,30);
                    Drop.enabled = false;
                    break;
                }
                Trigger.offset = new Vector3(0, Eta * Eta / 5, -.1f);
                ItemBox.localPosition = new Vector3(0, Eta * Eta / 5, -.1f);
                break;
            case 1:
                Eta -= Time.deltaTime;
                if (Eta < 0)
                {
                    UpdatePhase(2, 10);
                }
                break;
            case 2:
                Eta -= Time.deltaTime;
                if (Eta < 0)
                {
                    if (isServer) 
                    {

                        Vector2 rel;
                        if (NetSystem.I.PlayerBrains.Count > 0)
                            rel = NetSystem.I.PlayerBrains.ElementAt(Random.Range(0, NetSystem.I.PlayerBrains.Count)).Value.transform.position;
                        else
                            rel = Vector2.zero;

                        RaycastHit2D hit = Physics2D.Raycast(Random.insideUnitCircle * 100 + rel, Vector2.down,1<<9);
                        if (hit.distance < 3 || hit.point == Vector2.zero)
                        {
                            break;
                        }
                        Randomize(hit.point);
                    }
                    break;
                }
                Trigger.offset = new Vector3(0, (10 - Eta) * (10 - Eta) / 5, -.1f);
                ItemBox.localPosition = new Vector3(0, (10-Eta) * (10-Eta) / 5, -.1f);
                break;
        }
    }
    private void OnTriggerStay2D(Collider2D col)
    {
        Mob M = col.GetComponent<Mob>();
        if (M != null && Item != null && M.B.isInteracting())
        {
			switch (Phase) 
            {
                case 0:
                    UpdatePhase(2,10 - Eta);
                    break;
                case 1:
                    UpdatePhase(2,10);
                    break;
                case 2:
                    return;
            }
            Drop.enabled = false;
            if (isServer)
            {
                //Contents.Randomize();
                if (!Item.Pickup(M))
                    return;
                Item = null;
            }
        }
    }
}
