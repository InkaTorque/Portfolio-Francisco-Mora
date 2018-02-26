using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RoomData))]

public class RoomEditor : Editor 
{
	public PrefabToInstantiate newGO;

	public override void OnInspectorGUI()
	{
		RoomData myScript = (RoomData)target;

        EditorGUILayout.LabelField("Room ID", myScript.RoomId);

        DrawDefaultInspector();

        newGO = (PrefabToInstantiate)EditorGUILayout.EnumPopup("Choose", newGO);

        if (GUILayout.Button("Rearange Hierarchy"))
        {
            myScript.RearangeHierarchy();
        }
	}
}
