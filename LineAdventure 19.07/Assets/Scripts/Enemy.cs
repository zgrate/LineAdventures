using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {

    public Vector2[] startVertexes = new Vector2[3];
    public float rotationspeed = 0;
    public float VelocityX = 0, VelocityY = 0;
    public Vector3 RotationAxis;
    public bool rotateAroundMiddle = true;
    public Vector3 RotationMiddle;
    public int timesToCircle;
    public bool enableDebugEditor = false;
    public bool calLengh = false;
    private GameObject text;
    void Start()
    {
        if (startVertexes.Length < 3)
            return;
        Mesh m = PhysicsGenerator.getMesh(startVertexes);
        gameObject.GetComponent<MeshFilter>().mesh = m;
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(VelocityX, VelocityY);
        gameObject.GetComponent<PolygonCollider2D>().points = startVertexes;
        text = Instantiate(GameObject.Find("TextNumber"));
        Vector2 vector = gameObject.transform.TransformPoint(getVectorOnDirection(Vector2.up));
        text.transform.position = new Vector3(vector.x, vector.y + 100, 0);
        if(calLengh)
        {
            float distance = 0f;
            Vector2 last = Vector2.zero;
            bool assigned = false;
            Debug.Log("TESt1");
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
            Debug.Log("Distance: " + distance);
        }
        
    }

    public GameObject getTextObject()
    {
        return text;
    }

    public void finished()
    {
        text.SetActive(false);
    }

    void Update()
    {
        if (enableDebugEditor)
        {
            Mesh m = PhysicsGenerator.getMesh(startVertexes);
            gameObject.GetComponent<MeshFilter>().mesh = m;
        }
        if (startVertexes.Length < 3)
            return;
        if (rotateAroundMiddle)
            gameObject.transform.RotateAround(gameObject.GetComponent<MeshRenderer>().bounds.center, RotationAxis, rotationspeed);
        else
            gameObject.transform.RotateAround(RotationMiddle, RotationAxis, rotationspeed);
        if(hasVelocity())
        {
            Vector2 direction = gameObject.transform.TransformPoint(getVectorOnDirection(Vector2.up));
            text.transform.position = direction;
        }
    }

    public bool hasVelocity()
    {
        return gameObject.GetComponent<Rigidbody2D>().velocity != Vector2.zero;
    }

    public Vector2 getVectorOnDirection(Vector2 direction)
    {
        if(direction.Equals(Vector3.up))
        {
            Vector2 v = Vector2.zero;
            float maxY = 0;
            foreach(Vector2 v2 in startVertexes)
            {
                if(v == Vector2.zero)
                {
                    maxY = v2.y;
                    v = v2;
                    continue;
                }
                if(maxY < v.y)
                {
                    maxY = v.y;
                    v = v2;
                    continue;
                }
            }
            return v;
        }
        return Vector2.zero;
    }

    public void Circeled(int times) 
    {
        text.SetActive(true);
        text.GetComponent<TextMesh>().text = "" + (this.timesToCircle-times);
    }
    public void cancel()
    {
        text.SetActive(false);
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
