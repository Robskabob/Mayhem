using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class IKAnimator : MonoBehaviour
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
    public float rotR=2;
    void Update()
    {
        for (int i = 0; i < Attachments.Count; i++)
        {
            Attachment A = Attachments[i];
            Vector2 p1 = A.Part1.Part.TransformPoint(A.offset1);
            Vector2 p2 = A.Part2.Part.TransformPoint(A.offset2);
            Vector3 diff = ((p1-p2) / R);
            Vector2 dir = (p1 - p2).normalized;
            float dist = Vector2.Distance(p1, p2);
            //Vector2 Center = p1 - (dir * dist / 2);
            float theta1 = Vector2.SignedAngle(A.Part1.Part.TransformDirection(A.offset1),dir) / rotR;
            float theta2 = Vector2.SignedAngle(A.Part2.Part.TransformDirection(A.offset2),dir) / rotR;
            diff.z = 0;
            A.Part1.Part.position -= diff * A.Weight;
            A.Part2.Part.position += diff * (1 - A.Weight);
            A.Part1.Part.Rotate(Vector3.forward, theta1 * A.Weight);
            A.Part2.Part.Rotate(Vector3.forward, -theta2 * (1 - A.Weight));
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
[CustomEditor(typeof(IKAnimator))]
public class AnimatorEditor : Editor 
{
    IKAnimator A;
    static bool Parts;
    static bool Attachments;

	private void OnEnable()
	{
        A = target as IKAnimator;
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
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(Part1.Part.TransformPoint(offset1),.01f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(Part1.Part.position, Part1.Part.TransformDirection(offset1));
        }
        if(Part2 != null)
		{
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(Part2.Part.TransformPoint(offset2),.01f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(Part2.Part.position, Part2.Part.TransformDirection(offset2));
		}
        if(Part1 != null && Part2 != null) 
        {
            Vector2 p1 = Part1.Part.TransformPoint(offset1);
            Vector2 p2 = Part2.Part.TransformPoint(offset2);
            Vector2 dir = (p1 - p2).normalized;
            float dist = Vector2.Distance(p1,p2);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(p1,p2);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(p1 - (dir * dist / 2),dir);
            Handles.color = Color.black;
            //Handles.SphereHandleCap(0,Vector3.zero,Quaternion.identity,5,EventType.Repaint);
            Handles.DrawWireArc(p1,Vector3.forward, Part1.Part.TransformDirection(offset1), Vector2.SignedAngle(Part1.Part.TransformDirection(offset1),dir),.05f);
            Handles.DrawWireArc(p2,Vector3.forward, Part2.Part.TransformDirection(offset2), Vector2.SignedAngle(Part2.Part.TransformDirection(offset2),dir),.05f);
        }
    }
}
