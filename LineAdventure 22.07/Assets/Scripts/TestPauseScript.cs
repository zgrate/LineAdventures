using UnityEngine;
using System.Collections;

public class TestPauseScript : MonoBehaviour {

    int lastframe;
	// Use this for initialization
	void Start () {
        lastframe = Time.frameCount;
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
        FIXEDUPDATE = iTweenImplementation.CurrentTimeMillis();
    }
}
