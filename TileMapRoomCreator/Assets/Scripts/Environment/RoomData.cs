using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelBounds
{
	public float maxH;
	public float minH;
	public float maxV;
	public float minV;
}

public enum PrefabToInstantiate 
{ 
	None	            = 0,
	DialogueTrigger		= 1,
    ActionTrigger       = 2,
    DriverPuzzle        = 3,
    VinePuzzle          = 4
}

public class RoomData : MonoBehaviour 
{
    [HideInInspector]
	public string RoomId;
    public Sprite selectorSprite;

    [SerializeField][HideInInspector]
    private GameObject checkpoint;

//	public Transform LeftWall;
//	public Transform RightWall;
//	public Transform UpperWall;
//	public Transform LowerWall;

	private Transform levelBoundary;
	private LevelBounds levelBounds;
    private Transform enemies;
    private Transform environment;

	void Awake()
	{
		levelBounds = new LevelBounds();
		levelBoundary=transform.Find("Playable Boundary");
        enemies = transform.Find("Enemies");
        environment = transform.Find("Environment");
		AccquireLevelBounds();
	}

	private void AccquireLevelBounds()
	{
		levelBounds.maxH=levelBoundary.Find("Right Wall").transform.position.x;
		levelBounds.minH=levelBoundary.Find("Left Wall").transform.position.x;
		levelBounds.maxV=levelBoundary.Find("Top Wall").transform.position.y;
		levelBounds.minV=levelBoundary.Find("Bot Wall").transform.position.y;
	}

    public void RearangeHierarchy()
    {
        if(transform.Find("Environment")==null)
        {
            GameObject Environment = new GameObject();
            Environment.transform.parent = gameObject.transform;
            Environment.name = "Environment";
            Environment.transform.localPosition = Vector3.zero;
        }
        if (transform.Find("Selector") == null)
        {
            GameObject Selector = new GameObject();
            Selector.transform.parent = gameObject.transform;
            Selector.name = "Selector";
            Selector.transform.localPosition = Vector3.zero;
            Selector.AddComponent<SpriteRenderer>();
            Selector.GetComponent<SpriteRenderer>().sprite = selectorSprite;
            Selector.transform.localScale = new Vector3(0.2f,0.2f,0f);
            #if UNITY_EDITOR
             Selection.activeGameObject = Selector;
            #endif
        }
        foreach (Transform child in transform)
        {
            if(!child.gameObject.tag.Equals("Untagged") && !child.gameObject.tag.Equals("Connection"))
            {
                child.parent = transform.Find("Environment");
            }
        }
        
    }

	public LevelBounds LevelBounds
	{
		set{
			levelBounds=value;
		}
		get
		{
			return levelBounds;
		}
	}
}
