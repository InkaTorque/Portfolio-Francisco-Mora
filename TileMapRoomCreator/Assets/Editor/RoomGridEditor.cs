using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
[CustomEditor(typeof(RoomGrid))]
public class RoomGridEditor : Editor
{
    private int newW=GameConstants.minimalRoomWidth, newH=GameConstants.minimalRoomHeight;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RoomGrid myScript = (RoomGrid)target;

        if (GUILayout.Button("CLEAR ROOM"))
        {
            myScript.ClearRoom();
        }

        newW=EditorGUILayout.IntField("New Width : ", newW);
        newH=EditorGUILayout.IntField("Nwe Height : ", newH);
        if (GUILayout.Button("RESIZE ROOM"))
        {
            myScript.ResizeRoom(newH, newW);
        }
        if (GUILayout.Button(" <-- UNDO "))
        {
            myScript.Undo();
        }
        if (GUILayout.Button(" REDO --> "))
        {
            myScript.Redo();
        } 
        if (GUILayout.Button(" UPDATE REFERENCES "))
        {
            myScript.UpdateGridReferences();
        }

        if (GUILayout.Button("SHOW OCCUPIED BLOCKS"))
        {
            myScript.MarkAllOccupiedBlocks();
        }
    }
}
