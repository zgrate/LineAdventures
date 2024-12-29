using UnityEngine;
using System.Collections;


public class TraingleVertexGetter : PhysicsGenerator 
{
    Vector2[] actualVertices;
    // Use this for initialization
    void Start () {
        //Mesh m = PhysicsGenerator.getMesh(new Vector2[] { new Vector2(10, 10), new Vector2(0, 0), new Vector2(20, 20), new Vector2(20, 20) });
        Vector2[] v2 = new Vector2[] {
            new Vector2(1,2),
            new Vector2(3,4),
            new Vector2(5,7),
        };
        actualVertices = v2;
        Mesh m = PhysicsGenerator.getMesh(v2);
        gameObject.GetComponent<MeshFilter>().mesh = m;
        Debug.Log("Done!");
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(Input.touches.Length);
        if(Input.touches.Length > 2)
        {
            Vector2[] v2 = new Vector2[Input.touches.Length];
            int i = 0;
            foreach(Touch t in Input.touches)
            {
               // Debug.Log(Camera.main.ScreenToWorldPoint(t.position));
                v2[i++] = Camera.main.ScreenToWorldPoint(t.position);
            }
            actualVertices = v2;
           // Debug.Log("I Have " + actualVertices.Length);
            Mesh m = PhysicsGenerator.getMesh(v2);
            gameObject.GetComponent<MeshFilter>().mesh = m;
        }
	}


    public Vector2[] getVertices()
    {
        return actualVertices;
        
        
    }

}
