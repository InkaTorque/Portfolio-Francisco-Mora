using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

//AUTHOR : FRANCISCO ANOTNIO MORA ARAMBULO

[Serializable]
public class ColorPallete : ScriptableObject {

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Color Palette")]
    public static void CreateColorPalette()
    {
        if (Selection.activeObject is Texture2D)
        {
            Texture2D selectedTexture = Selection.activeObject as Texture2D;

            string selectionPath = AssetDatabase.GetAssetPath(selectedTexture);

            selectionPath = selectionPath.Replace(".png", ".asset");

            ColorPallete newPalette = CustomAssetUtility.CreateAsset<ColorPallete>(selectionPath);

            newPalette.source = selectedTexture;
            newPalette.SetPalette();

            Debug.Log(selectionPath);
        }
        else
        {
            Debug.LogError("CANT CREATE A PALETTE");
        }
    }
#endif

    public Texture2D source, cachedTexture;
    public List<Color> originalPalette = new List<Color>();
    public List<Color> swapPalette = new List<Color>();

    private void BuildPalette(Texture2D source)
    {
        originalPalette.Clear();
        swapPalette.Clear();
        int counter = 0;

        var colors = source.GetPixels();

        foreach (Color color in colors)
        {
            if (counter < colors.Length / 2)
            {
                originalPalette.Add(color);
            }
            else
            {
                swapPalette.Add(color);
            }
            counter++;
        }

    }

    public void SetPalette()
    {
        BuildPalette(source);
    }

    public void ChangeSnV(float variance)
    {
        //CHANGES S AND V VALUES OF HSV SPECTRUM BY variance UNITS
        float h, s, v;
        int i = 0;
        foreach (Color col in swapPalette)
        {
            Color.RGBToHSV(col, out h, out s, out v);
            s = (359 * s);
            v = (359 * v);
            s = s + variance;
            v = v + variance;
            s = (s / 359);
            v = (v / 359);
            swapPalette[i] = Color.HSVToRGB(h, s, v);
            i++;
        }
    }

    public Color GetColor(Color color)
    {
        for (int i = 0; i < originalPalette.Count; i++)
        {
            Color tmpColor = originalPalette[i];

            if (Mathf.Approximately(color.r, tmpColor.r) &&
                Mathf.Approximately(color.g, tmpColor.g) &&
                Mathf.Approximately(color.b, tmpColor.b) &&
                Mathf.Approximately(color.a, tmpColor.a))
            {
                return swapPalette[i];
            }
        }
        return color;
    }

}
