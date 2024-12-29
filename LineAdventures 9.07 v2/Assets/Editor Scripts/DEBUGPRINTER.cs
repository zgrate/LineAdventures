using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class DEBUGPRINTER : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
    public Vector2 VTODRAW;
	// Update is called once per frame
	void Update () {

    }
    void OnDrawGizmos()
    {
        Gizmos.DrawCube(VTODRAW, new Vector2(100, 100));
    }
}
