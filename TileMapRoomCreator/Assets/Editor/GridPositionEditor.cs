using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
[CustomEditor(typeof(GridPosition))]
public class GridPositionEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridPosition myScript = (GridPosition)target;

        if (GUILayout.Button(" Move UP ^ "))
        {
            myScript.EditorMoveUp();
        }
        if (GUILayout.Button("! Move DOWN "))
        {
            myScript.EditorMoveDown();
        }

        if (GUILayout.Button(" Move RIGHT -> "))
        {
            myScript.EditorMoveRight();
        }
        if (GUILayout.Button(" <- Move LEFT "))
        {
            myScript.EditorMoveLeft();
        }
    }
}
