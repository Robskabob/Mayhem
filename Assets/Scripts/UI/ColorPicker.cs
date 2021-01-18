using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
	public static ColorPicker CP;
	public Image ColorRep;

	public Slider R;
	public Slider G;
	public Slider B;

	public Slider H;
	public Slider S;
	public Slider V;

	public Color C = Color.white;
	public InputField N;

	private void Start()
	{
		CP = this;
		SetRGB();
		SetHSV();
		R.onValueChanged.AddListener((v) => { C.r = v; SetHSV(); });
		G.onValueChanged.AddListener((v) => { C.g = v; SetHSV(); });
		B.onValueChanged.AddListener((v) => { C.b = v; SetHSV(); });

		H.onValueChanged.AddListener((v) => { Color.RGBToHSV(C,out float H, out float S, out float Va); C = Color.HSVToRGB(v, S, Va); SetRGB(); });
		S.onValueChanged.AddListener((v) => { Color.RGBToHSV(C,out float H, out float S, out float Va); C = Color.HSVToRGB(H, v, Va); SetRGB(); });
		V.onValueChanged.AddListener((v) => { Color.RGBToHSV(C,out float H, out float S, out float Va); C = Color.HSVToRGB(H, S, v ); SetRGB(); });
	}

	public void SetRGB()
	{
		R.value = C.r;
		G.value = C.g;
		B.value = C.b;
	}
	public void SetHSV()
	{
		Color.RGBToHSV(C, out float h, out float s, out float v);
		H.value = h;
		S.value = s;
		V.value = v;
	}

	private void Update()
	{
		ColorRep.color = C;
	}
}