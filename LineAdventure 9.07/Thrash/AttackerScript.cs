using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System;
using System.Text;

public class AttackerScript : MonoBehaviour {


    public Hashtable tempHashData = new Hashtable();
    public TextAsset DataFile;

    public enum AttackType
    {
        VERTEX_SHOOTER, IDLE
    }

   

    public void Start()
    {
        if ((script = gameObject.GetComponent<InteractivityScript>()) == null)
        {
            throw new System.InvalidOperationException("You need InteractiveScript component attached to this gameobject!");
        }
        DATA = ((Hashtable)DataThread.GetData(new MemoryStream(DataFile.bytes))).converHashtableToVector2WithClone();
        count = (int)((Hashtable)DATA[CONST.DATA_DATA])[CONST.DATA_COUNT];
    }


    Hashtable executables = new Hashtable();

    private TextAsset last;
    
    public void Update()
    {
        foreach(DictionaryEntry e in (Hashtable)executables.Clone())
        {
            Debug.Log("ACTION!");
            //object key = e.Key;
            ((System.Action<AttackerScript, Hashtable>)e.Value).Invoke(this, (Hashtable)e.Key);

        }
    }

    public void PerformEndAttack()
    {
        for(int i = 0; i < count; i++)
        {
            Hashtable table = (Hashtable)DATA[i];
            if((bool)table[CONST.DATA_EXECUTE_ON_THE_END])
            {
                if(((AttackType)table[CONST.DATA_ATTACK_TYPE]) == AttackType.VERTEX_SHOOTER)
                {
                    //Debug.Log(table[CONST.VS_DATA_VERTEX].GetType());
                    Hashtable hash = new Hashtable();
                    hash.Add(CONST.VS_SHOOT_LOCATIONS, table[CONST.VS_DATA_VERTEX]);
                    VertexShooterAttackType2.startPrepare3(this, hash);
                    executables.Add(hash, (System.Action<AttackerScript, Hashtable>)VertexShooterAttackType2.Shoot3);
                }
            }
        }
    }

    public static class CONST
    {
        public const string
        DATA_COUNT = "datacount", DATA_DATA = "datadata", DATA_ATTACK_TYPE = "dataattacktype",

        DATA_REACT_ON_PROXIMETRY = "vsdatareactproxymetry", DATA_REACT_ON_CIRCLE = "vsdatareactcircle", DATA_PROXIMETRY_DETECTION_RADIUS = "vsdatadetectioradios",
        DATA_WARNING_TIME = "vswarningtime", DATA_EXECUTE_ON_THE_END = "executeontheend", DATA_PROXIMETRY_TIME_OUT = "dataproxtimeout", DATA_CIRCLE_TIME_OUT = "datacircletimeout",
        DATA_MOVE_DURING_EXECUTION = "vsmoveduringexecution",

        VS_DATA_VERTEX = "vsdatavertex",
      //VERTEX_DATA_VERTEX_MAIN = "vsvertexdatamain", VERTEX_DATA_VERTEX_SECOND = "vsvertexdatasecond", VERTEX_DATA_X_ADDITIONAL = "vsvertexdataxadd", VERTEX_DATA_LINE = "vertexdataline",
        VS_SHOOT_LOCATIONS = "vsdatalocation",// VS_SHOOT_UPDATE = "vsshootupdate", VS_DATA_SPEED = "vsdataspeed", VS_SYSTEM_LINE_RENDERER = "vssystemrenderer",

        VS_DATA_START_VERTEX = "vsstartvertex", VS_DATA_END_VERTEX = "vsendvertex", VS_DATA_VELOCITY_ADDITION = "vsdataadditionvelocity";
            
            
    }
    
    private Hashtable DATA;
    //public Hashtable data = new Hashtable();
    public List<MainGameScript.myLine> colisionsPoints = new List<MainGameScript.myLine>();
    public InteractivityScript script;

    public int count;

    public Vector2 v2;
    public Vector2 middle;
    public float r;
    

    public void EndOfAttack(Hashtable key)
    {
        executables.Remove(key);
        if(executables.Count == 0)
        {
            script.setAttackState(false);
        }
    }



    [Serializable]
    public struct LineRenderProperty
    {
        public float width;
        public Material material;
        public Color color;
    }

    public LineRenderProperty[] linerendering;

    public void OnDrawGizmos()
    {
        //Debug.Log("DRAWING");
        foreach (DictionaryEntry e in executables)
        {
            Hashtable hash = (Hashtable)e.Key;
            foreach(Hashtable h in (List<Hashtable>)hash["vsdatatmpvertex"])
            {
                //Debug.Log("VERTEX: "+h[1]);
                Gizmos.DrawCube((Vector2)h[1], new Vector2(100, 100));
            }
           // Debug.Log(hash[5]);
            

        }
    }
    private static readonly System.DateTime Jan1st1970 = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

    public static long CurrentTimeMillis()
    {
        return (long)(System.DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
    }



    public static GameObject CreateInstanceLineRendering(LineRenderProperty property)
    {
        GameObject obj = new GameObject();
        LineRenderer line = obj.AddComponent<LineRenderer>();
        Rigidbody2D d2 = obj.AddComponent<Rigidbody2D>();
        d2.mass = 0;
        d2.gravityScale = 0;
        line.useWorldSpace = false;
        line.SetColors(property.color, property.color);
        line.SetWidth(property.width, property.width);
        line.material = property.material;
        return obj;
    }

    public static class VertexShooterAttackType2
    {
        public const string TEMP_VERTEX = "vsdatatmpvertex";
   //     public const int START = 0, END = 1, LINE = 2, ADD = 3, SPEED = 4, FINALVECTOR = 5, LAST_START_VECTOR = 6, LAST_END_VECTOR = 7, LINE_RENDERER = 8, ADDITIONAL = 9, LENGTH = 10, TEMPTIME = 11, ;

        public const int START = 0, END = 1, VELOCITY_VECTOR = 3, GAME_OBJECT = 4, TEMPOBJECT = 5;

        public static void startPrepare3(AttackerScript script, Hashtable hash)
        {
            List<Hashtable> list;
            hash[TEMP_VERTEX] = list = new List<Hashtable>();
            int i = 0;
            foreach (Hashtable h in (List<Hashtable>)hash[CONST.VS_SHOOT_LOCATIONS])
            {
                Vector2 start = (Vector2)h[CONST.VS_DATA_START_VERTEX];
                Vector2 end = (Vector2)h[CONST.VS_DATA_END_VERTEX];
                GameObject obj = AttackerScript.CreateInstanceLineRendering(script.linerendering[i++]);
                LineRenderer rendering = obj.GetComponent<LineRenderer>();
                rendering.SetVertexCount(2);
                rendering.SetPositions(new Vector3[] { rendering.transform.InverseTransformPoint(script.transform.TransformPoint(start)), rendering.transform.InverseTransformPoint(script.transform.TransformPoint(end)) });
                float speed = (float)h[CONST.VS_DATA_VELOCITY_ADDITION];
                Vector2 sTrans = script.transform.TransformPoint(start);
                Vector2 eTrans = script.transform.TransformPoint(end);
                float xend = (eTrans.x - sTrans.x) * speed;
                float yend = (eTrans.y - sTrans.y) * speed;
                Vector2 test = new Vector2(xend, yend);
                obj.GetComponent<Rigidbody2D>().velocity = rendering.transform.InverseTransformVector(test);
                Hashtable has = new Hashtable();
                has[START] = start; has[END] = end; has[TEMPOBJECT] = obj;
                list.Add(has);
            }
        }

        public static void Shoot3(AttackerScript script, Hashtable hash)
        {
            Debug.Log("SHOOT 2");
            List<Hashtable> list = (List<Hashtable>)hash[TEMP_VERTEX];
            Debug.Log(list.Count);
            if (list.Count == 0)
            {

                script.EndOfAttack(hash);
            }

            foreach (Hashtable h in new List<Hashtable>(list))
            {
                GameObject obj = (GameObject)h[TEMPOBJECT];
                //Debug.Log("Positions " + obj.transform.position + " " + obj.transform.TransformPoint((Vector2)h[END]) + " " + Camera.main.WorldToViewportPoint(obj.transform.position) + " " + Camera.main.WorldToViewportPoint(obj.transform.TransformPoint((Vector2)h[END])) + " " + obj.transform.TransformPoint(script.transform.TransformPoint((Vector2)h[END])));
                if (isOutsideScreen(obj.transform.TransformPoint(script.transform.TransformPoint((Vector2)h[END]))))
                {
                    GameObject.Destroy(obj);
                    list.Remove(h);
                }
            }
            
        }

        //public static void startPrepare2(AttackerScript script, Hashtable hash)
        //{
        //    List<Hashtable> list;
        //    hash[TEMP_VERTEX] = list = new List<Hashtable>();
        //    int i = 0;
        //    foreach (Hashtable h in (List<Hashtable>)hash[CONST.VS_SHOOT_LOCATIONS])
        //    {
        //        Vector2 start = (Vector2)h[CONST.VERTEX_DATA_VERTEX_MAIN];
        //        Vector2 end = (Vector2)h[CONST.VERTEX_DATA_VERTEX_SECOND];
        //        Line line = (Line)h[CONST.VERTEX_DATA_LINE];
        //        bool b = (bool)h[CONST.VERTEX_DATA_X_ADDITIONAL];
        //        float speed = (float)h[CONST.VS_DATA_SPEED];
        //        float lenght = Vector3.Distance(start, end);
        //        float diff = Math.Abs(start.x - end.x);
        //        LineRenderer rendering = GameObject.Instantiate<LineRenderer>(script.RenderingLinesList[i++]);
        //        rendering.SetVertexCount(2);
        //        Hashtable has = new Hashtable();
        //        has[START] = start; has[END] = end; has[LINE] = line; has[ADD] = b; has[SPEED] = speed; has[LAST_START_VECTOR] = start; has[ADDITIONAL] = diff; has[LENGTH] = lenght; has[TEMPTIME] = AttackerScript.CurrentTimeMillis();
        //        has[LINE_RENDERER] = rendering;
        //        list.Add(has);
        //   }

        //}


        //public static void startPrepare(AttackerScript script, Hashtable hash)
        //{
        //    List<Hashtable> list;
        //    hash[TEMP_VERTEX] = list = new List<Hashtable>();
        //    int i = 0;
        //    foreach(Hashtable h in (List<Hashtable>)hash[CONST.VS_SHOOT_LOCATIONS])
        //    {
        //        Vector2 start = (Vector2)h[CONST.VERTEX_DATA_VERTEX_MAIN];
        //        Vector2 end = (Vector2)h[CONST.VERTEX_DATA_VERTEX_SECOND];
                


        //        Line line = (Line)h[CONST.VERTEX_DATA_LINE];
        //        bool b = (bool)h[CONST.VERTEX_DATA_X_ADDITIONAL];
        //        float speed = (float)h[CONST.VS_DATA_SPEED];
        //        Vector2 finalVector = calculateLastPoint(line, end, b, 1f);
        //        Hashtable has = new Hashtable();
        //        float tx = b ? start.x + speed : start.x - speed;
        //        Vector2 LastEndV = new Vector2(start.x, start.y); //new Vector2(tx, line.getY(tx));
        //        /*
        //        do
        //        {
        //            if (Vector2.Distance(LastEndV, start) < Vector2.Distance(start, end))
        //            {
        //                tx = b ? tx + speed : tx - speed;
        //                LastEndV.Set(tx, line.getY(tx));
        //            }
        //            else
        //            {
        //                break;
        //            }
        //        } while (true);
        //        */

        //        LineRenderer rendering = GameObject.Instantiate<LineRenderer>(script.RenderingLinesList[i++]);
        //        rendering.SetVertexCount(2);
        //        has[START] = start; has[END] = end; has[LINE] = line; has[ADD] = b; has[SPEED] = speed; has[FINALVECTOR] = finalVector; has[LAST_START_VECTOR] = start; has[LAST_END_VECTOR] = LastEndV;
        //        has[LINE_RENDERER] = rendering;
        //        Debug.Log("ENTRY START");
        //        foreach (DictionaryEntry e in has)
        //        {
        //            Debug.Log("ENTRY: " + e.Key + " V: " + e.Value);
        //        }
        //        Debug.Log("Entrt END");
        //        list.Add(has);
        //    }

        //}
        public static bool isOutsideScreen(Vector2 v2)
        {
            Vector2 v = Camera.main.WorldToViewportPoint(v2);
            Debug.Log("TRanslated " + v2 + " TO " + v);
            Debug.Log((v.x > 1f) + " " + (v.x < 0f) + " " + (v.y > 1f) + " " + (v.y < 0f));
            if(v.x > 1f || v.x < 0f || v.y > 1f || v.y < 0f)
            {
                Debug.Log("IS OUT :(");
                return true;
            }
            
            return false;
        }
        private static bool isOutsideScreenWithConversion(Transform transform, Vector2 v2)
        {
            Vector2 v = Camera.main.WorldToViewportPoint(transform.TransformPoint(v2));
            Debug.Log("TRanslated " + v2 + " TO " + v);
            Debug.Log((v.x > 1f) + " " + (v.x < 0f) + " " + (v.y > 1f) + " " + (v.y < 0f));
            if (v.x > 1f || v.x < 0f || v.y > 1f || v.y < 0f)
            {
                return true;
            }
            Debug.Log("IS OUT :(");
            return false;
        }
        private static Vector2 calculateLastPoint( Line line, Vector2 start, bool add, float jump)
        {
            Vector2 temp = Camera.main.WorldToScreenPoint(start);
            float X = add ? temp.x + jump : temp.x - jump;
            if (X < 0)
                X = 0;
            else if (X > Screen.width)
                X = Screen.width;
            Vector2 finalV = Vector2.zero;
            if(add)
            {
                for(float i = X; i < Screen.width; i+=jump)
                {
                    float Y = line.getY(i);
                    if(isOutsideScreen(new Vector2(X, Y)))
                    {
                        finalV = new Vector2(i-jump, line.getY(i-jump));
                        break;
                    }
                }
            }
            else
            {
                for (float i = X; i > 0; i -= jump)
                {
                    float Y = line.getY(i);
                    if (Y < 0 || Y > Screen.height)
                    {
                        finalV = new Vector2(i - jump, line.getY(i - jump));
                        break;
                    }
                }
            }
            return Camera.main.ScreenToWorldPoint(finalV);

        }
        //public static void Shoot2(AttackerScript script, Hashtable hash)
        //{
        //    Debug.Log("SHOOT 2");
        //    List<Hashtable> list = (List<Hashtable>)hash[TEMP_VERTEX];
        //    Debug.Log(list.Count);
        //    if (list.Count == 0)
        //    {

        //        script.EndOfAttack(hash);
        //    }
            
        //    foreach (Hashtable h in new List<Hashtable>(list))
        //    {
        //        float speed = script.DEBUG_SPEED;//(float)h[SPEED];
        //        if ((CurrentTimeMillis() - (long)h[TEMPTIME]) < speed)
        //        {
        //            return;
        //        }
        //        else {
        //            Vector2 LastStartV = (Vector2)h[START];
        //            Vector2 LastEndV = (Vector2)h[END];
        //            ((LineRenderer)h[LINE_RENDERER]).SetPositions(new Vector3[] { script.gameObject.transform.TransformPoint(LastStartV), script.gameObject.transform.TransformPoint(LastEndV) });
        //            Vector2 st = LastEndV;
        //            float x = (bool)h[ADD] ? LastEndV.x + (float)h[ADDITIONAL] : LastEndV.x - (float)h[ADDITIONAL];
        //            //x = x * 0.5f;
        //            Vector2 end = new Vector2(x, ((Line)h[LINE]).getY(x));
        //            if (isOutsideScreenWithConversion(script.gameObject.transform, end))
        //            {
        //                GameObject.DestroyObject((LineRenderer)h[LINE_RENDERER]);
        //                list.Remove(h);
        //            }
        //            h[START] = st;
        //            h[END] = end;
        //            h[TEMPTIME] = CurrentTimeMillis();
        //        }
        //    }
        //}
        //public static void Shoot(AttackerScript script, Hashtable hash)
        //{

        //    List<Hashtable> list = (List<Hashtable>)hash[TEMP_VERTEX];
        //    if(list.Count == 0)
        //    {
        //        script.EndOfAttack(hash);
        //    }
        //    foreach (Hashtable h in list)
        //    {
        //        Debug.Log("Final Vector: " + h[FINALVECTOR]);
        //        Vector2 LastStartV = (Vector2)h[START];
        //        Vector2 LastEndV = (Vector2)h[END];
        //        float speed = (float)h[SPEED];
        //        ((LineRenderer)h[LINE_RENDERER]).SetPositions(new Vector3[] {LastStartV, LastEndV});
        //        Vector2 st = LastEndV;
        //        float x = (bool)h[ADD] ? LastEndV.x + speed : LastEndV.x - speed;
        //        Vector2 end = new Vector2(x, ((Line)h[LINE]).getY(x));
        //        if (Vector2.Distance(end, (Vector2)h[FINALVECTOR]) < speed)
        //        {
        //            list.Remove(h);
        //        }

        //    }
        //}

    }

    /*
    public static class VertexShooterAttackType
    {
        public const string DATA_TEMP = "vsatdatatemp";
        public static void startPrepare(AttackerScript script)
        {
            

        }

        public static void update(AttackerScript script, Hashtable hash)
        {
            
        }


        /*
        public static Hashtable SHOOT(AttackerScript script, Hashtable data)
        {
            List<Vector2> ShootPoints = new List<Vector2>();
            foreach(Vector3 v3 in script.gameObject.GetComponent<MeshFilter>().mesh.vertices)
            {
                ShootPoints.Add(script.gameObject.transform.TransformPoint(v3));
            }
            Hashtable hashtable = new Hashtable();
            //float lenght = (float)data[CONST.VS_DATA_SHOOT_LENGHT];

            List<MainGameScript.myLine> lines = new List<MainGameScript.myLine>();

            //if((bool)data[CONST.VS_DATA_RANDOM_SHOOT])
            if(true)
            {
                //Vector2[] vector = new Vector2[ShootPoints.Count];
                Hashtable ta = new Hashtable();
                int i = 0;
                Vector2 vvvv = script.gameObject.GetComponent<MeshFilter>().mesh.bounds.center;
                //Gizmos.DrawCube(vvvv, new Vector3(10, 10));
                vvvv = script.transform.TransformPoint(vvvv);
                Gizmos.DrawCube(vvvv, new Vector3(10, 10));

                foreach (Vector2 v2 in ShootPoints)
                {
 
                    //(x - a)^2 + (y - b)^2 = r^2
                    Vector2 final = Vector2.zero;
                    bool b = true;
                    do
                    {
                        float  x = Random.Range(v2.x - lenght, v2.x + lenght);
                        Vector2[] finals = AttackerScript.GetPositionsOnEdgeOfCircle(v2, lenght, x);
                        if(!MainGameScript.IsInPolygon(script.gameObject.GetComponent<MeshFilter>().mesh.vertices.toVector2Array(), finals[0]))
                        {
                            final = finals[0];
                            b = false;
                        }
                        else if(!MainGameScript.IsInPolygon(script.gameObject.GetComponent<MeshFilter>().mesh.vertices.toVector2Array(), finals[1]))
                        {
                            final = finals[1];
                            b = false;
                        }
                        else
                        {
                            b = true;
                        }
                    } while (b);
                    ta[v2] = final;
                }
                return ta;
            }

        }

    }
    */

    public int side = 1;

    public Vector2 LocationOfLaserPoint;
    public Vector2 ShootDirection;
    public Vector2 Temp;


    /*
    public void generateGizmosMiddle(int side)
    {
        Mesh m = gameObject.GetComponent<MeshFilter>().mesh;
        if(m.vertexCount < side)
        {
            return;
        }
        Vector2 v1 = this.gameObject.transform.TransformPoint(m.vertices[side-1]);
        Vector2 v2 = this.gameObject.transform.TransformPoint(m.vertices[side == m.vertexCount ? 0 : side]);
        Vector2 middle = calculatePoint(v1, v2, 0.5f);
      //  Gizmos.DrawCube( middle , new Vector3(10, 10, 0));
        int vPut = side - 1;
        int vEndPut = side;
        float proportion = 5/Vector3.Distance(v1, v2);
        Vector2 gen1 = calculatePoint(middle, v1, proportion);
        Vector2 gen2 = calculatePoint(middle, v2, proportion);        Gizmos.DrawLine(gen1, gen2);
 
    }
    */
    public static Vector2 calculatePoint(Vector2 start, Vector2 end, float k)
    {
        //(x,y) = (x1 + k(x2 - x1), y1 + k(y2 - y1)
        float x = (start.x + (k * (end.x - start.x)));
        float y = (start.y + (k * (end.y - start.y)));
        return new Vector3(x, y, 0f);
    }

    /*
    public static Vector2[] GetPositionsOnEdgeOfCircle(Vector2 middle, float r, float x)
    {
        float powerOfxa = Mathf.Pow(x - middle.x, 2);
        float powerofr = r * r;
        float root = Mathf.Sqrt(powerofr - powerOfxa);
        return new Vector2[] { new Vector2(x, -1*(root)+ middle.y), new Vector2(x, root + middle.y )};
    }
    */

    /*
    public static void test(Vector2 middle, float r, Vector2 v1)
    {
        Line line = Line.getLine(middle, v1);
        float c = line.a;
        float d = line.b;
        float a = middle.x;
        float b = middle.y;

        Vector3 test = new Vector3(5, 5, 0);
        Gizmos.DrawCube(middle, test);
        Gizmos.DrawCube(v1, test);

        float A = (1 + (2 * p(c, 2)));
        float B = ((2 * c * d) - (2*a) - (2*c*b));
        float C = ((-2 *d * b) + p(a, 2) + p(d, 2) + p(b,2) - p(r, 2));

        float X = (-B) / (2*A);
        float DELTA = (p(B, 2)) - (4 * A*C);
        float Y = -DELTA - (4 * A );

        Gizmos.DrawSphere(new Vector3(X, Y, 0), 10f);
        //X = -b / 2a
        //X^2(1+2c^2)+x(2cd-2a-2cb) – 2db + a^2 + d^2 + b^2 – r^2 = 0


    }
    */
    public static float p(float p1, float p2)
    {
        return Mathf.Pow(p1, p2);
    }

/*
 *  public static Vector2[] GetTwoPointsOnCircle(Vector2 middle, float r, Line line)
 *  {
 *       
 *        *  y = ax+b
 *        *  
 *        * 1. y = -/ + b
 *        * cx+d = -/(r^2 - (x-a)^2)
 *        * (cx+d)^2 = r^2 - (x-a)^2
 *        * r^2 = -(x-a)^2 - (cx+d)^2
 *       
 *       
 *
 *
 *    //   float powerOfxa = Mathf.Pow(x - middle.x, 2);
 *     // float powerofr = r * r;
 *       //float root = Mathf.Sqrt(powerofr - powerOfxa);
 *       return null;
 *   }
 */
    public static Vector2[] GetPositionsOnEdgeOfCircleWithRange(Vector2 middle, float r, float x)
    {
        /*
         * (x-a)^2 + (y-b)^2 = r^2
         * (y-b)^2 = r^2 - (x-a)^2 
         * |y-b| = /(r^2 - (x-a)^2)
         * -(y-b) = -||-
         * y-b = -||-
         * 
         * -y = /- - b
         * y = -/ + b
         * y = / + b
         */
        float powerOfxa = Mathf.Pow(x - middle.x, 2);
        float powerofr = r * r;
        float root = Mathf.Sqrt(powerofr - powerOfxa);
        return new Vector2[] { new Vector2(x, -1 * (root) + middle.y), new Vector2(x, root + middle.y) };
    }



    [Serializable]
    public class Line
    {
        public float a, b;
        public MyVector start, end;

        public Line(float a, float b, Vector2 start, Vector2 end)
        {
            this.a = a;
            this.b = b;
            this.start = start.ToMyVector();
            this.end = end.ToMyVector();
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


[Serializable]
public class MyVector
{
    public float x, y;

    public MyVector(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
    public Vector2 toVector2()
    {
        return new Vector2(x, y);
    }
    
}
public static class exten
{
    public static MyVector ToMyVector(this Vector2 v2)
    {
        return new MyVector(v2.x, v2.y);
    }
    public static float specialAdd(this float f1, float f2)
    {
        if(f1 < 0)
        {
            f1 -= f2;

        }
        else if(f1 > 0)
        {
            f1 += f2;
        }
        return f1;
    }
    public static string ToStringAll(this List<object> list)
    {
        StringBuilder builder = new StringBuilder("{ ");
        for(int i = 0; i < list.Count; i++)
        {
            if(i+1 == list.Count)
            {
                builder.Append(list[i].ToString() + " }");
            }
            else
            {
                builder.Append(list[i].ToString() + ", ");
            }
        }
        return builder.ToString();
    }

    public static string[] ToStringListAll(this Vector3[] list)
    {
        string[] strings = new string[list.Length];
        for (int i = 0; i < list.Length; i++)
        {
            strings[i] = list[i].ToString();
        }
        return strings;
    }
}