using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace L33t.UI
{
	public class StatMenu : UIBehaviour
	{
		public Text Name;
		public class Stats
		{
			public string Name;
			public Stat<dynamic> Stat;
		}
		public class Stat<T>
		{
			public T value;
		}
	}
}