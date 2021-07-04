using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Animator : MonoBehaviour
{
    public List<BodyPart> Parts;
    public List<Attachment> Attachments;
    //public BodyPart Head;
    //public BodyPart Body;
    //public BodyPart RArm;
    //public BodyPart RForarm;
    //public BodyPart RHand;
    //public BodyPart LArm;
    //public BodyPart LForarm;
    //public BodyPart LHand;
    //public BodyPart RLeg;
    //public BodyPart RShin;
    //public BodyPart RFoot;
    //public BodyPart LLeg;
    //public BodyPart LShin;
    //public BodyPart LFoot;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public float R=2;
    void Update()
    {
        for (int i = 0; i < Attachments.Count; i++)
        {
            Attachment A = Attachments[i];
            Vector3 diff = ((A.Part1.Part.TransformPoint(A.offset1) - A.Part2.Part.TransformPoint(A.offset2)) / R);
            diff.z = 0;
            A.Part1.Part.position -= diff * A.Weight;
            A.Part2.Part.position += diff * (1 - A.Weight);
            A.Part1.Part.Rotate(Vector3.forward,5);
        }
    }
	private void OnDrawGizmos()
	{
        for(int i = 0; i < Attachments.Count; i++) 
        {
            Attachments[i].DrawGizmo();
        }
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(Animator))]
public class AnimatorEditor : Editor 
{
    Animator A;
    static bool Parts;
    static bool Attachments;

	private void OnEnable()
	{
        A = target as Animator;
	}

    public string[] GetStrings()
    {
        string[] strs = new string[A.Parts.Count];
        for(int i = 0; i < A.Parts.Count; i++) 
        {
            strs[i] = A.Parts[i].Part.name;
        }
        return strs;
    }

	public override void OnInspectorGUI()
	{
        Parts = EditorGUILayout.BeginFoldoutHeaderGroup(Parts,"Parts");
        if(Parts)
		{
            for(int i = 0; i < A.Parts.Count; i++) 
            {
                EditorGUILayout.LabelField($"Part {i}");
                EditorGUI.indentLevel++;
                A.Parts[i].Part = EditorGUILayout.ObjectField(A.Parts[i].Part,typeof(Transform),true) as Transform;
                EditorGUI.indentLevel--;
            }
		}
        EditorGUILayout.EndFoldoutHeaderGroup();

        Attachments = EditorGUILayout.BeginFoldoutHeaderGroup(Attachments, "Attachments");
        if(Attachments)
		{
            for(int i = 0; i < A.Attachments.Count; i++) 
            {
                EditorGUILayout.LabelField($"Attachment {i}");
                EditorGUI.indentLevel++;
                A.Attachments[i].Part1 = A.Parts[EditorGUILayout.Popup(A.Parts.FindIndex((x) => x.Part == A.Attachments[i].Part1.Part), GetStrings())];
                A.Attachments[i].Part2 = A.Parts[EditorGUILayout.Popup(A.Parts.FindIndex((x) => x.Part == A.Attachments[i].Part2.Part), GetStrings())];
                //A.Attachments[i].Part1 = EditorGUILayout.Vector2Field("Offset 1",A.Attachments[i].offset1);
                A.Attachments[i].offset1 = EditorGUILayout.Vector2Field("Offset 1",A.Attachments[i].offset1);
                A.Attachments[i].offset2 = EditorGUILayout.Vector2Field("Offset 2",A.Attachments[i].offset2);
                A.Attachments[i].Weight = EditorGUILayout.FloatField("Weight",A.Attachments[i].Weight);
                EditorGUI.indentLevel--;
            }
		}
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Separator();
        DrawDefaultInspector();
	}
}
#endif

[System.Serializable]
public class BodyPart 
{
    public Transform Part;
}

[System.Serializable]
public class Attachment
{
    public BodyPart Part1;
    public BodyPart Part2;
    public Vector2 offset1;
    public Vector2 offset2;
    public float Weight;
    public void DrawGizmo() 
    {
        if(Part1 != null)
            Gizmos.DrawWireSphere(Part1.Part.TransformPoint(offset1),.1f);
        if(Part2 != null)
            Gizmos.DrawWireSphere(Part2.Part.TransformPoint(offset2),.1f);
        if(Part1 != null && Part2 != null)
            Gizmos.DrawLine(Part1.Part.TransformPoint(offset1), Part2.Part.TransformPoint(offset2));   
    }
}
