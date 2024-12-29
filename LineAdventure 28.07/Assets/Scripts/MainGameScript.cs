using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class MainGameScript : MonoBehaviour {

    bool block = false;
    public int maxLength = 500;
    // Use this for initialization
    public LineRenderer rendered;

    public const short START_VER = 0, END_VER = 1, START_INDEX = 2, END_INDEX = 3, START_LINE = 4, END_LINE = 5;

    [System.NonSerialized]
    private List<Vector2> vertices = new List<Vector2>();

    public float distance = 0;
    public LvLStatus status;
    public Enemy[] enemies;
    public Hashtable hash = new Hashtable();
    public HealthScript HealthScript;

    public void StartListening()
    {
        Stopped = false;    
    }

    private void resetDatas()
    {
        vertices.Clear();
        rendered.SetVertexCount(0);
        rendered.SetPositions(new Vector3[0]);
        distance = 0;
        status.reset();
        block = false;
    }

    public void Update()
    {
        Tick();
    }

    public bool Stopped;

    IEnumerator CheckColision()
    {
        for (;;)
        {
            if (Stopped)
            {
                yield return new WaitForSeconds(0.1f);
                continue;
            }
            // Debug.Log("CHECK");
            if (vertices.Count > 0)
            {
                foreach (Enemy obje in enemies)
                {
                    Vector2[] v = obje.getVertices();
                    if (isCollidinWithVertices(v))
                    {
                        status.hitted(obje);
                        resetDatas();
                        block = true;
                        break;
                    }
                }
            }

            //PROXYMETRY DETECTION
            foreach (Enemy obje in enemies)
            {
                if (obje.gameObject.GetComponent<AttackManager>() != null && obje.gameObject.GetComponent<iTweenImplementation>() != null)
                {
                    AttackManager manager = obje.gameObject.GetComponent<AttackManager>();
                    iTweenImplementation impl = obje.gameObject.GetComponent<iTweenImplementation>();
                    if (manager.ProximetryAttack.TypeAttack != AttackManager.AttackType.NULL_ATTACK)
                    {
                        float dis = manager.Range;
                        Vector2 v;
                        if ((v = obje.ArePointsNearer(vertices, dis)) != Vector2.zero)
                            manager.ExecuteProximetryAttack(v, impl.StopMovingAndBlock, impl.StartMovingAndUnblock);
                    }

                }
            }
            yield return new WaitForSeconds(0.1f);
        }
        
    }

    void Tick()
    {
        if (Stopped)
        {
            return;
        }

        Vector2 position;
        Touch[] touches = Input.touches;
        if (touches.Length == 0)
            return;
        Touch t = touches[0];
        if(t.phase == TouchPhase.Ended) //END OF TOUCHING
        {
            resetDatas();
            return;
        }
        if (block) return;
        //DeltaChecking - if touch didn't move
        if (t.deltaPosition.x != 0 && t.deltaPosition.y != 0)
        {
            position = Camera.main.ScreenToWorldPoint(t.position);
            if (vertices.Count > 0)
            {
                vertices.Add(calculatePoint(vertices[vertices.Count - 1], position, 0.5f));
                addVertex();
            }
            vertices.Add(position);
            addVertex();
        }



        //CONNECTION WITH LINE
        if (isLineCollideHashtable())
        {
            myLine startLine = (myLine)hash[START_LINE];
            myLine endLine = (myLine)hash[END_LINE];

            //TODO: ENEMY CIRCELED CHECK - TODO (isInside(start, end))
            CheckEnemyCallable(startLine, endLine);

            int startI = vertices.IndexOf(startLine.StartPoint);
            int endI = vertices.IndexOf(endLine.EndPoint);
            for (int o = startI; o < endI; o++)
            {
                vertices.RemoveAt(startI);
            }
            recalculateDistance();
        }

        if (distance > maxLength) 
        {
            do
            {
                removeVertex();
                vertices.RemoveAt(0);

            } while (distance > maxLength);
        }

        //DRAWING
        rendered.SetVertexCount(vertices.Count);
        rendered.SetPositions(vertices.ToArray().toVector3Array());
    }

    public Vector2 calculatePoint(Vector2 start, Vector2 end, float k)
    {
        //(x,y) = (x1 + k(x2 - x1), y1 + k(y2 - y1)
        float x = (start.x + (k * (end.x - start.x)));
        float y = (start.y + (k * (end.y - start.y)));
        return new Vector3(x, y, 0f);
    }


    void Start () {
        enemies = status.getEnemies();
        StartCoroutine(CheckColision());
    }

    //Checking if line is colliding with sides of enemy
    private bool isCollidinWithVertices(Vector2[] enemyVertices)
    {
        if (vertices.Count < 2)
            return false;
        // Debug.Log("Sprawdzam");
        int TotalLines2 = enemyVertices.Length;
        myLine[] lines2 = new myLine[TotalLines2];
        if (TotalLines2 > 1)
        {
            for (int i = 0; i < TotalLines2; i++)
            {
                if (i + 1 >= TotalLines2)
                {
                    myLine line = (lines2[i] = new myLine());
                    line.StartPoint = (Vector3)enemyVertices[i];
                    line.EndPoint = (Vector3)enemyVertices[0];
                }
                else {
                    myLine line = (lines2[i] = new myLine());
                    line.StartPoint = (Vector3)enemyVertices[i];
                    line.EndPoint = (Vector3)enemyVertices[i + 1];
                }
            }
        }

        if (areAllPointsInThePolygonReversed(vertices.ToArray(), enemyVertices))
        {
            return true;
        }

        return false;
    }
    void recalculateDistance()
    {
        distance = 0;
        if (vertices.Count < 2)
            return;
        Vector2 last = vertices[0];
        for (int i = 1; i < vertices.Count; i++)
        {
            distance += Vector3.Distance(last, vertices[i]);
            last = vertices[i];
        }
    }
    
    public myLine isLineColliding(myLine line)
    {
        if (vertices.Count < 2)
            return myLine.zero_line();
        int TotalLines = vertices.Count - 1;
        myLine[] lines = new myLine[TotalLines];
        if (TotalLines > 1)
        {
            for (int i = 0; i < TotalLines; i++)
            {
                lines[i] = new myLine();
                lines[i].zero = false;
                lines[i].StartPoint = (Vector3)vertices[i];
                lines[i].EndPoint = (Vector3)vertices[i + 1];
            }
        }
        for (int i = 0; i < TotalLines - 1; i++)
        {
            if (isLinesIntersect(lines[i], line)) //CLOSED
            {
                return lines[i];
            }
            
        }
        return myLine.zero_line();
    }

    private IEnumerable<myLine> AllLinesYield()
    {
        int TotalLines = vertices.Count - 1;
        if (TotalLines > 1)
        {
            for (int i = 0; i < TotalLines; i++)
            {
                myLine line = (new myLine());
                line.StartPoint = (Vector3)vertices[i];
                line.EndPoint = (Vector3)vertices[i + 1];
                //Debug.Log("yield " + line);
                yield return line;
            }
        }

    }

    private bool isLineCollideHashtable()
    {
        if (vertices.Count < 3)
            return false;

        List<myLine> listLines = new List<myLine>();
        myLine line1 = new myLine(vertices[vertices.Count - 2], vertices[vertices.Count - 1]);
        listLines.Add(line1);
        if(vertices.Count > 2)
        {
            listLines.Add(new myLine(vertices[vertices.Count-3], vertices[vertices.Count-2]));
        }
        if(vertices.Count > 3)
        {
            listLines.Add(new myLine(vertices[vertices.Count - 4], vertices[vertices.Count - 3]));
        }

        
        foreach(myLine line in AllLinesYield())
        {
            //myLine line = AllLinesYield().Current;
            foreach (myLine l in listLines)
            {
                //Debug.Log(line + " + + " + l);
                if (isLinesIntersect(line, l)) //CLOSED
                {
                    hash[START_LINE] = line;
                    hash[END_LINE] = l;
                    return true;

                }
            }
        }
        return false;
    }

    private void CheckEnemyCallable(myLine startLine, myLine endLine)
    {
        Vector3 start = startLine.StartPoint;
        Vector3 end = endLine.EndPoint;
        int startI = vertices.IndexOf(start);
        int endI = vertices.IndexOf(end);
        Vector2[] points = vertices.GetRange(startI, endI - startI).ToArray();

        foreach (Enemy obj in enemies)
        {
            Debug.Log("ENEMY");
            Vector2[] ver = obj.getVertices();
            if (areAllPointsInThePolygon(ver, points))
            {
                Debug.Log("TEST");
                status.CircledEnemy(obj);
            }
        }
    }

    public bool areAllPointsInThePolygon(Vector2[] points, Vector2[] polygon, float correct = 0.1f)
    {
        foreach (Vector2 point in points)
        {
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
                if (b)
                    return true;
                if (start.x + f > end.x || start.y + f > end.y)
                    break;
            }
        }

        return false;
    }

    private bool IsInPolygon(Vector2[] poly, Vector2 p)
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


    private void addVertex()
    {
        if (vertices.Count > 1)
        {
            distance += dis(vertices[vertices.Count - 1], vertices[vertices.Count - 2]);
        }

    }
    private void removeVertex()
    {
        if (vertices.Count > 1)
        {
            distance -= dis(vertices[0], vertices[1]);

        }
    }

    float dis(Vector3 p1, Vector3 p2)
    {
        return Vector3.Distance(p1, p2);
    }


    public class myLine
    {
        public Vector3 StartPoint;
        public Vector3 EndPoint;
        public bool zero;

        public myLine(Vector3 start, Vector3 end)
        {
            this.StartPoint = start;
            this.EndPoint = end;
            this.zero = false;
        }

        public myLine()
        {
            this.zero = true;
        }

        private static myLine zeroLine = new myLine();

        public static myLine zero_line()
        {
            return zeroLine;
        }

    };

    //    -----------------------------------    
    //    Following method checks whether given two points are same or not
    //    -----------------------------------    
    private static bool checkPoints(Vector3 pointA, Vector3 pointB)
    {
        return (pointA.x == pointB.x && pointA.y == pointB.y);
    }


    public void Damage(myLine myline, float damage)
    {
        HealthScript.DecreaseBar(damage);
        resetDatas();
        block = true;
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