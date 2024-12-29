using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class AttackerScript : MonoBehaviour {

    private bool isLineCollide(List<Vector3> list)
    {
        
        if (list.Count < 2 || executables.Count == 0)
            return false;
        int TotalLines = list.Count - 1;
        MainGameScript.myLine[] lines = new MainGameScript.myLine[TotalLines];
        if (TotalLines > 1)
        {
            for (int i = 0; i < TotalLines; i++)
            {
                lines[i].StartPoint = (Vector3)list[i];
                lines[i].EndPoint = (Vector3)list[i + 1];
            }
        }

        foreach (DictionaryEntry e in (Hashtable)executables.Clone())
        {
            Hashtable hash = (Hashtable)e.Key;
            List<MainGameScript.myLine> linesToCheck = new List<MainGameScript.myLine>(); 
            if ((AttackType)hash[CONST.TYPE_TEMP] == AttackType.VERTEX_SHOOTER)
            {
                foreach(Hashtable h in (List<Hashtable>)hash[VertexShooterAttack.TEMP_VERTEX])
                {
                    GameObject obj = (GameObject)h[VertexShooterAttack.GAME_OBJECT];
                    LineRenderer ren = obj.GetComponent<LineRenderer>();
                    linesToCheck.Add(new MainGameScript.myLine(obj.transform.TransformPoint(gameObject.transform.TransformPoint((Vector2)h[VertexShooterAttack.START])), obj.transform.TransformPoint(gameObject.transform.TransformPoint((Vector2)h[VertexShooterAttack.END]))));
                }
            }
            foreach (MainGameScript.myLine currentLine in linesToCheck)
            {
                for (int i = 0; i < TotalLines - 1; i++)
                {
                    if (MainGameScript.isLinesIntersect(lines[i], currentLine))
                    {
                        Debug.Log("Collision");
                        return true;
                    }

                }
            }
        }
        return false;
    }

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


    private Hashtable executables = new Hashtable();
    private TextAsset last;
    
    public void Update()
    {
        foreach(DictionaryEntry e in (Hashtable)executables.Clone())
        {
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
                    VertexShooterAttack.PrepareToAttack(this, hash);
                    executables.Add(hash, (System.Action<AttackerScript, Hashtable>)VertexShooterAttack.ShootTick);
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

        VS_DATA_VERTEX = "vsdatavertex", VS_SHOOT_LOCATIONS = "vsdatalocation", VS_DATA_START_VERTEX = "vsstartvertex", VS_DATA_END_VERTEX = "vsendvertex", VS_DATA_VELOCITY_ADDITION = "vsdataadditionvelocity",

        TYPE_TEMP = "typetemp";
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
        return;
        foreach (DictionaryEntry e in executables)
        {

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

    public static class VertexShooterAttack
    {
        public const string TEMP_VERTEX = "vsdatatmpvertex";

        public const int START = 0, END = 1, VELOCITY_VECTOR = 3, GAME_OBJECT = 4, TEMPOBJECT = 5;

        public static void PrepareToAttack(AttackerScript script, Hashtable hash)
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

        public static void ShootTick(AttackerScript script, Hashtable hash)
        {
            List<Hashtable> list = (List<Hashtable>)hash[TEMP_VERTEX];
            if (list.Count == 0)
            {
                script.EndOfAttack(hash);
            }

            foreach (Hashtable h in new List<Hashtable>(list))
            {
                GameObject obj = (GameObject)h[TEMPOBJECT];
                if (isOutsideScreen(obj.transform.TransformPoint(script.transform.TransformPoint((Vector2)h[END]))))
                {
                    GameObject.Destroy(obj);
                    list.Remove(h);
                }
            }
            
        }

        public static bool isOutsideScreen(Vector2 v2)
        {
            Vector2 v = Camera.main.WorldToViewportPoint(v2);
            if(v.x > 1f || v.x < 0f || v.y > 1f || v.y < 0f)
            {
                return true;
            }
            
            return false;
        }


    }

    public static Vector2 calculatePoint(Vector2 start, Vector2 end, float k)
    {
        //(x,y) = (x1 + k(x2 - x1), y1 + k(y2 - y1)
        float x = (start.x + (k * (end.x - start.x)));
        float y = (start.y + (k * (end.y - start.y)));
        return new Vector3(x, y, 0f);
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