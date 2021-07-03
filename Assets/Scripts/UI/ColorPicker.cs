using UnityEngine;
using UnityEngine.UI;
namespace L33t.UI
{
	public class ColorPicker : MonoBehaviour
	{
		public static ColorPicker CP;
		public Image ColorRep;

		public Material Material;

		public Slider R;
		public Slider G;
		public Slider B;
		public Image BR;
		public Image BG;
		public Image BB;
		Material MR;
		Material MG;
		Material MB;

		public Slider H;
		public Slider S;
		public Slider V;
		public Image BH;
		public Image BS;
		public Image BV;
		Material MH;
		Material MS;
		Material MV;

		public Color C = Color.white;
		public InputField N;

		private void Start()
		{
			CP = this;

			MR = new Material(Material);
			MG = new Material(Material);
			MB = new Material(Material);
			MH = new Material(Material);
			MS = new Material(Material);
			MV = new Material(Material);

			BR.material = MR;
			BG.material = MG;
			BB.material = MB;
			BH.material = MH;
			BS.material = MS;
			BV.material = MV;

			SetRGB();
			SetHSV();

			R.onValueChanged.AddListener((v) => { C.r = v; SetHSV(); });
			G.onValueChanged.AddListener((v) => { C.g = v; SetHSV(); });
			B.onValueChanged.AddListener((v) => { C.b = v; SetHSV(); });

			H.onValueChanged.AddListener((v) => { Color.RGBToHSV(C, out float H, out float S, out float Va); C = Color.HSVToRGB(v, S, Va); SetRGB(); });
			S.onValueChanged.AddListener((v) => { Color.RGBToHSV(C, out float H, out float S, out float Va); C = Color.HSVToRGB(H, v, Va); SetRGB(); });
			V.onValueChanged.AddListener((v) => { Color.RGBToHSV(C, out float H, out float S, out float Va); C = Color.HSVToRGB(H, S, v); SetRGB(); });
		}

		public void SetRGB()
		{
			R.value = C.r;
			MR.SetColor("C1",new Color(0,C.g,C.b));
			MR.SetColor("C2",new Color(1,C.g,C.b));
			G.value = C.g;
			MG.SetColor("C1",new Color(C.r,0,C.b));
			MG.SetColor("C2",new Color(C.r,1,C.b));
			B.value = C.b;
			MB.SetColor("C1",new Color(C.r,C.g,0));
			MB.SetColor("C2",new Color(C.r,C.g,1));
		}
		public void SetHSV()
		{
			Color.RGBToHSV(C, out float h, out float s, out float v);
			H.value = h;
			MH.SetColor("C1", Color.HSVToRGB(1, s, v));
			MH.SetColor("C2", Color.HSVToRGB(0, s, v));
			S.value = s;
			MS.SetColor("C1", Color.HSVToRGB(h, 1, v));
			MS.SetColor("C2", Color.HSVToRGB(h, 0, v));
			V.value = v;
			MV.SetColor("C1", Color.HSVToRGB(h, s, 1));
			MV.SetColor("C2", Color.HSVToRGB(h, s, 0));
		}

		private void Update()
		{
			ColorRep.color = C;
		}

		private void OnDisable()
		{

		}
	}
}