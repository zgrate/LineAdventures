using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewDetecionScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    List<myLine> linesToDraw = new List<myLine>();
    Vector2? last = null;

	// Update is called once per frame
	void Update () {
        if (Input.touchCount == 0)
            return;
        Touch t = Input.GetTouch(0);
        Vector2 pos = Camera.main.ScreenToWorldPoint(t.position);
        if(last == null)
        {
            last = pos;
            return;
        }
        
        myLine line = new myLine(last.Value, pos);
        last = pos;
        linesToDraw.Add(line);
    }
    public void OnDrawGizmos()
    {
        foreach(myLine line in linesToDraw)
        {
            Gizmos.DrawLine(line.StartPoint, line.EndPoint);
        }
    }
    public float precision = 0;
    public class myLine
    {
        public Vector3 StartPoint;
        public Vector3 EndPoint;
        public List<myLine> fragmentation = null;

        public myLine(Vector3 start, Vector3 end)
        {
            this.StartPoint = start;
            this.EndPoint = end;
        }
        public List<myLine> GetFragmentation()
        {
            return fragmentation;
        }

    };

    public List<myLine> GetFragmentation(myLine line)
    {
        if(line.fragmentation ==null)
        {
            line.fragmentation = getPoints(line, precision);
        }
        return line.fragmentation;
    }

    public void OnRenderObject()
    {
        drawLine();
    }


    public void drawLine()
    {
        if (linesToDraw.Count < 1)
            return;
        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);

        // Draw lines
        GL.Begin(GL.LINES);
        // Debug.Log(list.Count);
        for (int i = 0; i < linesToDraw.Count; ++i)
        {
            foreach (myLine line in GetFragmentation(linesToDraw[i]))
            { 
                GL.Vertex3(line.StartPoint.x, line.StartPoint.y, 0f);
                // GL.Vertex3(0, 0, 0);
                // Another vertex at edge of circle
                GL.Vertex3(line.EndPoint.x, line.EndPoint.y, 0f);
            }
        }
        GL.End();
        GL.PopMatrix();
    }

    public Material lineMaterial;
    void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            var shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    public Vector2 calculatePoint(Vector2 start, Vector2 end, float k)
    {
        //(x,y) = (x1 + k(x2 - x1), y1 + k(y2 - y1)
        float x = (start.x + (k * (end.x - start.x)));
        float y = (start.y + (k * (end.y - start.y)));
        return new Vector3(x, y, 0f);
    }

    public List<Vector3> getPoints(Vector3 start, Vector3 end, float howprecision)
    {
        List<Vector3> v = new List<Vector3>();

        for (int i = 0; i * howprecision < 1; i++)
        {
            v.Add(calculatePoint(start, end, i * howprecision));
        }

        return v;
    }

    public List<myLine> getPoints(myLine line, float howprecision)
    {
        List<myLine> v = new List<myLine>();
        Vector3 lastPoint = line.StartPoint;
        for (float i = 0; i * howprecision <= 1; i++)
        {
            Vector3 next = calculatePoint(line.StartPoint, line.EndPoint, i* howprecision);
            v.Add(new myLine(lastPoint, next));
            lastPoint = next;
        }

        return v;
    }
}
