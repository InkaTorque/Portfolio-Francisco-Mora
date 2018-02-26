using UnityEditor;
using UnityEngine;
using System.Collections;

public class SetTileTextures : Editor
{
    #region Load Variables
    TileMapWindow mapCreatorWindow;
    public GameObject[] backgroundTileList, mecanicTileList, foregroundTileList,currentTileList;
    public Texture2D tileBaseTex, tileHighLTex;
    Sprite tileLostTex, metaTileTex;
    public int maxTile = 999, maxTileColumn;
    public float[] tileBoxX, tileBoxY;
    public int currentTab = 0;
    public GUIStyle[] TileListStyle;
    public Rect windowRect;
    #endregion

    public void LoadListsAndGUIFirst()
    {
        #region Load GUI Files
        if (!mapCreatorWindow)
        {
            mapCreatorWindow = (TileMapWindow)EditorWindow.GetWindow(typeof(TileMapWindow));
        }
        tileBaseTex = Resources.Load(mapCreatorWindow.directory.resourceFile[0]) as Texture2D;
        tileHighLTex = Resources.Load(mapCreatorWindow.directory.resourceFile[1]) as Texture2D;
        #endregion

        #region Load Game Lists
        backgroundTileList = Resources.LoadAll<GameObject>(mapCreatorWindow.directory.resourcesPath[4]);
        mecanicTileList = Resources.LoadAll<GameObject>(mapCreatorWindow.directory.resourcesPath[5]);
        foregroundTileList = Resources.LoadAll<GameObject>(mapCreatorWindow.directory.resourcesPath[6]);
        #endregion

        #region Load Tile In Window
        LoadCurrentTabList();
        DrawCurrentTiles();
        GetTilePositions();
        #endregion
    }

    public void LoadCurrentTabList()
    {
        #region Change Tab List
        currentTab = mapCreatorWindow.currentTabOpen;
        currentTileList = null;
        if (currentTab == 0)
        {
            currentTileList = backgroundTileList;
        }
        if (currentTab == 1)
        {
            currentTileList = mecanicTileList;
        } 
        if (currentTab == 2)
        {
            currentTileList = foregroundTileList;
        }
        TileListStyle = null;
        TileListStyle = new GUIStyle[currentTileList.Length];
        #endregion
    }

    public void DrawCurrentTiles()
    {
        for (int a = 0; a < currentTileList.Length; a++)
        {
            #region Obtain Tile GUI Sprite From Editor Sprite
            metaTileTex = currentTileList[a].GetComponent<SpriteRenderer>().sprite;
            
            #endregion

            #region Create GUI Tile Texture
            Color[] pixels = new Color[(((int)metaTileTex.textureRect.width) * ((int)metaTileTex.textureRect.height))];
            pixels = metaTileTex.texture.GetPixels((int)metaTileTex.textureRect.x, (int)metaTileTex.textureRect.y, (int)metaTileTex.textureRect.width, (int)metaTileTex.textureRect.height); //Copys pixles of Sprite.
            Texture2D tileObjectTex = new Texture2D((int)metaTileTex.rect.width, (int)metaTileTex.rect.height); //Create a new Texture to set all of copied pixels to.
            tileObjectTex.filterMode = FilterMode.Point;

            tileObjectTex.SetPixels(pixels);
            tileObjectTex.Apply();
            GUIStyle styleTile = new GUIStyle();
            styleTile.normal.background = tileObjectTex; //Apply Texture to Style BG.
            styleTile.normal.background.name = currentTileList[a].name;
            TileListStyle[a] = styleTile; // Apply temp tile style to Public Main List Tile Style
            #endregion
        }
    }

    public void GetTilePositions()
    {
        #region Load Tiles into Position
        tileBoxX = new float[maxTile];
        tileBoxY = new float[maxTile];
        windowRect = new Rect(mapCreatorWindow.position.x, mapCreatorWindow.position.y, mapCreatorWindow.position.width, mapCreatorWindow.position.height);
        maxTileColumn = (int)Mathf.Floor((windowRect.width-115)/70); //Track number of tiles to start new line
        for (int x = 0; x < maxTile; x++)
        {
            if (x == 0)
            {
                /*Default start position of box Tile0 within the group OnGUI*/
                tileBoxX[x] = 0;
                tileBoxY[x] = 0;
            }
            else
            {
                tileBoxX[x] = tileBoxX[x - 1] + 70;
                tileBoxY[x] = tileBoxY[x - 1]; //Every next tile is set to the previous tiles coordinates, +70 on the X axis(box size<50> + space between box<20>)
                if (tileBoxX[x] >= windowRect.width-115)//If the next tiles X is greater than the border surrounding the window, move the tile down on Y axis by required space
                {
                    tileBoxX[x] = 0;
                    tileBoxY[x] += 100;
                }
            }
        }
        #endregion
    }
}
