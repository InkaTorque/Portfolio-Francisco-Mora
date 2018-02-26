using UnityEngine;
using System.Collections;
using UnityEditor;
[CustomEditor(typeof(SelectorLogic))]

public class SelectorLogicEditor : Editor {
	
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		SelectorLogic myScript = (SelectorLogic)target;
		
		if (GUILayout.Button("Select Room"))
		{
			Selection.activeGameObject = myScript.getParent();
		}
	}
}