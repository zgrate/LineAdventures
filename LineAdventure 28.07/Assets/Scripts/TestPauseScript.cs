using UnityEngine;
using System.Collections;

public class TestPauseScript : MonoBehaviour {

    int lastframe;
    // Use this for initializationp
    public RandomPreset preset;
	void Start () {
        //lastframe = Time.frameCount;
        GameObject obj = new GameObject();
        obj.AddComponent<MeshFilter>();
        obj.AddComponent<MeshRenderer>();
        obj.GetComponent<MeshFilter>().mesh = preset.Enemy.Vertices.GenerateMesh();
        obj.GetComponent<MeshRenderer>().material = preset.Enemy.Material;
	}



    long TimeUpdate, FIXEDUPDATE;
	// Update is called once per frame
	void Update () {
//        Debug.Log("UPDATE " + (iTweenImplementation.CurrentTimeMillis()-TimeUpdate));
  //      TimeUpdate = iTweenImplementation.CurrentTimeMillis();
	    
	}

    void FixedUpdate()
    {
        //Debug.Log("FIXED UPDATE " + (iTweenImplementation.CurrentTimeMillis() - FIXEDUPDATE));
       // FIXEDUPDATE = iTweenImplementation.CurrentTimeMillis();
    }
}
