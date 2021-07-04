using Mirror;
using UnityEngine;

namespace L33t.Equipment
{
	public abstract class Equipment : NetworkBehaviour
	{
		public bool Abandand;// { get { return _Abandand; } set { Debug.Log("EquipAbandand set: "+value); _Abandand = value; } }
							 //private bool _Abandand;
		public float ExpireTime;
		public abstract bool Pickup(Mob M);

		[Command]
		public virtual void Drop() { }
		public abstract void Randomize();
		public abstract string PrintStats();
		//public abstract void SetStats(EquipmentStats stats);
		//
		//public interface EquipmentStats 
		//{
		//	void Set(Equipment E);
		//}

		protected virtual void Update()
		{
			if (Abandand)
			{
				ExpireTime -= Time.deltaTime;

				if (isServer && ExpireTime < 0)
				{
					if (hasAuthority)
					{
						//Abandand = false;
						//ExpireTime = 10;
						//Drop();
						Debug.LogError("Abandoned Held Item " + GetType().FullName);
						Abandand = false;
						return;
					}

					NetworkServer.Destroy(gameObject);
				}
			}
		}

		public bool PickUpAble = true;

		private void OnTriggerStay2D(Collider2D col)
		{
			if (isServer && PickUpAble)
			{
				Mob M = col.GetComponent<Mob>();
				if (M != null && M.B.isInteracting())
				{
					Pickup(M);
				}
			}
		}
	}

	public abstract class WeaponEquipment : Equipment
	{
		//public virtual float Damage { get; set; }
		public abstract void Use(Vector2 Pos);

	}
	public abstract class DirectedEquipment : Equipment
	{
		public abstract void Use(Vector2 Pos);

	}
	public abstract class ActiveEquipment : Equipment
	{
		public abstract void Use();

	}
	public abstract class PasiveEquipment : Equipment
	{

	}
}