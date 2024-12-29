using UnityEngine;
using System.Collections.Generic;

public class MainGameSystem : MonoBehaviour {

    public List<Vector2> points = new List<Vector2>();
    public List<Line> lines = new List<Line>();
    public LineRenderer renderer;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 v;
        
        
        if (Input.touchCount < 1 && Input.anyKey)
        {
            v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.touchCount > 0)
        {
            Touch t = Input.touches[0];
            v = Camera.main.ScreenToWorldPoint(t.position);
        }
        else return;
        if(points.Count > 0)
        {
            Line l;
            lines.Add(l = Line.getLine(points[0], v));
            points.Add(v);
        }
        else
        {
            points.Add(v);
        }
        renderer.SetVertexCount(points.Count);
        renderer.SetPositions(points.ToArray().toVector3Array());
	}

    public bool isInterSecting(Line line, List<Line> lines)
    {
        foreach(Line l in lines)
        {
            if (!isAboveScale(calculateIntersectPoint(line, l), line))
                return true;
        }
        return false;
    }

    public Vector2 calculateIntersectPoint(Line l1, Line l2)
    {
        float y = ((l1.b * l2.a) - (l1.a * l2.b)) / (l2.a - l1.a);
        float x = (y / l2.a) - (l2.b) / (l2.a);
        return new Vector2(x, y);
    }
    public bool isIn(Line line, Vector2 p, Vector2 s1, Vector2 s2)
    {
        if (line.getY(p.x) != p.y)
            return false;
        return p.x > Mathf.Min(s1.x, s2.x) && p.x < Mathf.Max(s1.x, s2.x) &&
                p.y > Mathf.Min(s1.y, s2.y) && p.y < Mathf.Max(s1.y, s2.y);
    }
    public bool isAboveScale(Vector2 point, Line[] lines)
    {
        foreach (Line line in lines)
        {
            if (!isIn(line, point, line.start, line.end))
                return true;
        }
        return false;
    }
    public bool isAboveScale(Vector2 point, Line line)
    {
        return !isIn(line, point, line.start, line.end);
    }
    public class Line
    {
        public float a, b;
        public Vector2 start, end;

        public Line(float a, float b, Vector2 start, Vector2 end)
        {
            this.a = a;
            this.b = b;
            this.start = start;
            this.end = end;
        }

        public float getY(float x)
        {
            return (a * x) + b;
        }
        public static Line getLine(Vector2 start, Vector2 end)
        {
            //a1 = (y1 – y2) / (x1 – x2)
            float a = (start.y - end.y) / (start.x - end.x);
            float b = (-1 * (a * end.x)) + end.y;
            return new Line(a, b, start, end);

        }

        public float getMinX()
        {
            return Mathf.Min(start.x, end.x);
        }
        public float getMinY()
        {
            return Mathf.Min(start.y, end.y);
        }
        public float getMaxX()
        {
            return Mathf.Max(start.x, end.x);
        }
        public float getMaxY()
        {
            return Mathf.Max(start.y, end.y);
        }
    }
}
