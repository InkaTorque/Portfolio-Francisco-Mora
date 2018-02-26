using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridPosition : MonoBehaviour {
    [SerializeField]
    public  List<Coordinate> currentBlockGroup;
    [HideInInspector][SerializeField]
    public int type;
    [HideInInspector][SerializeField]
    public int count;
    [SerializeField][HideInInspector]
    public RoomGrid roomGrid;
    [SerializeField]
    public float xDim, yDim;

    public void SetBlockReference(List<Coordinate> blocks, int tab, RoomGrid rg, int blockCount)
    {
        currentBlockGroup = blocks;
        type = tab;
        roomGrid = rg;
        count = blockCount;
        xDim = (transform.Find("EditorHitbox").GetComponent<BoxCollider2D>().size.x / GameConstants.gridBlockWidth);
        yDim = (transform.Find("EditorHitbox").GetComponent<BoxCollider2D>().size.y / GameConstants.gridBlockHeight);
    }
    public void SetBlockReferencePlayer(List<Coordinate> blocks, int tab, RoomGrid rg, int blockCount , float xdimen,float ydimen)
    {
        currentBlockGroup = blocks;
        type = tab;
        roomGrid = rg;
        count = blockCount;
        xDim = xdimen;
        yDim = ydimen;
    }

    public void EditorMoveRight()
    {
        //Revisar si arriba esta libre
        Vector2 destination;
        if(CheckIfRoomAvailable(1,out destination))
        {
            //moverlo
            gameObject.transform.position = new Vector3(transform.position.x + GameConstants.gridBlockWidth, transform.position.y, transform.position.z);
            //actualizar referencia
            UpdateGridReferenceByEditorMovement(1);
        }
        
    }

    public void EditorMoveLeft()
    {
        //Revisar si arriba esta libre  
        Vector2 destination;
        if (CheckIfRoomAvailable(3, out destination))
        {
            //moverlo
            gameObject.transform.position = new Vector3(transform.position.x - GameConstants.gridBlockWidth, transform.position.y, transform.position.z);
            //actualizar referencia
            UpdateGridReferenceByEditorMovement(3);
        }
    }
    public void EditorMoveUp()
    {
        //Revisar si arriba esta libre 
        Vector2 destination;
        if (CheckIfRoomAvailable(0, out destination))
        {
            //moverlo
            gameObject.transform.position = new Vector3(transform.position.x, transform.position.y + GameConstants.gridBlockHeight, transform.position.z);
            //actualizar referencia
            UpdateGridReferenceByEditorMovement(0);
        }
    }
    public void EditorMoveDown()
    {
        //Revisar si arriba esta libre  
        Vector2 destination;
        if (CheckIfRoomAvailable(2, out destination))
        {
            //moverlo
            gameObject.transform.position = new Vector3(transform.position.x , transform.position.y-GameConstants.gridBlockHeight, transform.position.z);
            //actualizar referencia
            UpdateGridReferenceByEditorMovement(2);
        }
    }

    //0 up , 1 right , 2 down , 3 left
    public void UpdateGridReferenceByEditorMovement(int direction)
    {
        ClearReferenceOfObjectInGrid();
        AssignReferenceOfObjectInGridNxtPositionEditor(direction);
        UpdateCoordinates(direction);
    }

    public void ClearReferenceOfObjectInGrid()
    {
        Coordinate aux;
        for (int j = 0; j < count; j++)
        {
            aux = currentBlockGroup[j];
            roomGrid.ClearReferenceOfObject(aux.x, aux.y, type);
        }
    }
    public void ClearReferenceOfObjectInGridDiagonal(int direction)
    {
        Coordinate aux;
        switch(direction)
        {
            case 4: for (int j = 0; j < count; j++)
                {
                    aux = currentBlockGroup[j];
                    roomGrid.ClearReferenceOfObject(aux.x, aux.y, type);
                    roomGrid.ClearReferenceOfObject(aux.x + 1, aux.y, type);
                    roomGrid.ClearReferenceOfObject(aux.x, aux.y-1, type);
                } break;
            case 5: for (int j = 0; j < count; j++)
                {
                    aux = currentBlockGroup[j];
                    roomGrid.ClearReferenceOfObject(aux.x, aux.y, type);
                    roomGrid.ClearReferenceOfObject(aux.x + 1, aux.y, type);
                    roomGrid.ClearReferenceOfObject(aux.x, aux.y + 1, type);
                } break;
            case 6: for (int j = 0; j < count; j++)
                {
                    aux = currentBlockGroup[j];
                    roomGrid.ClearReferenceOfObject(aux.x, aux.y, type);
                    roomGrid.ClearReferenceOfObject(aux.x - 1, aux.y, type);
                    roomGrid.ClearReferenceOfObject(aux.x, aux.y + 1, type);
                } break;
            case 7: for (int j = 0; j < count; j++)
                {
                    aux = currentBlockGroup[j];
                    roomGrid.ClearReferenceOfObject(aux.x, aux.y, type);
                    roomGrid.ClearReferenceOfObject(aux.x - 1, aux.y, type);
                    roomGrid.ClearReferenceOfObject(aux.x, aux.y - 1, type);
                } break;
        }
        
    }

    public void AssignReferenceOfObjectInGridNxtPositionEditor(int direction)
    {
        Coordinate aux;
        for (int j = 0; j < count; j++)
        {
            aux = currentBlockGroup[j];
            switch (direction)
            {
                case 0: roomGrid.SetReferenceOfObject(aux.x, aux.y - 1, type, gameObject);break;
                case 1: roomGrid.SetReferenceOfObject(aux.x + 1, aux.y, type, gameObject);break;
                case 2: roomGrid.SetReferenceOfObject(aux.x, aux.y + 1, type, gameObject);break;
                case 3: roomGrid.SetReferenceOfObject(aux.x - 1, aux.y, type, gameObject);break;
                }
        }
    }

    public void AssignReferenceOfObjectInGridNxtPositionTraversal(int direction)
    {
        Coordinate aux;
        for (int j = 0; j < count; j++)
        {
            aux = currentBlockGroup[j];
            switch (direction)
            {
                case 0: roomGrid.SetReferenceOfObject(aux.x, aux.y - 1, type, gameObject); break;
                case 1: roomGrid.SetReferenceOfObject(aux.x + 1, aux.y, type, gameObject); break;
                case 2: roomGrid.SetReferenceOfObject(aux.x, aux.y + 1, type, gameObject); break;
                case 3: roomGrid.SetReferenceOfObject(aux.x - 1, aux.y, type, gameObject); break;
                case 4: roomGrid.SetReferenceOfObject(aux.x + 1, aux.y - 1, type, gameObject);
                    roomGrid.SetReferenceOfObject(aux.x + 1, aux.y, type, gameObject);
                    roomGrid.SetReferenceOfObject(aux.x, aux.y - 1, type, gameObject); break;
                case 5: roomGrid.SetReferenceOfObject(aux.x + 1, aux.y + 1, type, gameObject);
                    roomGrid.SetReferenceOfObject(aux.x + 1, aux.y, type, gameObject);
                    roomGrid.SetReferenceOfObject(aux.x, aux.y + 1, type, gameObject); break;
                case 6: roomGrid.SetReferenceOfObject(aux.x - 1, aux.y + 1, type, gameObject);
                    roomGrid.SetReferenceOfObject(aux.x - 1, aux.y, type, gameObject);
                    roomGrid.SetReferenceOfObject(aux.x, aux.y + 1, type, gameObject); break;
                case 7: roomGrid.SetReferenceOfObject(aux.x - 1, aux.y - 1, type, gameObject);
                    roomGrid.SetReferenceOfObject(aux.x - 1, aux.y, type, gameObject);
                    roomGrid.SetReferenceOfObject(aux.x, aux.y - 1, type, gameObject); break;
                default: break;
            }
        }
    }

    public void UpdateCoordinates(int direction)
    {
        Coordinate aux;
        for (int j = 0; j < count; j++)
        {
            aux = currentBlockGroup[j];
            switch (direction)
            {
                case 0: currentBlockGroup[j].y = aux.y - 1; break;
                case 1: currentBlockGroup[j].x = aux.x + 1; break;
                case 2: currentBlockGroup[j].y = aux.y + 1; break;
                case 3: currentBlockGroup[j].x = aux.x - 1; break;
                case 4: currentBlockGroup[j].y = aux.y - 1;
                        currentBlockGroup[j].x = aux.x + 1;break;
                case 5: currentBlockGroup[j].y = aux.y + 1;
                        currentBlockGroup[j].x = aux.x + 1; break;
                case 6: currentBlockGroup[j].y = aux.y + 1;
                        currentBlockGroup[j].x = aux.x - 1; break;
                case 7: currentBlockGroup[j].y = aux.y - 1;
                        currentBlockGroup[j].x = aux.x - 1; break;
            }
        }
    }

    //0 up , 1 right , 2 down , 3 left
    public bool CheckIfRoomAvailable(int direction,out Vector2 position,bool targetIsPlayer=false)
    {
        Coordinate aux;
        bool enoughRoom = true;
        Vector2 auxPos;
        switch (direction)
        {
            case 0:
                for (int i = 0; i < count; i++)
                {
                    aux = currentBlockGroup[i];
                    if (roomGrid.CheckAvailableSpace(aux.x, aux.y - 1, type, gameObject.GetInstanceID(),targetIsPlayer) == false)
                    {
                        enoughRoom = false;
                        break;
                    }
                }
                break;
            case 1:
                for (int i = 0; i < count; i++)
                {
                    aux = currentBlockGroup[i];
                    if (roomGrid.CheckAvailableSpace(aux.x + 1, aux.y, type, gameObject.GetInstanceID(),targetIsPlayer) == false)
                    {
                        enoughRoom = false;
                        break;
                    }
                } break;
            case 2:
                for (int i = 0; i < count; i++)
                {
                    aux = currentBlockGroup[i];
                    if (roomGrid.CheckAvailableSpace(aux.x, aux.y + 1, type, gameObject.GetInstanceID(),targetIsPlayer) == false)
                    {
                        enoughRoom = false;
                        break;
                    }
                } break;
            case 3:
                for (int i = 0; i < count; i++)
                {
                    aux = currentBlockGroup[i];
                    if (roomGrid.CheckAvailableSpace(aux.x - 1, aux.y, type, gameObject.GetInstanceID(), targetIsPlayer) == false)
                    {
                        enoughRoom = false;
                        break;
                    }
                } break;
            case 4: 
                for (int i = 0; i < count; i++)
                {
                    aux = currentBlockGroup[i];
                    if ((roomGrid.CheckAvailableSpace(aux.x + 1, aux.y, type, gameObject.GetInstanceID(), targetIsPlayer) == false) ||
                        (roomGrid.CheckAvailableSpace(aux.x + 1, aux.y - 1, type, gameObject.GetInstanceID(), targetIsPlayer) == false) ||
                        (roomGrid.CheckAvailableSpace(aux.x, aux.y - 1, type, gameObject.GetInstanceID(), targetIsPlayer) == false))
                    {
                        enoughRoom = false;
                        break;
                    }
                }break;
            case 5: for (int i = 0; i < count; i++)
                {
                    aux = currentBlockGroup[i];
                    if ((roomGrid.CheckAvailableSpace(aux.x + 1, aux.y, type, gameObject.GetInstanceID(), targetIsPlayer) == false) ||
                        (roomGrid.CheckAvailableSpace(aux.x + 1, aux.y + 1, type, gameObject.GetInstanceID(), targetIsPlayer) == false) ||
                        (roomGrid.CheckAvailableSpace(aux.x, aux.y + 1, type, gameObject.GetInstanceID(), targetIsPlayer) == false))
                    {
                        enoughRoom = false;
                        break;
                    }
                } break;
            case 6: for (int i = 0; i < count; i++)
                {
                    aux = currentBlockGroup[i];
                    if ((roomGrid.CheckAvailableSpace(aux.x - 1, aux.y, type, gameObject.GetInstanceID(), targetIsPlayer) == false) ||
                        (roomGrid.CheckAvailableSpace(aux.x - 1, aux.y + 1, type, gameObject.GetInstanceID(), targetIsPlayer) == false) ||
                        (roomGrid.CheckAvailableSpace(aux.x, aux.y + 1, type, gameObject.GetInstanceID(), targetIsPlayer) == false))
                    {
                        enoughRoom = false;
                        break;
                    }
                } break;
            case 7: for (int i = 0; i < count; i++)
                {
                    aux = currentBlockGroup[i];
                    if ((roomGrid.CheckAvailableSpace(aux.x - 1, aux.y, type, gameObject.GetInstanceID(), targetIsPlayer) == false) ||
                        (roomGrid.CheckAvailableSpace(aux.x - 1, aux.y - 1, type, gameObject.GetInstanceID(), targetIsPlayer) == false) ||
                        (roomGrid.CheckAvailableSpace(aux.x, aux.y - 1, type, gameObject.GetInstanceID(), targetIsPlayer) == false))
                    {
                        enoughRoom = false;
                        break;
                    }
                } break;
        }
        if(enoughRoom)
        {
            auxPos=CalculateCenterOfCoordinates(direction);
        }
        else
        {
            auxPos = new Vector2(-1,-1);
        }
        position = auxPos;
        return enoughRoom;
    }
    public Vector2 CalculateCenterOfCoordinates(int direction)
    {
        Vector2 destination;
        Coordinate[] nxtPost = new Coordinate[count];
        switch(direction)
        {
            case 0: 
                for(int i=0;i<count;i++)
                {
                    nxtPost[i]=new Coordinate();
                    nxtPost[i].x = currentBlockGroup[i].x;
                    nxtPost[i].y = currentBlockGroup[i].y-1;
                }break;
            case 1: 
                for(int i=0;i<count;i++)
                {
                    nxtPost[i] = new Coordinate();
                    nxtPost[i].x = currentBlockGroup[i].x+1;
                    nxtPost[i].y = currentBlockGroup[i].y;
                }break;
            case 2: 
                for(int i=0;i<count;i++)
                {
                    nxtPost[i] = new Coordinate();
                    nxtPost[i].x = currentBlockGroup[i].x;
                    nxtPost[i].y = currentBlockGroup[i].y+1;
                }break;
            case 3: 
                for(int i=0;i<count;i++)
                {
                    nxtPost[i] = new Coordinate();
                    nxtPost[i].x = currentBlockGroup[i].x-1;
                    nxtPost[i].y = currentBlockGroup[i].y;
                }break;
            case 4: for (int i = 0; i < count; i++)
                {
                    nxtPost[i] = new Coordinate();
                    nxtPost[i].x = currentBlockGroup[i].x + 1;
                    nxtPost[i].y = currentBlockGroup[i].y - 1;
                } break;
            case 5: for (int i = 0; i < count; i++)
                {
                    nxtPost[i] = new Coordinate();
                    nxtPost[i].x = currentBlockGroup[i].x + 1;
                    nxtPost[i].y = currentBlockGroup[i].y + 1;
                } break;
            case 6: for (int i = 0; i < count; i++)
                {
                    nxtPost[i] = new Coordinate();
                    nxtPost[i].x = currentBlockGroup[i].x - 1;
                    nxtPost[i].y = currentBlockGroup[i].y + 1;
                } break;
            case 7: for (int i = 0; i < count; i++)
                {
                    nxtPost[i] = new Coordinate();
                    nxtPost[i].x = currentBlockGroup[i].x - 1;
                    nxtPost[i].y = currentBlockGroup[i].y - 1;
                } break;
        }
        Rect origin = roomGrid.gridMatrix[nxtPost[0].y].cells[nxtPost[0].x].pos;
        Rect end = roomGrid.gridMatrix[nxtPost[count - 1].y].cells[nxtPost[count - 1].x].pos;
        end.x = end.x + end.width;
        end.y = end.y - end.height;
        destination = new Vector2(((end.x + origin.x) / 2) , ((end.y + origin.y) / 2));
        return destination;
    }

    public void AlowPlayerUpdateCoordinates()
    {
       // GetComponent<Player.Movement>().initialCoordinateSetUpDone = true;
    }

    public void UpdatePlayerPosition(int playerID,Vector2 position)
    {
        Coordinate coord = roomGrid.ReturnCoordinatesOfObject(position);
        if(coord.x!=currentBlockGroup[0].x || coord.y!=currentBlockGroup[0].y)
        {
            ClearReferenceOfObjectInGrid();
            roomGrid.SetReferenceOfObject(coord.x, coord.y, 1, gameObject);
            currentBlockGroup[0].x = coord.x;
            currentBlockGroup[0].y = coord.y;
            switch (playerID)
            {
                //case 1: PlayersController.Instance.p1Coord = coord; break;
                //case 2: PlayersController.Instance.p2Coord = coord; break;
            }

        }
    }
    public void UpdateObjectPosition(Vector2 position)
    {
        Coordinate coord = roomGrid.ReturnCoordinatesOfObject(position);
        currentBlockGroup[0].x = coord.x;
        currentBlockGroup[0].y = coord.y;
        roomGrid.SetReferenceOfObject(coord.x, coord.y, type, gameObject);
    }
    
    public void MarkPathfindingTile(int x , int y)
    {
        //roomGrid.gridMatrix[y].cells[x].blockSprite.sprite = redBlock;
    }

    public Vector2 GetCenterOfCoordinates(Coordinate coordinate)
    {
        return roomGrid.GetCenterPosOfGroupCoordinates(coordinate,xDim,yDim);
    }

    public int CalculateDirectionOfNxtPosition(Coordinate nxtPos)
    {
        int direction = -10;
        Coordinate currentCoord = currentBlockGroup[0];
        if(nxtPos.y<currentCoord.y)
        {
            if(nxtPos.x>currentCoord.x)
            {
                direction = 4;
            }
            else
            {
                if(nxtPos.x<currentCoord.x)
                {
                    direction = 7;
                }
                else
                {
                    direction = 0;
                }
            }
        }else
        {
            if(nxtPos.y>currentCoord.y)
            {
                if (nxtPos.x > currentCoord.x)
                {
                    direction = 5;
                }
                else
                {
                    if (nxtPos.x < currentCoord.x)
                    {
                        direction = 6;
                    }
                    else
                    {
                        direction = 2;
                    }
                }
            }
            else
            {
                if (nxtPos.x > currentCoord.x)
                {
                    direction = 1;
                }
                else
                {
                    if (nxtPos.x < currentCoord.x)
                    {
                        direction = 3;
                    }
                    else
                    {
                        direction = -1;
                    }
                }
            }
        }
        return direction;

    }

    public bool CheckIfCoordinateIsObject(Coordinate coord , out GameObject targetObject)
    {
        if (roomGrid.gridMatrix[coord.y].cells[coord.x].groundObject != null)
        {
            targetObject = roomGrid.gridMatrix[coord.y].cells[coord.x].groundObject;
            return true;
        }
        else
        {
            targetObject = null;
            return false;
        }
    }

    public bool CheckIfCoordinateIsTarget(Coordinate coord ,int targetInstanceID)
    {
        if(roomGrid.gridMatrix[coord.y].cells[coord.x].groundObject != null)
        {
            if (roomGrid.gridMatrix[coord.y].cells[coord.x].groundObject.GetInstanceID() == targetInstanceID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }



}
