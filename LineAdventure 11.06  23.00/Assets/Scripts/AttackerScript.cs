using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class AttackerScript : MonoBehaviour {

    public enum AttackType
    {
        VERTEX_SHOOTER, IDLE
    }

    public static class CONST
    {
        public const string 
        VS_DATA_REACT_ON_PROXIMETRY = "vsdatareact", VS_DATA_REACT_ON_CIRCLE = "vsdatareact", VS_DATA_PROXIMETRY_DETECTION = "vsdatadetection", VS_DATA_WARNING_TIME = "vswarningtime", VS_DATA_RANDOM_SHOOT = "vsrandomshoot",VS_DATA_SHOOT_DIAMETER = "vsdatashootdiameter", VS_DATA_SHOOT_LENGHT = "vsdatashootlenght",
        VS_SHOOT_LOCATIONS = "vsdatalocation", VS_SHOOT_UPDATE = "vsshootupdate";
    }
    
    private Hashtable DATA;
    public Hashtable data = new Hashtable();
    public List<MainGameScript.myLine> colisionsPoints = new List<MainGameScript.myLine>();
    public InteractivityScript script;

    void Start()
    {
        if ((script = gameObject.GetComponent<InteractivityScript>()) == null)
        {
            throw new System.InvalidOperationException("You need InteractiveScript component attached to this gameobject!");
        }
    }

    public Vector2 v2;
    public Vector2 middle;
    public float r;
    void OnDrawGizmos()
    {
        if(false)
        {
            test(middle, r, v2);
            return;
        }
        
        if(true)
        {
            generateGizmosMiddle(side);
            return;
        }
        this.data[CONST.VS_DATA_SHOOT_LENGHT] = 20f;
        foreach (DictionaryEntry e in VertexShooterAttackType.SHOOT(this, this.data))
        {
            Gizmos.DrawLine((Vector2)e.Key, (Vector2)e.Value);
        }
    }

    public static class VertexShooterAttackType
    {
        public static void startPrepare(AttackerScript script)
        {
            return;            
        }

        public static void update(AttackerScript script)
        {
            
        }

        public static Hashtable SHOOT(AttackerScript script, Hashtable data)
        {
            List<Vector2> ShootPoints = new List<Vector2>();
            foreach(Vector3 v3 in script.gameObject.GetComponent<MeshFilter>().mesh.vertices)
            {
                ShootPoints.Add(script.gameObject.transform.TransformPoint(v3));
            }
            Hashtable hashtable = new Hashtable();
            float lenght = (float)data[CONST.VS_DATA_SHOOT_LENGHT];

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


    public int side = 1;

    public void generateGizmosMiddle(int side)
    {
        Mesh m = gameObject.GetComponent<MeshFilter>().mesh;
        if(m.vertexCount < side)
        {
            return;
        }
        Vector2 v1 = this.gameObject.transform.TransformPoint(m.vertices[side-1]);
        Vector2 v2 = this.gameObject.transform.TransformPoint(m.vertices[side == m.vertexCount ? 0 : side]);
        Gizmos.DrawCube(calculatePoint(v1, v2, 0.5f), new Vector3(10, 10, 0));
    }
    public static Vector2 calculatePoint(Vector2 start, Vector2 end, float k)
    {
        //(x,y) = (x1 + k(x2 - x1), y1 + k(y2 - y1)
        float x = (start.x + (k * (end.x - start.x)));
        float y = (start.y + (k * (end.y - start.y)));
        return new Vector3(x, y, 0f);
    }
    public static Vector2[] GetPositionsOnEdgeOfCircle(Vector2 middle, float r, float x)
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
        return new Vector2[] { new Vector2(x, -1*(root)+ middle.y), new Vector2(x, root + middle.y )};
    }

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
    public static float p(float p1, float p2)
    {
        return Mathf.Pow(p1, p2);
    }

    public static Vector2[] GetTwoPointsOnCircle(Vector2 middle, float r, Line line)
    {
        /*
         *  y = ax+b
         *  
         * 1. y = -/ + b
         * cx+d = -/(r^2 - (x-a)^2)
         * (cx+d)^2 = r^2 - (x-a)^2
         * r^2 = -(x-a)^2 - (cx+d)^2
         * 
         * 
         * 
         * 
         */


     //   float powerOfxa = Mathf.Pow(x - middle.x, 2);
       // float powerofr = r * r;
        //float root = Mathf.Sqrt(powerofr - powerOfxa);
        return null;
    }

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
