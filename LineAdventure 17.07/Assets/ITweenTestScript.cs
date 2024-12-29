using UnityEngine;
using System.Collections;

public class ITweenTestScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Rotate();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void Move()
    {
        iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath("p2"), "islocal", true, "time", 5, "easetype", iTween.EaseType.spring, "oncomplete", "completed", "oncompletetarget", gameObject));
    }
    void Rotate()
    {
        Vector2 v2 = gameObject.GetComponent<MeshFilter>().mesh.bounds.center;
        //iTween.RotateAdd(gameObject, iTween.Hash("amount", new Vector3(360f, 0f, 0), "time", 5f, "x", v2.x, "y", v2.y));
        iTween.RotateTo(this.gameObject, iTween.Hash("z", 180.0f, "time", 1.0f, "easeType", iTween.EaseType.easeInOutSine));
    }
    void completed()
    {
        Debug.Log("END!");
        gameObject.transform.RotateAround(gameObject.GetComponent<MeshRenderer>().bounds.center, new Vector3(0, 0, 1), 100);

        Move();
    }
    void comp()
    {

    }

}
