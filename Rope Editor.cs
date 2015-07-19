using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Rope))]
public class RopeEditor : Editor {

	public override void OnInspectorGUI() 
	{
		DrawDefaultInspector();

		Rope ropeScript = (Rope)target;

		if(GUILayout.Button ("Generate Rope"))
			ropeScript.GenerateRope();

		if(GUILayout.Button("Apply Rigidbodies"))
			ropeScript.ApplyRigidbody2D();

		if(GUILayout.Button("Apply Hinge2D"))
			ropeScript.ApplyHinge2D();
	}
}
