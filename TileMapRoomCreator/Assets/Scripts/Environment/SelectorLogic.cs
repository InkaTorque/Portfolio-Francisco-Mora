using UnityEngine;
using System.Collections;

public class SelectorLogic : MonoBehaviour {

    public GameObject getParent()
    {
        return transform.parent.gameObject;
    }


}
