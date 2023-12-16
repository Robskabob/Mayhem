using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace L33t.UI
{
	public class StatMenu : UIBehaviour
	{
		public Text Title;
		public List<Stat> Stats;
		public Stat Prefab;

		public void Regenerate(data[] stats) 
		{
			for(int i = 0; i < stats.Length; i++) 
			{
				if (Stats.Count < (i + 1)) 
				{
					Stat S = Instantiate(Prefab,transform);
					(S.transform as RectTransform).anchoredPosition = new Vector2(0,(i*-20)-40);
					Stats.Add(S);
				}

				Stats[i].Back.enabled = true;
				Stats[i].Label.text = stats[i].Label; //.Value.ToString();
				Stats[i].Image.fillAmount = stats[i].Value / stats[i].Max;
				Stats[i].Image.color = stats[i].Color;
			}
			if (Stats.Count > stats.Length) 
			{
				for(int i = stats.Length; i < Stats.Count; i++)
				{
					Stats[i].Back.enabled = false;
				}
			}
		}

		public struct data
		{
			public data(string label,Color color, float min, float max, float value)
			{
				Label = label ;
				Color = color;
				Min = min;
				Max = max;
				Value = value;
			}

			public string Label;
			public Color Color;
			public float Min, Max, Value;
		}

		public void Setup (string title, data[] stats)
		{
			Title.text = title;
			Regenerate(stats);
		}

		public void UpdateValues(float[] val)
		{
			for (int i = 0; i < val.Length; i++)
			{
				Stats[i].Image.fillAmount = val[i];
			}
		}
		public void UpdateValues(data[] val)
		{
			for (int i = 0; i < val.Length; i++)
			{
				Stats[i].Image.fillAmount = val[i].Value / val[i].Max;
			}
		}
	}
}