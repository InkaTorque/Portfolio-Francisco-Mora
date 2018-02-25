using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PaletteSwapper))]
public class PaletteSwapperV3Editor : Editor {

    PaletteSwapper script;

    private void OnEnable()
    {
        script = target as PaletteSwapper;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if(GUILayout.Button("Apply Palette"))
        {
            script.TransferSwapTextureToRenderer();
        }

    }
}
