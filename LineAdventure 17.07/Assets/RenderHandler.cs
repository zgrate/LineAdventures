using UnityEngine;
using System.Collections;

public class RenderHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ExecuteMovement(Hashtable hash)
    {
        iTween.Init(gameObject);
        iTween.MoveTo(gameObject, hash);
    }
}
