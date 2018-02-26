using UnityEngine;
using System.Collections.Generic;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class GridBlock
{
    [SerializeField]
    public Rect pos = new Rect(0,0,GameConstants.gridBlockWidth,GameConstants.gridBlockHeight);
    [SerializeField]
    public GameObject backgroundObject;
    [SerializeField]
    public GameObject groundObject;
    [SerializeField]
    public GameObject foregroundObject;
    [SerializeField]
    public SpriteRenderer blockSprite;
}

[System.Serializable]
public class GridRow
{
    [SerializeField]
    public GridBlock[] cells;
}

[System.Serializable]
public class StackData
{
    [SerializeField]
    public string actionType;
    [SerializeField]
    public Vector2 position;
    [SerializeField]
    public string objectTag;
    [SerializeField]
    public int height , width ,blockCount,tabIndex;
    [SerializeField]
    public List<Coordinate> coorndinates;
}
[System.Serializable]
public class Coordinate
{
    [SerializeField]
    public int y,x;

    public Coordinate()
    {

    }

    public Coordinate(int xCoord , int yCoord)
    {
        x = xCoord;
        y = yCoord;
    }

    public string ToString()
    {
        return "[" + y + "," + x + "]";
    }

    public bool Equals(Coordinate second)
    {
        bool equals = false;
        if ((x == second.x) && (y == second.y))
        {
            equals = true;
        }
        return equals;
    }
}
[System.Serializable]
public class BlocksToAssign
{
    [SerializeField]
    public GridBlock block;
    [SerializeField]
    public Coordinate coordinate;
}

[System.Serializable]
public class RoomGrid : MonoBehaviour {

    [SerializeField]
    [HideInInspector]
    public int roomHeight, roomWidth,blockCount;
    [SerializeField]
    [HideInInspector]
    public GridRow[] gridMatrix;
    [SerializeField]
    [HideInInspector]
    public BlocksToAssign[] blocksToAssign;
    [SerializeField]
    [HideInInspector]
    public List<StackData> undoStack;
    [SerializeField]
    [HideInInspector]
    public List<StackData> redoStack;
    public bool DisableGridGismo=true;
    [SerializeField]
    private Vector2 topLeftCorner;

    private List<Coordinate> bushCoordinates;

    public Sprite blueBlock;

    void Start()
    {
        if(DisableGridGismo)
        {
            DisableGridSprites();
        }
        bushCoordinates = findCoordinatesOfObjectType("HiddingBush");
    }

    #region GridCreation

    public void CreateGrid(int width , int height)
    {
        roomHeight = height;
        roomWidth = width;
        GridRow[] auxGrid2 = new GridRow[roomHeight];
        for(int x=0;x<roomHeight;x++)
        {
            auxGrid2[x] = new GridRow();
            for(int y=0;y<roomWidth;y++)
            {
                auxGrid2[x].cells = new GridBlock[roomWidth];
            }
        }
        Transform gridBlockGroupT = transform.Find("GridBlockGroup");
        GameObject gridBlockGroup;
        if(gridBlockGroupT==null)
        {
            gridBlockGroup = new GameObject("GridBlockGroup");
            gridBlockGroup.transform.parent = gameObject.transform;
        }
        else
        {
            gridBlockGroup = gridBlockGroupT.gameObject;
        }
        gridBlockGroup.transform.localPosition = Vector2.zero;
        Transform boundary = transform.Find("Playable Boundary");
        Transform leftW = boundary.gameObject.transform.Find("Left Wall");
        Transform topW = boundary.gameObject.transform.Find("Top Wall");
        topLeftCorner = new Vector2(leftW.gameObject.transform.localPosition.x, topW.gameObject.transform.localPosition.y);
        SpriteRenderer sr;
        for(int i=0;i<roomHeight;i++)
        {
            for(int j=0;j<roomWidth;j++)
            {
                auxGrid2[i].cells[j] = new GridBlock();
                GridBlock auxblock = auxGrid2[i].cells[j];
                auxblock.pos.x = topLeftCorner.x + ((auxblock.pos.width) * j);
                auxblock.pos.y = topLeftCorner.y - ((auxblock.pos.height) * i);
                GameObject spriteBlock = new GameObject("GridBlock");
                spriteBlock.tag = "GridBlock";
                spriteBlock.AddComponent<SpriteRenderer>();
                sr = spriteBlock.GetComponent<SpriteRenderer>();
                sr.sprite = Resources.Load("Sprites/LevelConstructionSprites/GreenGridSMALL", typeof(Sprite)) as Sprite;
                spriteBlock.transform.parent = gridBlockGroup.transform;
                spriteBlock.transform.localPosition = new Vector2(auxblock.pos.x+(auxblock.pos.width/2), auxblock.pos.y-(auxblock.pos.height/2));
                auxblock.blockSprite = sr;
            }
        }
        Transform rightW = boundary.gameObject.transform.Find("Right Wall");
        Transform botW = boundary.gameObject.transform.Find("Bot Wall");
        rightW.localPosition = new Vector2(topLeftCorner.x + (roomWidth * GameConstants.gridBlockWidth), rightW.localPosition.y);
        botW.localPosition = new Vector2(botW.localPosition.x, topLeftCorner.y-(roomHeight*GameConstants.gridBlockHeight));
        gridMatrix = auxGrid2;
        undoStack = new List<StackData>();
        redoStack = new List<StackData>();
    }

    #endregion

    #region ObjectInsertion
    public bool CheckAvailableBlocks(Rect candidate, int selectedTab, Vector2 center, out Vector2 createPosition)
    {
        bool foundIntersection = false, creationAvailable = false, needsMoreX = true, needsMoreY = true;
        blocksToAssign = new BlocksToAssign[roomWidth * roomHeight];
        int initialX = -1, finalX = -1, initialY = -1, finalY = -1, Xlimit, Ylimit;
        blockCount = 0;
        Ylimit = roomHeight;
        Xlimit = roomWidth;
        for (int x = 0; x < blocksToAssign.Length; x++)
        {
            blocksToAssign[x] = new BlocksToAssign();
            blocksToAssign[x].coordinate = new Coordinate();
        }
        for (int i = 0; i < Ylimit; i++)
        {
            for (int j = 0; j < Xlimit; j++)
            {
                if (RectsIntersect(candidate, gridMatrix[i].cells[j].pos, center))
                {
                    foundIntersection = true;

                    if (CheckIfAvailable(gridMatrix[i].cells[j], selectedTab))
                    {
                        if (blockCount == 0)
                        {
                            initialX = j;
                            finalX = j;
                            initialY = i;
                            finalY = i;
                        }
                        else
                        {
                            if (j > finalX)
                            {
                                finalX = j;
                            }
                            if (i > finalY)
                            {
                                finalY = i;
                            }
                        }
                        if (((finalX - initialX + 1) * GameConstants.gridBlockWidth >= candidate.width) && needsMoreX)
                        {
                            Xlimit = j + 1;
                            needsMoreX = false;
                        }
                        if (((finalY - initialY + 1) * GameConstants.gridBlockHeight >= candidate.height) && needsMoreY)
                        {
                            Ylimit = i + 1;
                            needsMoreY = false;
                        }
                        blocksToAssign[blockCount].block = gridMatrix[i].cells[j];
                        blocksToAssign[blockCount].coordinate.x = j;
                        blocksToAssign[blockCount].coordinate.y = i;
                        blockCount++;
                        if (!needsMoreX && !needsMoreY && blockCount == ((candidate.width / GameConstants.gridBlockWidth) * (candidate.height / GameConstants.gridBlockHeight)))
                        {
                            creationAvailable = true;
                            i = Ylimit;
                            break;
                        }
                    }
                    else
                    {
                        i = roomHeight;
                        break;
                    }
                }
            }
        }


        if (foundIntersection == true)
        {
            if (creationAvailable == true)
            {
                Rect origin = blocksToAssign[0].block.pos;
                origin.x += center.x;
                origin.y += center.y;
                Rect end = blocksToAssign[blockCount - 1].block.pos;
                end.x += center.x;
                end.y += center.y;
                createPosition = new Vector2(((end.x + origin.x) / 2) + (GameConstants.gridBlockWidth / 2), ((end.y + origin.y) / 2) - (GameConstants.gridBlockHeight / 2));
                return true;
            }
            else
            {
                createPosition = Vector2.zero;
                return false;
            }

        }
        else
        {
            createPosition = Vector2.zero;
            return false;
        }

    }

    public bool RectsIntersect(Rect candidate , Rect evaluatedRect , Vector2 center)
    {
        evaluatedRect.x = evaluatedRect.x + center.x;
        evaluatedRect.y = evaluatedRect.y + center.y;
        Vector2 l1 = new Vector2(candidate.x, candidate.y);
        Vector2 r1 = new Vector2(candidate.x + candidate.width, candidate.y - candidate.height);
        Vector2 l2 = new Vector2(evaluatedRect.x, evaluatedRect.y);
        Vector2 r2 = new Vector2(evaluatedRect.x + evaluatedRect.width, evaluatedRect.y - evaluatedRect.height);
        // If one rectangle is on left side of other
        if (l1.x > r2.x || l2.x > r1.x)
            return false;

        // If one rectangle is above other
        if (l1.y < r2.y || l2.y < r1.y)
            return false;

        return true;
    }

    public bool CheckIfAvailable(GridBlock block , int selectedTab)
    {
        bool result = false ;
        switch (selectedTab)
        {
            case 0:
                if (block.backgroundObject == null)
                    {
                        result =  true;
                    }
                    else
                    {
                        result =  false;
                    }break;
            case 1: if (block.groundObject == null)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    } break;
            case 2: if (block.foregroundObject == null)
                {
                    result = true;
                }
                else
                {
                    result = false;
                } break;
        }
        return result;
    }

    public void AddReferenceToBlock(GameObject go, int selectedTab)
    {
        List<Coordinate> coordList = new List<Coordinate>();
        int k;
        switch (selectedTab)
        {
            case 0:
                for (k = 0; k < blockCount; k++)
                {
                    blocksToAssign[k].block.backgroundObject = go;
                    coordList.Add(blocksToAssign[k].coordinate);

                } break;
            case 1:
                for (k = 0; k < blockCount; k++)
                {
                    blocksToAssign[k].block.groundObject = go;
                    coordList.Add(blocksToAssign[k].coordinate);
                } break;
            case 2:
                for (k = 0; k < blockCount; k++)
                {
                    blocksToAssign[k].block.foregroundObject = go;
                    coordList.Add(blocksToAssign[k].coordinate);
                } break;
        }
        go.GetComponent<GridPosition>().SetBlockReference(coordList, selectedTab, this, blockCount);
        AddObjectToHeiarchy(go);
        AddToUndoStack("CR", go.transform.position, go.tag, roomHeight, roomWidth, blockCount, coordList, selectedTab);

    }

    public void AddObjectToHeiarchy(GameObject go)
    {
        if (go.tag.Equals("Enemy"))
        {
            go.transform.SetParent(gameObject.transform.Find("Enemies"));
        }
        else
        {
            go.transform.SetParent(gameObject.transform.Find("Environment"));
        }
    }

    #endregion

    #region GridMantainance

    public void ResizeRoom(int newHeight , int newWidth)
    {
        AddToUndoStack("BRS", Vector2.zero, "", roomHeight, roomWidth, 0, new List<Coordinate>(), -1);
        BoxCollider2D c2d = GetComponent<BoxCollider2D>();
        GridRow[] auxGrid2 = new GridRow[newHeight];
        for (int x = 0; x < newHeight; x++)
        {
            auxGrid2[x] = new GridRow();
            for (int y = 0; y < newWidth; y++)
            {
                auxGrid2[x].cells = new GridBlock[newWidth];
            }
        }
        GridBlock aux;
        Transform gridBlockGroupT = transform.Find("GridBlockGroup");
        GameObject gridBlockGroup;
        if (gridBlockGroupT == null)
        {
            gridBlockGroup = new GameObject("GridBlockGroup");
            gridBlockGroup.transform.parent = gameObject.transform;
        }
        else
        {
            gridBlockGroup = gridBlockGroupT.gameObject;
        }
        gridBlockGroup.transform.localPosition = Vector2.zero;
        Transform boundary = transform.Find("Playable Boundary");
        Transform leftW = boundary.gameObject.transform.Find("Left Wall");
        Transform topW = boundary.gameObject.transform.Find("Top Wall");
        topLeftCorner = new Vector2(leftW.gameObject.transform.localPosition.x, topW.gameObject.transform.localPosition.y);
        SpriteRenderer sr;
        for (int i = 0; i < newHeight; i++)
        {
            for (int j = 0; j < newWidth; j++)
            {
                if((i<roomHeight) && (j<roomWidth))
                {

                    auxGrid2[i].cells[j] = gridMatrix[i].cells[j];
                }
                else
                {
                    auxGrid2[i].cells[j] = new GridBlock();
                    GridBlock auxblock = auxGrid2[i].cells[j];
                    auxblock.pos.x = topLeftCorner.x + ((auxblock.pos.width) * j);
                    auxblock.pos.y = topLeftCorner.y - ((auxblock.pos.height) * i);
                    GameObject spriteBlock = new GameObject("GridBlock");
                    spriteBlock.tag = "GridBlock";
                    spriteBlock.AddComponent<SpriteRenderer>();
                    sr = spriteBlock.GetComponent<SpriteRenderer>();
                    sr.sprite = Resources.Load("Sprites/LevelConstructionSprites/GreenGridSMALL", typeof(Sprite)) as Sprite;
                    spriteBlock.transform.parent = gridBlockGroup.transform;
                    spriteBlock.transform.localPosition = new Vector2(auxblock.pos.x + (auxblock.pos.width / 2), auxblock.pos.y - (auxblock.pos.height / 2));
                    auxblock.blockSprite = sr;
                }
                
            }
        }

        if(newHeight<roomHeight || newWidth<roomWidth)
        {
            for (int i = 0; i < roomHeight; i++)
            {
                for (int j = 0; j < roomWidth; j++)
                {
                    if(j>newWidth-1 || i>newHeight-1)
                    {
                        aux = gridMatrix[i].cells[j];
                        if (aux.backgroundObject != null)
                        {
                            DestroyImmediate(aux.backgroundObject);
                        }
                        aux.backgroundObject = null;
                        if (aux.groundObject != null)
                        {
                            DestroyImmediate(aux.groundObject);
                        }
                        aux.groundObject = null;
                        if (aux.foregroundObject != null)
                        {
                            DestroyImmediate(aux.foregroundObject);
                        }
                        aux.foregroundObject = null;
                        DestroyImmediate(aux.blockSprite.gameObject);
                    }

                }
            }
        }
        Transform rightW = boundary.gameObject.transform.Find("Right Wall");
        Transform botW = boundary.gameObject.transform.Find("Bot Wall");
        rightW.localPosition = new Vector2(topLeftCorner.x + (newWidth * GameConstants.gridBlockWidth), rightW.localPosition.y);
        botW.localPosition = new Vector2(botW.localPosition.x, topLeftCorner.y - (newHeight * GameConstants.gridBlockHeight));
        AddToUndoStack("ERS", Vector2.zero, "", roomHeight, roomWidth, 0, new List<Coordinate>(), -1);
        roomHeight = newHeight;
        roomWidth = newWidth;

        Vector2 size = c2d.size;
        size.x = GameConstants.gridBlockWidth * newWidth;
        size.y = GameConstants.gridBlockHeight * newHeight;
        c2d.size = size;

        Vector2 offset = c2d.offset;
        offset.x = (0.5f * (roomWidth - GameConstants.minimalRoomWidth) * GameConstants.gridBlockWidth);
        offset.y = (-0.5f * (roomHeight - GameConstants.minimalRoomHeight) * GameConstants.gridBlockHeight);
        c2d.offset = offset;

        gridMatrix = null;
        gridMatrix = auxGrid2;

    }

    public void EraseBlock(Rect eraseRect, int selectedTab, Vector2 center)
    {
        List<Coordinate> auxCoord = new List<Coordinate>();
        int blockCount;
        GridBlock aux;
        for (int i = 0; i < roomHeight; i++)
        {
            for (int j = 0; j < roomWidth; j++)
            {
                if (RectsIntersect(eraseRect, gridMatrix[i].cells[j].pos, center))
                {
                    if (!CheckIfAvailable(gridMatrix[i].cells[j], selectedTab))
                    {
                        aux = gridMatrix[i].cells[j];
                        if (aux != null)
                        {
                            switch (selectedTab)
                            {
                                case 0:
                                    auxCoord = FindReferencesOfObject(aux.backgroundObject, selectedTab, out blockCount);
                                    AddToUndoStack("ER", aux.backgroundObject.transform.position, aux.backgroundObject.tag, roomHeight, roomWidth, blockCount, auxCoord, selectedTab);
                                    DestroyImmediate(aux.backgroundObject.gameObject);
                                    aux.backgroundObject = null;
                                    break;
                                case 1:
                                    auxCoord = FindReferencesOfObject(aux.groundObject, selectedTab, out blockCount);
                                    AddToUndoStack("ER", aux.groundObject.transform.position, aux.groundObject.tag, roomHeight, roomWidth, blockCount, auxCoord, selectedTab);
                                    DestroyImmediate(aux.groundObject.gameObject);
                                    aux.groundObject = null;
                                    break;
                                case 2:
                                    auxCoord = FindReferencesOfObject(aux.foregroundObject, selectedTab, out blockCount);
                                    AddToUndoStack("ER", aux.foregroundObject.transform.position, aux.foregroundObject.tag, roomHeight, roomWidth, blockCount, auxCoord, selectedTab);
                                    DestroyImmediate(aux.foregroundObject.gameObject);
                                    aux.foregroundObject = null;
                                    break;
                            }
                        }
                        i = roomHeight;
                        break;
                    }
                    else
                    {
                        i = roomHeight;
                        break;
                    }
                }
            }
        }
    }

    public void ClearRoom()
    {
        AddToUndoStack("BCL", Vector2.zero, "", 0, 0, 0, new List<Coordinate>(), 0);
        GridBlock aux;
        List<Coordinate> auxCoord = new List<Coordinate>();
        int blockCount;
        for (int i = 0; i < roomHeight; i++)
        {
            for (int j = 0; j < roomWidth; j++)
            {
                aux = gridMatrix[i].cells[j];
                if (aux != null)
                {
                    if (aux.backgroundObject != null)
                    {
                        auxCoord = FindReferencesOfObject(aux.backgroundObject, 0, out blockCount);
                        AddToUndoStack("ER", aux.backgroundObject.transform.position, aux.backgroundObject.tag, roomHeight, roomWidth, blockCount, auxCoord, 0);
                        DestroyImmediate(aux.backgroundObject.gameObject);
                    }
                    aux.backgroundObject = null;
                    if (aux.groundObject != null)
                    {
                        auxCoord = FindReferencesOfObject(aux.groundObject, 1, out blockCount);
                        AddToUndoStack("ER", aux.groundObject.transform.position, aux.groundObject.tag, roomHeight, roomWidth, blockCount, auxCoord, 1);
                        DestroyImmediate(aux.groundObject.gameObject);
                    }
                    aux.groundObject = null;
                    if (aux.foregroundObject != null)
                    {
                        auxCoord = FindReferencesOfObject(aux.foregroundObject, 1, out blockCount);
                        AddToUndoStack("ER", aux.foregroundObject.transform.position, aux.foregroundObject.tag, roomHeight, roomWidth, blockCount, auxCoord, 1);
                        DestroyImmediate(aux.foregroundObject.gameObject);
                    }
                    aux.foregroundObject = null;
                }
            }
        }
        AddToUndoStack("ECL", Vector2.zero, "", 0, 0, 0, new List<Coordinate>(), 0);
    }

    public void AddToUndoStack(string actionType, Vector2 pos, string objectTag, int height, int width, int blockCount, List<Coordinate> coordinates, int tabIndex)
    {
        StackData aux = new StackData();
        aux.actionType = actionType;
        aux.position = pos;
        aux.objectTag = objectTag;
        aux.height = height;
        aux.width = width;
        aux.blockCount = blockCount;
        aux.coorndinates = coordinates;
        aux.tabIndex = tabIndex;
        undoStack.Add(aux);
        redoStack.Clear();
    }

    public void AddToUndoStack(StackData sd)
    {
        undoStack.Add(sd);
    }

    public void AddToRedoStack(StackData data)
    {
        redoStack.Add(data);
    }

    public void Undo()
    {
        if (undoStack.Count > 0)
        {
            StackData aux = undoStack[undoStack.Count - 1];
            undoStack.RemoveAt(undoStack.Count - 1);
            GameObject auxGo = null;
            switch (aux.actionType)
            {
                case "CR":
                    for (int i = 0; i < aux.blockCount; i++)
                    {
                        switch (aux.tabIndex)
                        {
                            case 0:
                                if (i == 0)
                                {
                                    DestroyImmediate(gridMatrix[aux.coorndinates[i].y].cells[aux.coorndinates[i].x].backgroundObject);
                                }
                                else
                                {
                                    gridMatrix[aux.coorndinates[i].y].cells[aux.coorndinates[i].x].backgroundObject = null;
                                }
                                break;
                            case 1:
                                if (i == 0)
                                {
                                    DestroyImmediate(gridMatrix[aux.coorndinates[0].y].cells[aux.coorndinates[0].x].groundObject);
                                }
                                else
                                {
                                    gridMatrix[aux.coorndinates[i].y].cells[aux.coorndinates[i].x].groundObject = null;
                                }
                                break;
                            case 2:
                                if (i == 0)
                                {
                                    DestroyImmediate(gridMatrix[aux.coorndinates[0].y].cells[aux.coorndinates[0].x].foregroundObject);
                                }
                                else
                                {
                                    gridMatrix[aux.coorndinates[i].y].cells[aux.coorndinates[i].x].foregroundObject = null;
                                }
                                break;
                        }
                    }
                    break;
                case "ER":
                    for (int i = 0; i < aux.blockCount; i++)
                    {
                        switch (aux.tabIndex)
                        {
                            case 0:
                                if (i == 0)
                                {
                                    GameObject metaTile = (GameObject)Instantiate(GetGO(aux.objectTag));
                                    metaTile.transform.position = aux.position;
                                    gridMatrix[aux.coorndinates[i].y].cells[aux.coorndinates[i].x].backgroundObject = metaTile;
                                    AddObjectToHeiarchy(metaTile);
                                    auxGo = metaTile;
                                }
                                else
                                {
                                    gridMatrix[aux.coorndinates[i].y].cells[aux.coorndinates[i].x].backgroundObject = auxGo;
                                }
                                break;
                            case 1:
                                if (i == 0)
                                {
                                    GameObject metaTile = (GameObject)Instantiate(GetGO(aux.objectTag));
                                    metaTile.transform.position = aux.position;
                                    gridMatrix[aux.coorndinates[i].y].cells[aux.coorndinates[i].x].groundObject = metaTile;
                                    AddObjectToHeiarchy(metaTile);
                                    auxGo = metaTile;
                                }
                                else
                                {
                                    gridMatrix[aux.coorndinates[i].y].cells[aux.coorndinates[i].x].groundObject = auxGo;
                                }
                                break;
                            case 2:
                                if (i == 0)
                                {
                                    GameObject metaTile = (GameObject)Instantiate(GetGO(aux.objectTag));
                                    metaTile.transform.position = aux.position;
                                    gridMatrix[aux.coorndinates[i].y].cells[aux.coorndinates[i].x].foregroundObject = metaTile;
                                    AddObjectToHeiarchy(metaTile);
                                    auxGo = metaTile;
                                }
                                else
                                {
                                    gridMatrix[aux.coorndinates[i].y].cells[aux.coorndinates[i].x].foregroundObject = auxGo;
                                }
                                break;
                        }
                    }
                    break;
                case "ECL":
                    AddToRedoStack(aux);
                    aux = undoStack[undoStack.Count - 1];
                    undoStack.RemoveAt(undoStack.Count - 1);
                    while (!aux.actionType.Equals("BCL"))
                    {
                        for (int i = 0; i < aux.blockCount; i++)
                        {
                            switch (aux.tabIndex)
                            {
                                case 0:
                                    if (i == 0)
                                    {
                                        GameObject metaTile = (GameObject)Instantiate(GetGO(aux.objectTag));
                                        metaTile.transform.position = aux.position;
                                        gridMatrix[aux.coorndinates[i].y].cells[aux.coorndinates[i].x].backgroundObject = metaTile;
                                        AddObjectToHeiarchy(metaTile);
                                        auxGo = metaTile;
                                    }
                                    else
                                    {
                                        gridMatrix[aux.coorndinates[i].y].cells[aux.coorndinates[i].x].backgroundObject = auxGo;
                                    }
                                    break;
                                case 1:
                                    if (i == 0)
                                    {
                                        GameObject metaTile = (GameObject)Instantiate(GetGO(aux.objectTag));
                                        metaTile.transform.position = aux.position;
                                        gridMatrix[aux.coorndinates[i].y].cells[aux.coorndinates[i].x].groundObject = metaTile;
                                        AddObjectToHeiarchy(metaTile);
                                        auxGo = metaTile;
                                    }
                                    else
                                    {
                                        gridMatrix[aux.coorndinates[i].y].cells[aux.coorndinates[i].x].groundObject = auxGo;
                                    }
                                    break;
                                case 2:
                                    if (i == 0)
                                    {
                                        GameObject metaTile = (GameObject)Instantiate(GetGO(aux.objectTag));
                                        metaTile.transform.position = aux.position;
                                        gridMatrix[aux.coorndinates[i].y].cells[aux.coorndinates[i].x].foregroundObject = metaTile;
                                        AddObjectToHeiarchy(metaTile);
                                        auxGo = metaTile;
                                    }
                                    else
                                    {
                                        gridMatrix[aux.coorndinates[i].y].cells[aux.coorndinates[i].x].foregroundObject = auxGo;
                                    }
                                    break;
                            }
                        }
                        AddToRedoStack(aux);
                        aux = undoStack[undoStack.Count - 1];
                        undoStack.RemoveAt(undoStack.Count - 1);
                    }
                    break;
            }
            AddToRedoStack(aux);
        }

    }

    public void Redo()
    {
        if (redoStack.Count > 0)
        {
            GameObject auxGo = null;
            StackData aux = redoStack[redoStack.Count - 1];
            redoStack.RemoveAt(redoStack.Count - 1);
            switch (aux.actionType)
            {
                case "CR":
                    for (int i = 0; i < aux.blockCount; i++)
                    {

                        switch (aux.tabIndex)
                        {
                            case 0:
                                if (i == 0)
                                {
                                    GameObject metaTile = (GameObject)Instantiate(GetGO(aux.objectTag));
                                    metaTile.transform.position = aux.position;
                                    gridMatrix[aux.coorndinates[i].y].cells[aux.coorndinates[i].x].backgroundObject = metaTile;
                                    AddObjectToHeiarchy(metaTile);
                                    auxGo = metaTile;
                                }
                                else
                                {
                                    gridMatrix[aux.coorndinates[i].y].cells[aux.coorndinates[i].x].backgroundObject = auxGo;
                                }
                                break;
                            case 1:
                                if (i == 0)
                                {
                                    GameObject metaTile = (GameObject)Instantiate(GetGO(aux.objectTag));
                                    metaTile.transform.position = aux.position;
                                    gridMatrix[aux.coorndinates[i].y].cells[aux.coorndinates[i].x].groundObject = metaTile;
                                    AddObjectToHeiarchy(metaTile);
                                    auxGo = metaTile;
                                }
                                else
                                {
                                    gridMatrix[aux.coorndinates[i].y].cells[aux.coorndinates[i].x].groundObject = auxGo;
                                }
                                break;
                            case 2:
                                if (i == 0)
                                {
                                    GameObject metaTile = (GameObject)Instantiate(GetGO(aux.objectTag));
                                    metaTile.transform.position = aux.position;
                                    gridMatrix[aux.coorndinates[i].y].cells[aux.coorndinates[i].x].foregroundObject = metaTile;
                                    AddObjectToHeiarchy(metaTile);
                                    auxGo = metaTile;
                                }
                                else
                                {
                                    gridMatrix[aux.coorndinates[i].y].cells[aux.coorndinates[i].x].foregroundObject = auxGo;
                                }
                                break;
                        }
                    }
                    break;
                case "ER":
                    for (int i = 0; i < aux.blockCount; i++)
                    {
                        switch (aux.tabIndex)
                        {
                            case 0:
                                if (i == 0)
                                {
                                    DestroyImmediate(gridMatrix[aux.coorndinates[i].y].cells[aux.coorndinates[i].x].backgroundObject);
                                }
                                else
                                {
                                    gridMatrix[aux.coorndinates[i].y].cells[aux.coorndinates[i].x].backgroundObject = null;
                                }
                                break;
                            case 1:
                                if (i == 0)
                                {
                                    DestroyImmediate(gridMatrix[aux.coorndinates[0].y].cells[aux.coorndinates[0].x].groundObject);
                                }
                                else
                                {
                                    gridMatrix[aux.coorndinates[i].y].cells[aux.coorndinates[i].x].groundObject = null;
                                }
                                break;
                            case 2:
                                if (i == 0)
                                {
                                    DestroyImmediate(gridMatrix[aux.coorndinates[0].y].cells[aux.coorndinates[0].x].foregroundObject);
                                }
                                else
                                {
                                    gridMatrix[aux.coorndinates[i].y].cells[aux.coorndinates[i].x].foregroundObject = null;
                                }
                                break;
                        }
                    }
                    break;
                case "BCL":
                    AddToUndoStack(aux);
                    aux = redoStack[redoStack.Count - 1];
                    redoStack.RemoveAt(redoStack.Count - 1);
                    while (!aux.actionType.Equals("ECL"))
                    {
                        for (int i = 0; i < aux.blockCount; i++)
                        {
                            switch (aux.tabIndex)
                            {
                                case 0:
                                    if (i == 0)
                                    {
                                        DestroyImmediate(gridMatrix[aux.coorndinates[i].y].cells[aux.coorndinates[i].x].backgroundObject);
                                    }
                                    else
                                    {
                                        gridMatrix[aux.coorndinates[i].y].cells[aux.coorndinates[i].x].backgroundObject = null;
                                    }
                                    break;
                                case 1:
                                    if (i == 0)
                                    {
                                        DestroyImmediate(gridMatrix[aux.coorndinates[0].y].cells[aux.coorndinates[0].x].groundObject);
                                    }
                                    else
                                    {
                                        gridMatrix[aux.coorndinates[i].y].cells[aux.coorndinates[i].x].groundObject = null;
                                    }
                                    break;
                                case 2:
                                    if (i == 0)
                                    {
                                        DestroyImmediate(gridMatrix[aux.coorndinates[0].y].cells[aux.coorndinates[0].x].foregroundObject);
                                    }
                                    else
                                    {
                                        gridMatrix[aux.coorndinates[i].y].cells[aux.coorndinates[i].x].foregroundObject = null;
                                    }
                                    break;
                            }
                        }
                        AddToUndoStack(aux);
                        aux = redoStack[redoStack.Count - 1];
                        redoStack.RemoveAt(redoStack.Count - 1);
                    }
                    break;
            }
            AddToUndoStack(aux);
        }

    }

    public GameObject GetGO(string tag)
    {
        GameObject toReturn = null;
        switch (tag)
        {
            case "Carro": toReturn = Resources.Load("Prefabs/BackGroundItems/Carro") as GameObject; break;
            case "Truck": toReturn = Resources.Load("Prefabs/BackGroundItems/TruckAndTrunksH") as GameObject; break;
            case "Draggable": toReturn = Resources.Load("Prefabs/GroundItems/Box") as GameObject; break;
            case "Creature": toReturn = Resources.Load("Prefabs/GroundItems/Creature") as GameObject; break;
            case "DestructibleWall": toReturn = Resources.Load("Prefabs/GroundItems/DestructibleWall") as GameObject; break;
            case "Door": toReturn = Resources.Load("Prefabs/GroundItems/Door") as GameObject; break;
            case "DragonFly": toReturn = Resources.Load("Prefabs/GroundItems/Dragon Fly") as GameObject; break;
            case "Enemy": toReturn = Resources.Load("Prefabs/GroundItems/Enemy") as GameObject; break;
            case "Food": toReturn = Resources.Load("Prefabs/GroundItems/Food") as GameObject; break;
            case "Hazard": toReturn = Resources.Load("Prefabs/GroundItems/Hazard") as GameObject; break;
            case "Hinge": toReturn = Resources.Load("Prefabs/GroundItems/Hinge") as GameObject; break;
            case "Hole": toReturn = Resources.Load("Prefabs/GroundItems/Hole") as GameObject; break;
            case "Lever": toReturn = Resources.Load("Prefabs/GroundItems/Lever") as GameObject; break;
            case "Trap": toReturn = Resources.Load("Prefabs/GroundItems/MovingTrap") as GameObject; break;
            case "Plank": toReturn = Resources.Load("Prefabs/GroundItems/Plank") as GameObject; break;
            case "River": toReturn = Resources.Load("Prefabs/GroundItems/River") as GameObject; break;
            case "Rock": toReturn = Resources.Load("Prefabs/GroundItems/Rock") as GameObject; break;
            case "Shadow": toReturn = Resources.Load("Prefabs/ForeGroundItems/Shadow") as GameObject; break;
            case "SlowTerrain": toReturn = Resources.Load("Prefabs/GroundItems/SlowTerrain") as GameObject; break;
            case "Switch": toReturn = Resources.Load("Prefabs/GroundItems/Switch") as GameObject; break;
            case "Tree": toReturn = Resources.Load("Prefabs/GroundItems/Tree") as GameObject; break;
        }
        return toReturn;
    }

    public void DisableGridSprites()
    {
        for (int i = 0; i < roomHeight; i++)
        {
            for (int j = 0; j < roomWidth; j++)
            {
                SpriteRenderer gs = gridMatrix[i].cells[j].blockSprite;
                if (gs != null)
                {
                    gs.enabled = false;
                }
            }
        }
    }

    public void PeekStacks()
    {
        if (undoStack.Count > 0)
        {
            Debug.Log("[" + undoStack[undoStack.Count - 1].coorndinates[0].x + "," + undoStack[undoStack.Count - 1].coorndinates[0].y + "] ACTIONTTAG " + undoStack[undoStack.Count - 1].actionType);

        }
        else
        {
            Debug.Log("UNDO STACK EMPTY");
        }
        if (redoStack.Count > 0)
        {
            Debug.Log("[" + redoStack[redoStack.Count - 1].coorndinates[0].x + "," + redoStack[redoStack.Count - 1].coorndinates[0].y + "] ACTIONTTAG " + redoStack[redoStack.Count - 1].actionType);
        }
        else
        {
            Debug.Log("REDO STACK EMPTY");
        }
    }

    public bool CheckAvailableSpace(int x, int y, int tab, int instanceIDOfRequester, bool targetIsObject = false , int instanceIDOfTarget = -1)
    {

        bool available = false;
        if ((x >= 0 && x < roomWidth) && (y >= 0 && y < roomHeight))
        {
            switch (tab)
            {
                case 0: if (gridMatrix[y].cells[x].backgroundObject == null)
                    {
                        available = true;
                    }
                    else
                    {
                        if (gridMatrix[y].cells[x].backgroundObject.GetInstanceID() == instanceIDOfRequester)
                        {
                            available = true;
                        }
                        else
                        {
                            if (targetIsObject)
                            {
                                if (gridMatrix[y].cells[x].backgroundObject.GetInstanceID() == instanceIDOfTarget)
                                {
                                    available = true;
                                }
                            }
                        }
                    } break;
                case 1: if (gridMatrix[y].cells[x].groundObject == null)
                    {
                        available = true;
                    }
                    else
                    {
                        if (gridMatrix[y].cells[x].groundObject.GetInstanceID() == instanceIDOfRequester)
                        {
                            available = true;
                        }
                        else
                        {
                            if (targetIsObject)
                            {
                                if (gridMatrix[y].cells[x].groundObject.GetInstanceID() == instanceIDOfTarget)
                                {
                                    available = true;
                                }
                            }
                        }
                    }
                    break;
                case 2: if (gridMatrix[y].cells[x].foregroundObject == null)
                    {
                        available = true;
                    }
                    else
                    {
                        if (gridMatrix[y].cells[x].foregroundObject.GetInstanceID() == instanceIDOfRequester)
                        {
                            available = true;
                        }
                        else
                        {
                            if (targetIsObject)
                            {
                                if (gridMatrix[y].cells[x].foregroundObject.GetInstanceID() == instanceIDOfTarget)
                                {
                                    available = true;
                                }
                            }
                        }
                    }
                    break;
            }
        }
        return available;


    }

    public void ClearReferenceOfObject(int x, int y, int tab)
    {
        if ((x >= 0 && x < roomWidth) && (y >= 0 && y < roomHeight))
        {
            switch (tab)
            {
                case 0: gridMatrix[y].cells[x].backgroundObject = null; break;
                case 1: gridMatrix[y].cells[x].groundObject = null;
                    gridMatrix[y].cells[x].blockSprite.sprite = Resources.Load("Sprites/LevelConstructionSprites/GreenGridSMALL", typeof(Sprite)) as Sprite; break;
                case 2: gridMatrix[y].cells[x].foregroundObject = null; break;
            }
        }

    }

    public void SetReferenceOfObject(int x, int y, int tab, GameObject go)
    {
        if ((x >= 0 && x < roomWidth) && (y >= 0 && y < roomHeight))
        {
            switch (tab)
            {
                case 0: gridMatrix[y].cells[x].backgroundObject = go; break;
                case 1: gridMatrix[y].cells[x].groundObject = go; break;
                    gridMatrix[y].cells[x].blockSprite.sprite = Resources.Load("Sprites/LevelConstructionSprites/BlueGridSMALL", typeof(Sprite)) as Sprite; break;
                case 2: gridMatrix[y].cells[x].foregroundObject = go; break;
            }
        }
    }

    public void UpdateGridReferences()
    {
        int blockCount;
        List<Coordinate> coordinates;
        for (int i = 0; i < roomHeight; i++)
        {
            for (int j = 0; j < roomWidth; j++)
            {
                if (gridMatrix[i].cells[j].backgroundObject != null)
                {
                    coordinates = FindReferencesOfObject(gridMatrix[i].cells[j].backgroundObject, 0, out blockCount);
                    if (gridMatrix[i].cells[j].backgroundObject.GetComponent<GridPosition>() == null)
                    {
                        gridMatrix[i].cells[j].backgroundObject.AddComponent<GridPosition>();
                    }
                    gridMatrix[i].cells[j].backgroundObject.GetComponent<GridPosition>().SetBlockReference(coordinates, 0, this, blockCount);
                }

                if (gridMatrix[i].cells[j].groundObject != null)
                {
                    coordinates = FindReferencesOfObject(gridMatrix[i].cells[j].groundObject, 1, out blockCount);
                    if (gridMatrix[i].cells[j].groundObject.GetComponent<GridPosition>() == null)
                    {
                        gridMatrix[i].cells[j].groundObject.AddComponent<GridPosition>();
                    }
                    gridMatrix[i].cells[j].groundObject.GetComponent<GridPosition>().SetBlockReference(coordinates, 1, this, blockCount);
                }

                if (gridMatrix[i].cells[j].foregroundObject != null)
                {
                    coordinates = FindReferencesOfObject(gridMatrix[i].cells[j].foregroundObject, 2, out blockCount);
                    if (gridMatrix[i].cells[j].foregroundObject.GetComponent<GridPosition>() == null)
                    {
                        gridMatrix[i].cells[j].foregroundObject.AddComponent<GridPosition>();
                    }
                    gridMatrix[i].cells[j].foregroundObject.GetComponent<GridPosition>().SetBlockReference(coordinates, 2, this, blockCount);
                }
            }
        }
        GetTopLeftCorner();
    }

    #endregion


    public List<Coordinate> FindReferencesOfObject(GameObject GO, int selectedTab, out int blockCountEx)
    {
        List<Coordinate> auxCoord = new List<Coordinate>();
        int blockCount=0;
        for(int i=0;i<roomHeight;i++)
        {
            for(int j=0;j<roomWidth;j++)
            {
                switch(selectedTab)
                {
                    case 0 :
                        if (gridMatrix[i].cells[j].backgroundObject!=null)
                        {

                            if (gridMatrix[i].cells[j].backgroundObject.GetInstanceID().Equals(GO.GetInstanceID()))
                            {
                                Coordinate coord = new Coordinate(j, i);
                                auxCoord.Add(coord);
                                blockCount++;
                            }
                        }break;
                    case 1:
                        if(gridMatrix[i].cells[j].groundObject!=null)
                        {
                            if (gridMatrix[i].cells[j].groundObject.GetInstanceID().Equals(GO.GetInstanceID()))
                            {
                                Coordinate coord = new Coordinate(j, i);
                                auxCoord.Add(coord);
                                blockCount++;
                            } 
                        }
                        break;
                    case 2:
                        if (gridMatrix[i].cells[j].foregroundObject != null)
                        {
                            if (gridMatrix[i].cells[j].foregroundObject.GetInstanceID().Equals(GO.GetInstanceID()))
                            {
                                Coordinate coord = new Coordinate(j, i);
                                auxCoord.Add(coord);
                                blockCount++;
                            }
                        }
                        break;
                }
            }
        }
        blockCountEx = blockCount;
        return auxCoord;
    }    

    public Coordinate ReturnCoordinatesOfObject(Vector2 pos)
    {
        Coordinate objectCoord=new Coordinate();
        objectCoord.x=Mathf.FloorToInt((Mathf.Abs(pos.x-topLeftCorner.x)/GameConstants.gridBlockWidth));
        objectCoord.y=Mathf.FloorToInt((Mathf.Abs(pos.y - topLeftCorner.y) / GameConstants.gridBlockHeight));
        return objectCoord;
    }

    public void GetTopLeftCorner()
    {
        Transform boundary = transform.Find("Playable Boundary");
        Transform leftW = boundary.gameObject.transform.Find("Left Wall");
        Transform topW = boundary.gameObject.transform.Find("Top Wall");
        topLeftCorner = new Vector2(leftW.gameObject.transform.localPosition.x, topW.gameObject.transform.localPosition.y);
    }

    public Vector2 GetCenterPosOfSingleCoordinate(Coordinate coord)
    {
        Vector2 pos;
        float x, y;
        x = topLeftCorner.x + ((coord.x * GameConstants.gridBlockWidth) + (GameConstants.gridBlockWidth / 2));
        y = topLeftCorner.y - (((coord.y+1) * GameConstants.gridBlockHeight) - (GameConstants.gridBlockHeight / 2));
        pos = new Vector2(x, y);
        return pos;
    }

    public Vector2 GetCenterPosOfGroupCoordinates(Coordinate coordinates,float xDim,float yDim)
    {
        Vector2 pos;
        float x, y;
        x = topLeftCorner.x + ((coordinates.x * GameConstants.gridBlockWidth) + ((GameConstants.gridBlockWidth / 2)*xDim));
        y = topLeftCorner.y - ((coordinates.y * GameConstants.gridBlockWidth) + ((GameConstants.gridBlockHeight / 2) * yDim));
        pos = new Vector2(x, y);
        return pos;
    }

    public void MarkAllOccupiedBlocks()
    {
        for (int i = 0; i < roomHeight; i++)
        {
            for (int j = 0; j < roomWidth; j++)
            {
                if (gridMatrix[i].cells[j].groundObject != null)
                {
                    gridMatrix[i].cells[j].blockSprite.sprite = blueBlock;
                }                
            }
        }
    }

    public List<Coordinate> findCoordinatesOfObjectType(string tag)
    {
        List<Coordinate> coordinates = new List<Coordinate>();
        List<int> acquiredObject = new List<int>();
        for(int i=0;i<roomHeight;i++)
        {
            for(int j=0;j<roomWidth;j++)
            {
                if(gridMatrix[i].cells[j].groundObject!=null)
                {
                    if (gridMatrix[i].cells[j].groundObject.tag == tag && (!acquiredObject.Contains(gridMatrix[i].cells[j].groundObject.GetInstanceID())))
                    {
                        coordinates.Add(new Coordinate(j, i));
                        acquiredObject.Add(gridMatrix[i].cells[j].groundObject.GetInstanceID());
                    }

                }
            }
        }

        return coordinates;
    }

    public int RoomHeight
    {
        get
        {
            return roomHeight;
        }
    }
    public int RoomWidth
    {
        get
        {
            return roomWidth;
        }
    }

    public List<Coordinate> BushCoordinates
    {
        get
        {
            return bushCoordinates;
        }
    }

}
