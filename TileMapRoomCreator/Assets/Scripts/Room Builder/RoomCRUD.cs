#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;

public static class RoomCRUD
{
	public static void CreateRoom (int roomWidth , int roomHeight) 
	{
		GameObject roomGO = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/RoomParts/Room"));
		roomGO.name = "New Room";

        RoomData rd = roomGO.GetComponent<RoomData>();
        RoomGrid rg = roomGO.GetComponent<RoomGrid>();
        BoxCollider2D c2d = roomGO.GetComponent<BoxCollider2D>();

        rd.RoomId = "R" + System.DateTime.Now.Year + System.DateTime.Now.Month + System.DateTime.Now.Day + System.DateTime.Now.Hour + System.DateTime.Now.Minute + System.DateTime.Now.Second + System.DateTime.Now.Millisecond;

		GameObject roomParentGO = GameObject.FindGameObjectWithTag("RoomGroup");
		
		if(roomParentGO != null)
		{
			roomGO.transform.parent = roomParentGO.transform;
		}
		else
		{
			roomParentGO = new GameObject("Rooms");
			roomParentGO.transform.position = Vector3.zero;
			roomParentGO.tag = "RoomGroup";

			roomGO.transform.parent = roomParentGO.transform;
		}
        rg.CreateGrid(roomWidth,roomHeight);

        Vector2 size = c2d.size;

        size.x = GameConstants.gridBlockWidth * roomWidth;
        size.y = GameConstants.gridBlockHeight * roomHeight;

        c2d.size = size;

        Vector2 offset = c2d.offset;

        offset.x = (0.5f * (roomWidth-GameConstants.minimalRoomWidth)*GameConstants.gridBlockWidth);
        offset.y = (-0.5f * (roomHeight-GameConstants.minimalRoomHeight)*GameConstants.gridBlockHeight);

        c2d.offset = offset;

        roomGO.transform.position = Vector2.zero;
		
		Selection.activeGameObject = roomGO;
	}

	[MenuItem ("Room Manager/Clear Scene")]
	private static void ClearScene()
	{
		GameObject[] rooms = GameObject.FindGameObjectsWithTag("Room");

		if(rooms != null)
		{
			for(int i = rooms.Length - 1; i >= 0; i--)
			{
				MonoBehaviour.DestroyImmediate(rooms[i]);
			}
		}
	}
}
#endif