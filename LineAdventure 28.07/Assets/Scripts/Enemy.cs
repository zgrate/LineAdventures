using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Enemy : MonoBehaviour {

    public Vector2[] startVertexes = new Vector2[3];
    public int timesToCircle;
    public bool enableDebugEditor = false;
    public bool calLengh = false;
    
    public float Lenght
    {
        get
        {
            float distance = 0f;
            Vector2 last = Vector2.zero;
            bool assigned = false;
            foreach (Vector2 v in startVertexes)
            {
                if (!assigned)
                {
                    last = v;
                    assigned = true;
                    continue;
                }
                distance += Vector2.Distance(v, last);
                last = v;
            }
            distance += Vector2.Distance(last, startVertexes[0]);
            return distance;
        }
        set
        {
            return;
        }
    }
    
    [System.NonSerialized]
    public  GameObject _text;


    void Start()
    {
    }


    public void finished()
    {
        _text.SetActive(false);
        Destroy(_text);
    }

    void OnDisable()
    {
        Destroy(_text);
    }


    void Update()
    {

#if UNITY_EDITOR
        if (enableDebugEditor)
        {
            Mesh m = (startVertexes).GenerateMesh();
            gameObject.GetComponent<MeshFilter>().mesh = m;
        }
        if(calLengh)
        {
            float distance = 0f;
            Vector2 last = Vector2.zero;
            bool assigned = false;
            foreach (Vector2 v in startVertexes)
            {
                if (!assigned)
                {
                    last = v;
                    assigned = true;
                    continue;
                }
                distance += Vector2.Distance(v, last);
                last = v;
            }
            distance += Vector2.Distance(last, startVertexes[0]);
            Debug.Log("Length of Line around: " + distance);
        }
#endif
    }


    public void Circeled(int times) 
    {
        _text.SetActive(true);
        _text.GetComponent<TextMesh>().text = "" + (this.timesToCircle-times);
    }
    public void cancel()
    {
        _text.SetActive(false);
    }

    public Vector2[] getVertices()
    {
        Vector2[] transformed = new Vector2[startVertexes.Length];
        for(int i = 0; i < startVertexes.Length; i++)
        {
            transformed[i] = gameObject.transform.TransformPoint(startVertexes[i]);
        }
        return transformed;
    }

    public bool IsNearer(Vector2 position, float distance)
    {
        foreach(Vector2 v2 in getVertices())
        {
            if (Vector2.Distance(position, v2) <= distance)
                return true;
        }
        return false;
    }

    public Vector2 ArePointsNearer(List<Vector2> positions, float distance)
    {
        foreach(Vector2 v in positions)
        {
            if (IsNearer(v, distance))
                return v;
        }
        return Vector2.zero;
    }

}
