using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
//AUTHOR FRANCISCO ANTONIO MORA ARAMBULO

[CustomEditor(typeof(ColorPallete))]
public class ColorPaletteEditor : Editor {

    public ColorPallete colorPalette;
    public float sAndVAmountVariance;

    private void OnEnable()
    {
        colorPalette = target as ColorPallete;
    }

    public override void OnInspectorGUI()
    {
        //BUILDS CUSTOM INSPECTOR WITH 2 COLUMS SHOWCASING BASE PALETTE AND CUSTOM NEW SWAP PALETTE
        GUILayout.Label("Source Texture");

        colorPalette.source = EditorGUILayout.ObjectField(colorPalette.source, typeof(Texture2D), false) as Texture2D;

        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("Original Palette");
        GUILayout.Label("Swap Palette");

        EditorGUILayout.EndHorizontal();

        for(int i=0; i < colorPalette.originalPalette.Count;i++)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.ColorField(colorPalette.originalPalette[i]);

            colorPalette.swapPalette[i] = EditorGUILayout.ColorField(colorPalette.swapPalette[i]);

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Separator();
        sAndVAmountVariance = EditorGUILayout.FloatField("H AND S VALUE VARIANCE", sAndVAmountVariance);
        if (GUILayout.Button("CHANGE H AND S VALUES"))
        {
            colorPalette.ChangeSnV(sAndVAmountVariance);
        }


        if (GUILayout.Button("RESET SWAP PALLETE"))
        {
            colorPalette.SetPalette();
        }

        EditorUtility.SetDirty(colorPalette);

    }
}
