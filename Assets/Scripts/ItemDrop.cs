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
//public class ItemDropController : MonoBehaviour
//{
//    public CullingGroup cull = new CullingGroup();
//    public List<ItemDrop> ItemDrops;
//    public BoundingSphere[] BS;
//    public Vector2 Center;
//	private void OnEnable()
//	{
//        BS = new BoundingSphere[ItemDrops.Count];
//        cull.onStateChanged += w;
//	}
//	private void Update()
//	{
//        for(int i = 0; i < ItemDrops.Count; i++) 
//        {
//            BS[i].position = ItemDrops.
//        }
//        cull.SetBoundingSpheres(BS);
//	}
//	private void w(CullingGroupEvent s)
//	{
//		if (s.hasBecomeInvisible) 
//        {
//            ItemDrops[s.index].enabled = false;
//        }
//	}
//}
public class ItemDrop : NetworkBehaviour
{
    public Transform ItemBox;
    public Equipment Item;
    [SyncVar]
    public uint EquipID;
    public SpriteRenderer Drop;
    public SpriteRenderer Box;
    public Collider2D Trigger;
    public Sprite wep;
    public Sprite sec;
    public Sprite act;
    public int Phase;
    public float Eta;
    public long EtaTime;

	//  void Start()
	//  {
	//      if(isServer)
	//{
	//          NetworkServer.Spawn(gameObject);
	//}
	//  }

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
        if (Item) 
            Item.Randomize();
        else
        {
            SetItem(pos,Registry.Reg.SpawnRandomEquipment());
        }
    }
    [Command(ignoreAuthority = true)]
    public void GetItem() 
    {
        SetItem(transform.position,Item.netId);
    }
    [ClientRpc]
    public void SetItem(Vector2 pos,uint netId)
    {
        transform.position = pos;
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
                    ItemBox.localPosition = new Vector3(0, 0, -.1f);
                    Trigger.offset = ItemBox.localPosition;
                    UpdatePhase(1,Random.Range(20,40f));
                    Drop.enabled = false;
                    break;
                }
                ItemBox.localPosition = new Vector3(0, Eta * Eta, -.1f);
                Trigger.offset = ItemBox.localPosition;
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

                        RaycastHit2D hit = Physics2D.Raycast(Random.insideUnitCircle * 100 + rel, Vector2.down,20,1<<9);
                        if (hit.distance < 3 || hit.point == Vector2.zero)
                        {
                            return;
                        }
                        //Debug.Log(hit.collider +" | "+ hit.transform.name);
                        Randomize(hit.point);
                    }
                    break;
                }
                ItemBox.localPosition = new Vector3(0, (10-Eta) * (10-Eta), -.1f);
                Trigger.offset = ItemBox.localPosition;
                break;
        }
    }
    private void OnTriggerStay2D(Collider2D col)
    {
        Mob M = col.GetComponent<Mob>();
        if (M && Item && M.B.isInteracting())
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
