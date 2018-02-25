using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//AUTHOR : FRANCISCO ANOTNIO MORA ARAMBULO

public class PaletteSwapper : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    public ColorPallete currentPalette;

    private MaterialPropertyBlock block;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
            TransferSwapTextureToRenderer();
    }

    public void TransferSwapTextureToRenderer()
    {
        //TODO : TRANSFER SWAP PALETTE TEXTURE DATA AS AN ARRAY OF FLOAT4 INSTEAD OF A TEXTURE ASSET

        if(currentPalette!=null)
        {
            Texture2D basePaletteTexture, swapPaletteTexture;
            basePaletteTexture = new Texture2D(currentPalette.originalPalette.Count, 1, TextureFormat.RGBA32, false, false);
            basePaletteTexture.filterMode = FilterMode.Point;

            swapPaletteTexture = new Texture2D(currentPalette.swapPalette.Count, 1, TextureFormat.RGBA32, false, false);
            swapPaletteTexture.filterMode = FilterMode.Point;
            
            //GENERATING BASE PALETTE TEXTURE FOR SHADER
            basePaletteTexture.SetPixels(currentPalette.originalPalette.ToArray());
            basePaletteTexture.Apply();

            //GENERATING SWAP PALETTE TEXTURE FOR SHADER
            swapPaletteTexture.SetPixels(currentPalette.swapPalette.ToArray());
            swapPaletteTexture.Apply();

            //APPLYING SWAT TEXTURE TO MATERIAL
            block = new MaterialPropertyBlock();
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.GetPropertyBlock(block);
            block.SetTexture("_BasePaletteTex", basePaletteTexture);
            block.SetTexture("_SwapPaletteTex", swapPaletteTexture);
            block.SetFloat("_SwapTexColorNumber", currentPalette.swapPalette.Count);
            spriteRenderer.SetPropertyBlock(block);
        }
    }

    public ColorPallete CurrentPalette { set { currentPalette = value; } }
}
