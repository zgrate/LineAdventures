using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class ScrollBarScript : MonoBehaviour {


    Scrollbar scroll;
	// Use this for initialization
	void Start () {
        scroll = gameObject.GetComponent<Scrollbar>();
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    float last = 0;
    public void change()
    {
    }
}
