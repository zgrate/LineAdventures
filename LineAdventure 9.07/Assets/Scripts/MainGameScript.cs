using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MainGameScript : MonoBehaviour {

    bool block = false;
    public int maxLength = 500;
    // Use this for initialization
    Touch last;
    int width, height;
    public LineRenderer rendered;
    [System.NonSerialized]
    public List<Vector3> list = new List<Vector3>();
    public Vector2[] debugLinesToDraw1 = new Vector2[0], debugLinesToDraw2 = new Vector2[0];
    public LvLStatus status;
    public Enemy[] enemies;
    
	void Start () {
        enemies = status.getEnemies();
    }

    
    public static Vector3 recal(Vector3 v3)
    {
        return Camera.main.ScreenToWorldPoint(v3);
    } 
    int i = 0;
    float distance = 0;
    // i is called once per frame
    int frame = 0;
    void resetAll()
    {
        i = 0;
        list.Clear();
        rendered.SetVertexCount(0);
        rendered.SetPositions(new Vector3[0]);
        distance = 0;
        status.reset();
    }

    public Vector2 calculatePoint(Vector2 start, Vector2 end, float k)
    {
        //(x,y) = (x1 + k(x2 - x1), y1 + k(y2 - y1)
        float x = (start.x + (k * (end.x - start.x)));
        float y = (start.y + (k * (end.y - start.y)));
        return new Vector3(x, y, 0f);

    }



    public Vector2 middle(Vector2 v1, Vector2 v2)
    {
        return new Vector2(((v1.x + v2.x) / 2), ((v1.y+v2.y)/2));
    }

    List<Vector3> getPoints(Vector3 start, Vector3 end, float howprecision)
    {
        List<Vector3> v = new List<Vector3>();
        /*
        float dis = Vector3.Distance(start, end);
        int steps = (int)(dis / precision);
        for(int i = 1; i <= steps; i++ )
        {
            v.Add(new Vector3(start.x + (i * precision), start.y + (i * precision), 0f));
            if (v.Count > 100)
                Application.Quit();
        }

        */
        /*
        float startX = Mathf.Min(start.x, end.x);
        float startY = Mathf.Min(start.y, end.y);

        float endX = Mathf.Max(start.x, end.x);
        float endY = Mathf.Max(start.y, end.y);
        for(float f = precision; ; f+=precision)
        {
            Vector3 v3 = new Vector3(startX, startY, 0f);
            if(v3.x >= endX || v3.y >= endY)
            {
                break;
            }
            v.Add(v3);
        }

        */

        for(int i = 0; i*howprecision < 1; i ++)
        {
            v.Add(calculatePoint(start, end, i * howprecision));
        }

        return v;


    }
	void Update () {
        Touch[] touches = Input.touches;
        
        if (touches.Length != 0)
        {
            
            Touch t = touches[0];
            Vector2 delta = t.deltaPosition;
           // Debug.Log(t.phase);
           
            if (t.phase == TouchPhase.Ended)
            {
                //Debug.Log("END!!");
                resetAll();
                block = false;
                return;
            }
            
            if(block)
            {
                return;
            }
            if (delta.x == 0 && delta.y == 0)
            {
               // Debug.Log("NOPE");
                return;
            }
            enemies = status.getEnemies();
            Vector3 vec = Camera.main.ScreenToWorldPoint(touches[0].position);
            vec.z = 0;
            list.Add(vec);
            addVertex();
            foreach(Enemy obje in enemies)
            {
                Vector2[] v = obje.getVertices  ();
                if(isCollidinWithVertices(v))
                {
                    status.hitted(obje);
                }
            }

            //COLLISIONS
            ;
            if (isLineCollide())
            {}

            //Debug.Log(obj.Length);

            if (distance > maxLength)
            {
                do {
                    removeVertex();
                    list.RemoveAt(0);
                    
                } while (distance > maxLength);
            }
    //        if (true)
     //           drawLine();
     //       else {
                rendered.SetVertexCount(list.Count);
                rendered.SetPositions(list.ToArray());
       //     }
            
        }

    }


    public static bool IsPointInPolygon4(Vector2[] polygon, Vector2 testPoint)
    {
        bool result = false;
        int j = polygon.Length - 1;
        for (int i = 0; i < polygon.Length; i++)
        {
            if (polygon[i].y < testPoint.y && polygon[j].y >= testPoint.y || polygon[j].y < testPoint.y && polygon[i].y >= testPoint.y)
            {
                if (polygon[i].x + (testPoint.y - polygon[i].y) / (polygon[j].y - polygon[i].y) * (polygon[j].x - polygon[i].x) < testPoint.x)
                {
                    result = !result;
                }
            }
            j = i;
        }
        return result;
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < debugLinesToDraw1.Length; i++)
        {
            if (i + 1 == debugLinesToDraw1.Length)
            {
                Gizmos.DrawLine(debugLinesToDraw1[i], debugLinesToDraw1[0]);
            }
            else
            { 
                Gizmos.DrawLine(debugLinesToDraw1[i], debugLinesToDraw1[i + 1]);
            }
        }
        Gizmos.color = Color.yellow;
        for (int i = 0; i < debugLinesToDraw2.Length - 1; i++)
        {
            Gizmos.DrawLine(debugLinesToDraw2[i], debugLinesToDraw2[i + 1]);
        }
    }
    private void isInside(myLine startLine, myLine endLine)
    {
        Vector3 start = startLine.StartPoint;
        Vector3 end = endLine.EndPoint;
        int startI = list.IndexOf(start);
        int endI = list.IndexOf(end);
        Vector3[] points = list.GetRange(startI, endI-startI).ToArray();

        foreach (Enemy obj in enemies)
        {
            Vector2[] ver = obj.getVertices();
            debugLinesToDraw1 = ver;
            debugLinesToDraw2 = points.toVector2Array();
            if (areAllPointsInThePolygon(ver, points.toVector2Array()))
             {
                status.CircledEnemy(obj);
            }
        }
    }
    public bool areAllPointsInThePolygon(Vector2[] points, Vector2[] polygon, float correct = 0.1f)
    {
        /*
        if(points.Length == 2)
        {
            Vector2 start = points[0];
            Vector2 end = points[1];
            for(float f = correct; ; f+=correct)
            {
                bool = isInside
            }
        }
        */
        foreach(Vector2 point in points)
        {
            // bool b = PointInsidePolygon(point, polygon);
            //bool b = IsPointInPolygon4(polygon, point);
            bool b = IsInPolygon(polygon, point);
            if (!b)
                return false;
        }
        return true;
    }

    public bool areAllPointsInThePolygonReversed(Vector2[] points, Vector2[] polygon, float correct = 0.1f)
    {

        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector2 start = points[i];
            Vector2 end = points[i + 1];
            for (float f = correct; ; f += correct)
            {

                bool b = IsInPolygon(polygon, Vector3.MoveTowards(start, end, f));
             //   Debug.Log("Y$S " + b);
                if (b)
                    return true;
                if (start.x + f > end.x || start.y + f > end.y)
                    break;
            }
        }
        
    /*    foreach (Vector2 point in points)
        {

            //bool b = PointInsidePolygon(point, polygon);
            //bool b = IsPointInPolygon4(polygon, point);
            bool b = IsInPolygon(polygon, point);
            if (b)
                return true;
        }
        */
        return false;
    }

    public static bool IsInPolygon(Vector2[] poly, Vector2 p)
    {
        Vector2 p1, p2;


        bool inside = false;


        if (poly.Length < 3)
        {
            return inside;
        }


        var oldPoint = new Vector2(
            poly[poly.Length - 1].x, poly[poly.Length - 1].y);


        for (int i = 0; i < poly.Length; i++)
        {
            var newPoint = new Vector2(poly[i].x, poly[i].y);


            if (newPoint.x > oldPoint.x)
            {
                p1 = oldPoint;

                p2 = newPoint;
            }

            else
            {
                p1 = newPoint;

                p2 = oldPoint;
            }


            if ((newPoint.x < p.x) == (p.x <= oldPoint.x)
                && (p.y - (long)p1.y) * (p2.x - p1.x)
                < (p2.y - (long)p1.y) * (p.x - p1.x))
            {
                inside = !inside;
            }


            oldPoint = newPoint;
        }


        return inside;
    }

    public static bool PointInsidePolygon(Vector2 point, Vector2[] polygonVertices)
    {
        if (polygonVertices.Length < 3) //not a valid polygon
            return false;
        int nCounter = 0;
        int nPoints = polygonVertices.Length;
        Vector2 p1, p2;
        p1 = polygonVertices[0];
        for (int i = 1; i < nPoints; i++)
        {
            p2 = polygonVertices[i % nPoints];
            if (point.y > Mathf.Min(p1.y, p2.y))
            {
                if (point.y <= Mathf.Max(p1.y, p2.y))
                {
                    if (point.x <= Mathf.Max(p1.x, p2.x))
                    {
                        if (p1.y != p2.y)
                        {
                            double xInters = (point.y - p1.y) * (p2.x - p1.x) /
                                (p2.y - p1.y) + p1.x;
                            if ((p1.x == p2.x) || (point.x <= xInters))
                            {
                                nCounter++;
                            }
                        }
                    }
                }
            }
            p1 = p2;
        }
        if ((nCounter % 2) == 0)
            return false;
        else
            return true;
    }

    private bool isLineCollide()
    {
        if (list.Count < 2)
            return false;
        int TotalLines = list.Count - 1;
        myLine[] lines = new myLine[TotalLines];
        if (TotalLines > 1)
        {
            for (int i = 0; i < TotalLines; i++)
            {
                lines[i].StartPoint = (Vector3)list[i];
                lines[i].EndPoint = (Vector3)list[i + 1];
            }
        }
        for (int i = 0; i < TotalLines - 1; i++)
        {
            myLine currentLine;
            currentLine.StartPoint = (Vector3)list[list.Count - 2];
            currentLine.EndPoint = (Vector3)list[list.Count - 1];
            if (isLinesIntersect(lines[i], currentLine))
            {
                isInside(lines[i], currentLine);
                Vector3 start = lines[i].StartPoint;
                Vector3 end = currentLine.EndPoint;
                int startI = list.IndexOf(start);
                int endI = list.IndexOf(end);
                Debug.Log("removing " + startI + " " + endI + " " + list.Count);
                for(int o = startI; o < endI; o++)
                {
                    list.RemoveAt(startI);
                }
                recalculateDistance();
                return false;
            }
                
        }
        return false;
    }

    
    private bool isCollidinWithVertices(Vector2[] vertices)
    {
        if (list.Count < 2)
            return false;
       // Debug.Log("Sprawdzam");
        int TotalLines2 = vertices.Length;
        myLine[] lines2 = new myLine[TotalLi    nes2];
        if (TotalLines2 > 1)
        {
            for (int i = 0; i < TotalLines2; i++)
            {
                if (i+1 >= TotalLines2)
                {
                    lines2[i].StartPoint = (Vector3)vertices[i];
                    lines2[i].EndPoint = (Vector3)vertices[0];
                }
                else {
                    lines2[i].StartPoint = (Vector3)vertices[i];
                    lines2[i].EndPoint = (Vector3)vertices[i + 1];
                }
            }
        }
        myLine lastline;
        lastline.StartPoint = list[list.Count - 2];
        lastline.EndPoint = list[list.Count - 1];
        List<Vector2> pointsTocheck = new List<Vector2>();
        pointsTocheck.Add(list[list.Count - 2]);
        pointsTocheck.Add(list[list.Count - 1]);
        if(areAllPointsInThePolygonReversed(list.ToArray().toVector2Array(), vertices))
        {
            debugLinesToDraw1 = vertices;
         //   Debug.Log("UPS");
            resetAll();
            block = true;
            return true;
        }


        /*
        for (int j = 0; j < TotalLines2; j++)
        {
            myLine currentLine = lines2[j];

            foreach(Vector3 point in pointsTocheck)
            
            if (PointOnLineSegment(currentLine.EndPoint, currentLine.StartPoint, point, 2f) )
            {
                Debug.Log("Styknelo sie to " + lines2[i].StartPoint + " " + lines2[i].EndPoint + "   " + lastline.StartPoint + " " + lastline.EndPoint);
                resetAll();
                block = true;
                    return false;
             }

            
        }
        */
        return false;
    }
    void recalculateDistance()
    {
        distance = 0;
        if (list.Count < 2)
            return;
        Vector2 last = list[0];
        for(int i = 1; i < list.Count; i++)
        {
            distance += Vector3.Distance(last, list[i]);
            last = list[i];
        }
    }


    private void addVertex()
    {
        if (list.Count > 1)
        {
            distance += dis(list[list.Count - 1], list[list.Count - 2]);
        }

    }
    private void removeVertex()
    {
        if (list.Count > 1)
        {
            distance -= dis(list[0], list[1]);

        }
    }

    float dis(Vector3 p1, Vector3 p2)
    {
        return Vector3.Distance(p1, p2);
    }


    public struct myLine
    {
        public Vector3 StartPoint;
        public Vector3 EndPoint;
        
        public myLine(Vector3 start, Vector3 end)
        {
            this.StartPoint = start;
            this.EndPoint = end;
        }
    };

    //    -----------------------------------    
    //    Following method checks whether given two points are same or not
    //    -----------------------------------    
    private static bool checkPoints(Vector3 pointA, Vector3 pointB)
    {
        return (pointA.x == pointB.x && pointA.y == pointB.y);
    }
    //    -----------------------------------    
    //    Following method checks whether given two line intersect or not
    //    -----------------------------------    
    public static bool isLinesIntersect(myLine L1, myLine L2)
    {
        
        if (checkPoints(L1.StartPoint, L2.StartPoint) ||
            checkPoints(L1.StartPoint, L2.EndPoint) ||
            checkPoints(L1.EndPoint, L2.StartPoint) ||
            checkPoints(L1.EndPoint, L2.EndPoint))
            return false;
            

        return ((Mathf.Max(L1.StartPoint.x, L1.EndPoint.x) >= Mathf.Min(L2.StartPoint.x, L2.EndPoint.x)) &&
            (Mathf.Max(L2.StartPoint.x, L2.EndPoint.x) >= Mathf.Min(L1.StartPoint.x, L1.EndPoint.x)) &&
            (Mathf.Max(L1.StartPoint.y, L1.EndPoint.y) >= Mathf.Min(L2.StartPoint.y, L2.EndPoint.y)) &&
            (Mathf.Max(L2.StartPoint.y, L2.EndPoint.y) >= Mathf.Min(L1.StartPoint.y, L1.EndPoint.y))
         );
    }
    public bool PointOnLineSegment(Vector2 pt1, Vector2 pt2, Vector2 pt, float epsilon = 0.001f)
    {
        if (pt.x - Mathf.Max(pt1.x, pt2.x) > epsilon ||
            Mathf.Min(pt1.x, pt2.x) - pt.x > epsilon ||
            pt.y - Mathf.Max(pt1.y, pt2.y) > epsilon ||
            Mathf.Min(pt1.y, pt2.y) - pt.y > epsilon)
            return false;

        if (Mathf.Abs(pt2.x - pt1.x) < epsilon)
            return Mathf.Abs(pt1.x - pt.x) < epsilon || Mathf.Abs(pt2.x - pt.x) < epsilon;
        if (Mathf.Abs(pt2.y - pt1.y) < epsilon)
            return Mathf.Abs(pt1.y - pt.y) < epsilon || Mathf.Abs(pt2.y - pt.y) < epsilon;

        float x = pt1.x + (pt.y - pt1.y) * (pt2.x - pt1.x) / (pt2.y - pt1.y);
        float y = pt1.y + (pt.x - pt1.x) * (pt2.y - pt1.y) / (pt2.x - pt1.x);

        return Mathf.Abs(pt.x - x) < epsilon || Mathf.Abs(pt.y - y) < epsilon;
    }


}

public static class MyVector3Extension
{
    public static Vector2[] toVector2Array(this Vector3[] v3)
    {
        return System.Array.ConvertAll<Vector3, Vector2>(v3, getV3fromV2);
    }

    public static Vector3[] toVector3Array(this Vector2[] v2)
    {
        return System.Array.ConvertAll<Vector2, Vector3>(v2, getV2fromV3);
    }
    public static Vector2 getV3fromV2(Vector3 v3)
    {
        return new Vector2(v3.x, v3.y);
    }
    public static Vector3 getV2fromV3(Vector2 v3)
    {
        return new Vector3(v3.x, v3.y, 0f);
    }

    
}