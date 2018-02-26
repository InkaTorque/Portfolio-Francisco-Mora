using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Text.RegularExpressions;

//AUTHOR : FRANCISCO ANOTNIO MORA ARAMBULO

public class TileMapWindow : EditorWindow
{
    Tool lastTool = Tool.None;

    public int currentTabOpen, tileSelectedID, setupStage, roomWidth = 48, roomHeight = 27;

    private static bool windowIsOpen, eraseTool, mouseDown;

    public Rect windowRect;
    public Vector2 scrollPos, mousePos, startDrag;
    public SetTileTextures tileTextures;
    public CheckDirectories directory;
    public float boxMaxX;
    public GameObject[] currentTabList;
    //Bounding box positionet at insertion position of selected tile
    public GameObject gizmoCursor;
    //SelectedTileGizmo
    public GameObject gizmoTile;

    public bool tileSelected, removingTile;
    public GUIStyle currentTileTex;

    //The tile only snaps to a position inside the room , this is done by raycasting and detecting if there
    //is a room collider underneath
    private int roomLayer;
    private int selectedItemTab;

    //Reference to the game object to be initialized when tile has been painted
    //on Room Grid
    private GameObject selectedObject;

    //variables used for positioning the tile on the cursor position
    private float offsetX, offsetY;
    private bool horizontal = true;

    #region Normal Update & Show Console Logs
    void Update()
    {
        if (tileSelected)
        {
            //keeps the native editor tools from interfering on the painter
            Tools.current = Tool.None;

            if (gizmoCursor != null)
            {
                gizmoCursor.transform.position = new Vector2(Mathf.RoundToInt(mousePos.x) + (GameConstants.gridBlockWidth / 2), Mathf.RoundToInt(mousePos.y) - (GameConstants.gridBlockHeight / 2));
            }
            if (gizmoTile != null)
            {
                //Positions the gizmo on the right offset from the cursor
                Transform ehb = selectedObject.transform.Find("EditorHitbox");
                if (ehb != null)
                {
                    BoxCollider2D Tbx2d = ehb.GetComponent<BoxCollider2D>();
                    if (Tbx2d.size.x > GameConstants.gridBlockWidth)
                    {
                        offsetX = Tbx2d.size.x / 2 - (GameConstants.gridBlockWidth / 2);
                    }
                    else
                    {
                        offsetX = 0;
                    }
                    if (Tbx2d.size.y > GameConstants.gridBlockHeight)
                    {
                        offsetY = Tbx2d.size.y / 2 - (GameConstants.gridBlockHeight / 2);
                    }
                    else
                    {
                        offsetY = 0;
                    }
                    if (horizontal)
                    {
                        gizmoTile.transform.position = new Vector2(gizmoCursor.transform.position.x + offsetX, gizmoCursor.transform.position.y - offsetY);
                    }
                    else
                    {
                        gizmoTile.transform.position = new Vector2(gizmoCursor.transform.position.x + offsetY, gizmoCursor.transform.position.y - offsetX);
                    }
                }
            }
            EditorApplication.MarkSceneDirty();
        }
        else
        {
            Tools.current = lastTool;
        }
        if (!windowIsOpen)
        {
            this.Close();
        }
    }

    #endregion

    //Opens the TileMap Level Creator , can also be opened with Ctrl + m
    [MenuItem("Room Manager/Tile Based Level Creator %m")]
    private static void Init()
    {
        TileMapWindow window = (TileMapWindow)EditorWindow.GetWindow(typeof(TileMapWindow));
        window.minSize = new Vector2(800, 600);
        window.title = "Tile Based Level Creator";
        window.Show();
        windowIsOpen = !windowIsOpen;
    }

    void OnEnable()
    {
        #region Enable Scene View
        SceneView.onSceneGUIDelegate += SceneGUI;
        lastTool = Tools.current;
        roomLayer = 1 << LayerMask.NameToLayer("Room");
        #endregion
    }

    void OnDisable()
    {
        #region Disable Script
        SceneView.onSceneGUIDelegate -= SceneGUI;
        DestroyImmediate(GameObject.Find("gizmoCursor"));
        DestroyImmediate(GameObject.Find("gizmoTile"));
        windowIsOpen = false;
        //Tools.current = lastTool;
        #endregion
    }

    void OnInspectorUpdate()
    {
        //Loads the according files to use with the tilemap level creator
        #region Load Scripts
        if (!directory)
        {
            directory = ScriptableObject.CreateInstance<CheckDirectories>();
        }
        if (!tileTextures)
        {
            tileTextures = ScriptableObject.CreateInstance<SetTileTextures>();
        }
        if (directory.fileCheckClear == 0)
        {
            directory.BeginDirectoryCheck();
            setupStage = directory.fileCheckClear;
            #region Clear Up Old Window
            // Occurs on change of scene or restart
            tileSelected = false;
            #endregion
        }
        if (setupStage == 2)
        {
            //loads required textures for tilemap room creator
            tileTextures.LoadListsAndGUIFirst();
            currentTabList = tileTextures.currentTileList;
            setupStage = 3;
        }
        if (setupStage == 3 && tileTextures != null)
        {
            #region Monitor Window Size & Tab
            windowRect = new Rect(this.position.x, this.position.y, this.position.width, this.position.height);
            int lastTileInRow = tileTextures.maxTileColumn;
            //Change Border Box BG to wrap around tile
            if (tileTextures.tileBoxX[lastTileInRow] == 0)
            {
                boxMaxX = tileTextures.tileBoxX[lastTileInRow - 1] + 80;
            }
            else
            {
                boxMaxX = tileTextures.tileBoxX[lastTileInRow] + 80;
            }
            //If item list is less than windows minimum column length
            if (currentTabList.Length <= lastTileInRow)
            {
                boxMaxX = tileTextures.tileBoxX[currentTabList.Length] + 10;
            }
            //Detect Tab change
            if (currentTabOpen != tileTextures.currentTab)
            {
                tileTextures.LoadListsAndGUIFirst();
                currentTabList = tileTextures.currentTileList; // Load Current Open Tabs Textures
            }
            //Detect window resize
            if (windowRect != tileTextures.windowRect)
            {
                tileTextures.GetTilePositions();
            }
            #endregion
        }
        if (setupStage == -1)
        {
            Debug.LogError("Directory Check Failed. Closing Window.");
            this.Close();
        }
        #endregion

        #region Load Gizmos on Tile 
        //if theres a tile selected and the window textures have been loaded up
        if (tileSelected && setupStage == 3)
        {
            if (gizmoCursor == null)
            {
                gizmoCursor = (GameObject)Instantiate(Resources.Load(directory.resourceFile[3]));
                gizmoCursor.name = "gizmoCursor";
                gizmoCursor.hideFlags = HideFlags.HideInHierarchy;
            }

            if (gizmoTile == null)
            {
                gizmoTile = (GameObject)Instantiate(currentTabList[tileSelectedID]);
                gizmoTile.transform.name = "gizmoTile";
                selectedObject = currentTabList[tileSelectedID];
                SpriteRenderer[] tempTile;
                tempTile = gizmoTile.GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer sRen in tempTile)
                {
                    sRen.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.4f); //40% transparent
                }
                gizmoTile.hideFlags = HideFlags.HideInHierarchy;
            }
        }
        else
        {
            DestroyImmediate(GameObject.Find("gizmoCursor"));
            DestroyImmediate(GameObject.Find("gizmoTile"));
        }
        #endregion
        Repaint();
    }

    void SceneGUI(SceneView sceneView)
    {
        #region Gain Scene Views Mouse Coordinates
        //if creation available
        if (setupStage == 3 && tileTextures != null)
        {
            #region Setup Mouse Coordinates
            Event e = Event.current;
            Ray worldRays = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            mousePos = worldRays.origin;
            #endregion

            #region User Input on Scene
            if (tileSelected && gizmoTile != null)
            {
                int controlID = GUIUtility.GetControlID(FocusType.Passive);
                if (e.type == EventType.Layout)
                {
                    HandleUtility.AddDefaultControl(controlID);
                }
                switch (e.type)
                {
                    case EventType.MouseDown:
                        {
                            if (e.button == 0) //LEFT CLICK DOWN
                            {
                                mouseDown = true;
                            }
                            break;
                        }
                    case EventType.MouseUp:
                        {
                            if (e.button == 0) //LEFT CLICK UP
                            {
                                mouseDown = false;
                            }
                            break;
                        }
                    case EventType.KeyDown:
                        {
                            //Scape Key cancels selection
                            if (e.keyCode == KeyCode.Escape)
                            {
                                tileSelected = false;
                                Debug.Log("Pressed Scape");
                            }
                            //Holding down left control errases tiles on the room
                            if (e.keyCode == KeyCode.LeftControl)
                            {
                                eraseTool = true;
                            }
                            Event.current.Use();    // if you don't use the event, the default action will still take place.
                        }
                        break;
                    case EventType.KeyUp:
                        {
                            if (e.keyCode == KeyCode.LeftControl)
                            {
                                eraseTool = false;
                            }
                        }
                        break;
                }
            }
            #endregion

            #region Mouse Click Actions
            if (mouseDown)
            {

                if (eraseTool)
                {
                    removingTile = true;
                }
                else
                {
                    CheckAvailableCreation();
                    removingTile = false;
                }
            }
            else
            {
                if (eraseTool)
                {
                    removingTile = false;
                }
            }
            if (removingTile)
            {
                CheckTileRemoval();
            }
            #endregion
        }
        #endregion
    }

    void OnGUI()
    {
        Event e = Event.current;
        if (setupStage == 3 && tileTextures != null)
        {
            GUILayout.Box("", GUILayout.Width(windowRect.width - 5), GUILayout.Height(100)); //Setting Box
            GUI.Box(new Rect(30, 10, 64, 64), tileTextures.tileHighLTex, new GUIStyle()); // Current Tile Base Box
            GUI.Label(new Rect(5, 80, 200, 20), "Currently Selected Tile"); //Current Tile Label

            GUIStyle tmp = new GUIStyle();
            tmp.normal.textColor = Color.black;
            tmp.fontSize = 10;
            tmp.fontStyle = FontStyle.Bold;
            tmp.contentOffset = new Vector2(0, 10);

            GUI.Label(new Rect(175, 25, 170, 50), "Level Dimmensions \n ( 1 Block = 120px x 120px )");
            GUI.Label(new Rect(180, 60, 50, 35), "-Width-");
            roomWidth = EditorGUI.IntField(new Rect(230, 60, 22, 22), "", roomWidth);
            if (roomWidth <= 15)
            {
                roomWidth = 16;
            }
            GUI.Label(new Rect(260, 60, 35, 35), "X", tmp);
            GUI.Label(new Rect(295, 60, 50, 35), "-Height-");
            roomHeight = EditorGUI.IntField(new Rect(275, 60, 22, 22), "", roomHeight);
            if (roomHeight <= 8)
            {
                roomHeight = 9;
            }
            if (GUI.Button(new Rect(365, 5, 200, 95), "Create Room"))
            {
                RoomCRUD.CreateRoom(roomWidth, roomHeight);
            }
            if (GUI.Button(new Rect(570, 10, 130, 40), "Rotate Clockwise"))
            {
                RotateClockwise();
            }
            if (GUI.Button(new Rect(570, 55, 130, 40), "Rotate CounterClockwise"))
            {
                RotateCounterClockwise();
            }
            if (GUI.Button(new Rect(705, 10, 90, 40), "Flip Horizontal"))
            {
                FlipHorizontalGizmos();
            }
            if (GUI.Button(new Rect(705, 55, 90, 40), "Flip Vertial"))
            {
                FlipVertticalGizmos();
            }

            GUILayout.Box("Controls", GUILayout.Width(windowRect.width - 5), GUILayout.Height(125));
            GUI.Label(new Rect(10, 125, 700, 50), " - To START : Press the 'CREATE ROOM' button on the upper section of the TileMap Room Creator Window");
            GUI.Label(new Rect(10, 135, 700, 50), " - Select A Tab To View Available Tiles");
            GUI.Label(new Rect(10, 145, 700, 50), " - Selecting a Tile creates a Gizmo that follows the cursor on Scene View");
            GUI.Label(new Rect(10, 155, 700, 50), " - To position a Tile on the Room . With the gizmo active , Left Click on the desired position inside the Room");
            GUI.Label(new Rect(10, 165, 700, 50), " - ESC : Cancels tile selection");
            GUI.Label(new Rect(10, 175, 700, 50), " - LEFT CONTROL : Holding down this key activates the erraser tool \n                             While holding down this key , if Left Click is pressed over an existing tile , it will be removed");
            GUI.Label(new Rect(10, 205, 700, 50), " - WARNING : Only ONE tile of each Layer can exist on the same space inside the room");

            currentTabOpen = GUILayout.Toolbar(currentTabOpen, new string[] { "Background Layer (Decorative)", "Ground Layer (Mostly Mechanics)", " Foreground Layer" });

            scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(windowRect.width), GUILayout.Height(windowRect.height - 130));
            GUIStyle style = new GUIStyle(); //Blank GUIStyle to clear current textures
            GUIStyle styleText = new GUIStyle(); //Blank GUIStyle to clear current textures
            style.normal.background = tileTextures.tileBaseTex;
            style.hover.background = tileTextures.tileHighLTex;
            style.normal.background = tileTextures.tileBaseTex;
            style.hover.background = tileTextures.tileHighLTex;
            styleText.fontStyle = FontStyle.Bold;
            styleText.wordWrap = true;
            styleText.alignment = TextAnchor.UpperCenter;
            GUI.Box(new Rect(5, 5, boxMaxX, tileTextures.tileBoxY[currentTabList.Length - 1] + 110), ""); //BG Box wrapped around Content Box
            GUILayout.Box("", new GUIStyle(), GUILayout.Width(windowRect.width - 50), GUILayout.Height(tileTextures.tileBoxY[currentTabList.Length - 1] + 85)); //Content Box, used for size of window scroll only
            GUI.BeginGroup(new Rect(15, 15, windowRect.width, tileTextures.tileBoxY[currentTabList.Length - 1] + 120));
            for (int x = 0; x < currentTabList.Length; x++)
            {
                if (GUI.Button(new Rect(tileTextures.tileBoxX[x], tileTextures.tileBoxY[x], 64, 64), "", style)) //Tile Background Box
                {
                    tileSelectedID = x;
                    tileSelected = true;
                    selectedItemTab = currentTabOpen;
                    if (gizmoTile != null)
                    {
                        DestroyImmediate(gizmoTile);
                    }
                    CalculateOrientation(currentTabList[tileSelectedID]);
                    currentTileTex = new GUIStyle();
                    currentTileTex = tileTextures.TileListStyle[tileSelectedID];
                }
                GUI.Box(new Rect(tileTextures.tileBoxX[x] + 8, tileTextures.tileBoxY[x] + 8, 48, 48), "", tileTextures.TileListStyle[x]); // Tile Actual Tile Sprite
                GUI.Label(new Rect(tileTextures.tileBoxX[x], tileTextures.tileBoxY[x] + 64, 64, 150), tileTextures.currentTileList[x].name, styleText);
            }
            GUI.EndGroup();
            GUILayout.EndScrollView();
            if (tileSelected)
            {
                GUI.Box(new Rect(37.5f, 20, 48, 48), "", currentTileTex);
                if (EditorWindow.focusedWindow != this && EditorWindow.focusedWindow != EditorWindow.GetWindow<SceneView>())
                {
                    tileSelected = false;
                }
            }
        }

    }
    void CheckAvailableCreation()
    {
        Vector2 createPosition;
        Collider2D col;
        if (col = Physics2D.OverlapPoint(gizmoTile.transform.position, roomLayer))
        {
            Vector2 center = col.gameObject.transform.position;
            RoomGrid rg = col.gameObject.GetComponent<RoomGrid>();
            GameObject candidate = currentTabList[tileSelectedID];
            Transform editorHitTrans = candidate.transform.Find("EditorHitbox");
            if (editorHitTrans != null)
            {
                GameObject candidateHitbox = editorHitTrans.gameObject;
                BoxCollider2D c2d = candidateHitbox.GetComponent<BoxCollider2D>();
                Rect candidateRect;
                if(horizontal)
                {
                    candidateRect = new Rect(gizmoTile.transform.position.x - (c2d.size.x / 2),
                    gizmoTile.transform.position.y + (c2d.size.y / 2),
                    c2d.size.x,
                    c2d.size.y);
                }
                else
                {
                    candidateRect = new Rect(gizmoTile.transform.position.x - (c2d.size.y / 2),
                    gizmoTile.transform.position.y + (c2d.size.x / 2),
                    c2d.size.y,
                    c2d.size.x);
                }

                if (rg.CheckAvailableBlocks(candidateRect, selectedItemTab, center, out createPosition))
                {
                    AddTileToScene(createPosition, rg);
                }
            }
        }
    }
    void AddTileToScene(Vector2 createPosition, RoomGrid rg)
    {
        GameObject metaTile = (GameObject)Instantiate(currentTabList[tileSelectedID]);
        metaTile.transform.position = createPosition;
        rg.AddReferenceToBlock(metaTile, selectedItemTab);

    }

    void CheckTileRemoval()
    {
        Collider2D col;
        if (col = Physics2D.OverlapPoint(mousePos, roomLayer))
        {
            Vector2 center = col.gameObject.transform.position;
            RoomGrid rg = col.gameObject.GetComponent<RoomGrid>();
            Rect eraseRect = new Rect(mousePos.x, mousePos.y, 25, 25);
            rg.EraseBlock(eraseRect, selectedItemTab, center);
        }
    }

    public void RotateClockwise()
    {
        if (tileSelected)
        {
            GameObject go = currentTabList[tileSelectedID];
            go.transform.RotateClockwise();
            gizmoTile.transform.RotateClockwise();
            CalculateOrientation(go);
        }
    }

    public void RotateCounterClockwise()
    {
        if (tileSelected)
        {
            GameObject go = currentTabList[tileSelectedID];
            go.transform.RotateAntiClockwise();
            gizmoTile.transform.RotateAntiClockwise();
            CalculateOrientation(go);
        }
    }

    public void CalculateOrientation(GameObject go)
    {
        if (((Mathf.FloorToInt(go.transform.eulerAngles.z) / 90) % 2) == 0)
        {
            horizontal = true;
        }
        else
        {
            if (Mathf.Abs(((Mathf.FloorToInt(go.transform.eulerAngles.z) / 90) % 2)) == 1)
            {
                horizontal = false;
            }
        }
    }

    public void FlipHorizontalGizmos()
    {
        if (tileSelected)
        {
            GameObject go = currentTabList[tileSelectedID];
            go.transform.localScale = new Vector3((go.transform.localScale.x * -1), go.transform.localScale.y, go.transform.localScale.z);
            gizmoTile.transform.localScale = new Vector3((gizmoTile.transform.localScale.x * -1), gizmoTile.transform.localScale.y, gizmoTile.transform.localScale.z);
        }
    }

    public void FlipVertticalGizmos()
    {
        if (tileSelected)
        {
            GameObject go = currentTabList[tileSelectedID];
            go.transform.localScale = new Vector3(go.transform.localScale.x, (go.transform.localScale.y * -1), go.transform.localScale.z);
            gizmoTile.transform.localScale = new Vector3(gizmoTile.transform.localScale.x, (gizmoTile.transform.localScale.y * -1), gizmoTile.transform.localScale.z);
        }
    }


}