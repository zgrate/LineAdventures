using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public class iTweenImplementation : MonoBehaviour {
    [Serializable]
    public enum MoveType
    {
        RANDOMIZED, TYPICAL, DIRECTED, STOP, RANDOM_ROTATION
    }


    public MoveData[] MovementDatas;

    public void PutData(RandomPreset preset)
    {
        preset.iTweenImplementation.MoveDatas = MovementDatas;
    }

    public void GetData(RandomPreset preset)
    {
        MovementDatas = preset.iTweenImplementation.MoveDatas;
    }

    [Serializable]
    public class MoveData
    {
        [Header("Main Settings")]
        [Tooltip("Type of movement")]
        public MoveType MoveType;
        [Tooltip("Speed of movement, not used in some cases and overrided by iPathDataContainer if used")]
        public float Speed;
        [Tooltip("Ease type, not used in some cases and overrided by iPathDataContainer if used")]
        public iTween.EaseType EaseType;

        [Header("TYPICAL type settings (Not sure if working)")]
        [Tooltip("Main path to move")]
        public iPathDataContainer iPathToMove;
        [Tooltip("SIMULATE RANDOM WORLD MOVEMENT, LocalSpace have to be true")]
        public bool RandomSimulation;
        [Tooltip("WHAT RANDOM WORLD PATH SHOULD RANDOM OBJECT TRACK")]
        public iPathDataContainer[] RandomPaths;

        [Header("STOP type settings")]
        [Tooltip("How much time should object wait?")]
        public float PauseSeconds;


        [Header("DIRECTED type settings")]
        [Tooltip("What position is start position (in local space)")]
        public Vector2 StartVector;
        [Tooltip("What position is end position (in local space)")]
        public Vector2 EndVector;
        [Tooltip("Should line be drawn in gizmos?")]
        public bool DebugDirectedDraw;
        [Tooltip("Should X be added or substracted?")]
        public bool AddedX;
        [Tooltip("If Debug Draw is true, how much change X to draw a point?")]
        public float AddX;
        [Tooltip("If only move for given distance, if set to 0, distance is random")]
        public float FixedAdd = 0;

        [Header("RANDOM_ROTATION type settings")]
        [Tooltip("Max angle of rotation")]
        public float MaxAdd = 0;
        [Tooltip("True = Max, False = Fixed")]
        public bool MaxOrFixed;
        
        [HideInInspector]
        public Line line;

    }



    /// <summary>
    /// Checks if given world location is outside the screen, WARNING!! CUSTOM COUNTING!
    /// </summary>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static bool isOutsideScreenWorld(Vector2 v2)
    {
        Vector2 v = Camera.main.WorldToViewportPoint(v2);
        if (v.x > 1f || v.x < 0f || v.y > 1f || v.y < 0f)
        {
            return true;
        }

        return false;
    }



    [Serializable]
    public class iPathDataContainer : ICloneable
    {
        //PATH TO ITWEEN GENERATED PATH
        public string iTweenPathString;
        //SPEED OF MOVEMENT
        public float Speed;
        //LOOP TYPE
        public iTween.LoopType LoopType;
        //EASE TYPE
        public iTween.EaseType EaseType;
        //METHOD WHICh WILL BE EXECUTED AT START
        public string StartMethod = "OnStart";
        //METHOD WHICH WILL BE EXECUTED AT UPDATE
        public string UpdateMethod = "OnUpdate";
        //METHOD WHICH WILL BE EXECUTED AFTER EXECUTION
        public string CompleteMethod = "OnComplete";
        //GAMEOBJECT REFERENCE
        public GameObject Reference;
        //USE LOCAL OR WORLD SPACE
        public bool LocalSpace;
        //TEST VALUE
        //public bool MoveToPath;


        public Hashtable GenerateHashtable(bool MoveToPath)
        {
            return iTween.Hash("path", iTweenPath.GetPath(iTweenPathString), "speed", Speed, "islocal", LocalSpace, "easetype", EaseType, "looptype", LoopType, "onstart", StartMethod, "onstarttarget", Reference, "onupdate", UpdateMethod, "onupdatetarget", Reference, "oncomplete", CompleteMethod, "oncompletetarget", Reference, "movetopath", MoveToPath);
        }
        public object Clone()
        {
            iPathDataContainer container = new iPathDataContainer();
            container.iTweenPathString = iTweenPathString;
            container.Speed = Speed;
            container.LoopType = LoopType;
            container.EaseType = EaseType;
            container.StartMethod = StartMethod;
            container.UpdateMethod = UpdateMethod;
            container.CompleteMethod = CompleteMethod;
            container.Reference = Reference;
            container.LocalSpace = LocalSpace;
            return container;
        }
    }
    



    private int MoveCounter = 0;
    
    public class DirectedMovementHandler
    {
        public static void Move(iTweenImplementation impl, MoveData data)
        {
            GameObject obj = impl.gameObject;
            Vector3 v3 ;
            if (isOutsideScreenWorld(impl.gameObject.GetComponent<MeshRenderer>().bounds.center))
            {
                v3 = Vector3.zero;
            }
            else
            {
                try
                {
                    v3 = GetNextPositionInWorld(impl, data);
                }catch (System.InsufficientMemoryException ex)
                {
                    impl.action = null;
                    return;
                }
        }
            impl.action = impl.NullNothing;
            
            iTween.MoveTo(obj, iTween.Hash("position", v3, "speed", data.Speed, "oncomplete", "OnCompleteDirected", "oncompletetarget", obj, "easetype", data.EaseType, "islocal", false));

        }

        public static Vector2 GetNextPositionInLocal(iTweenImplementation gameObject, MoveData data)
        {
            float x, y;
            float maxX = gameObject.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0))).x;
            float minX = gameObject.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(new Vector3(0, 0))).x;
            
            do
            {

                x = UnityEngine.Random.Range(data.EndVector.x, data.AddedX ? maxX : minX);
                y = data.line.getY(x);

            }
            while (isOutsideScreenWorld(gameObject.transform.TransformPoint(new Vector3(x, y, 0f))));
            gameObject.debugDraw = gameObject.transform.TransformPoint(new Vector2(x, y));
            gameObject.DEBUG2 = new Vector2(x, y);
            return new Vector2(x, y);
        }
        
        public static Vector3 GetNextPositionInWorld(iTweenImplementation gameObject, MoveData data)
        {
            float x, y;
            float maxX = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0)).x;
            float minX = Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).x;
            Vector2 start = gameObject.transform.TransformPoint( data.StartVector);
            Vector2 end = gameObject.transform.TransformPoint( data.EndVector);
            Line line = Line.getLine(start, end);
            bool add = end.x - start.x > 0;
            int i = 0;
            do
            {
                if (i == 10)
                    throw new System.InsufficientMemoryException();
                i += 1;
                x = data.FixedAdd==0 ? UnityEngine.Random.Range(end.x, add ? maxX : minX) : (add ? end.x + data.FixedAdd : end.x - data.FixedAdd);
                y = line.getY(x);
            } while (isOutsideScreenWorld(new Vector2(x, y)));
            return new Vector3(x, y);
        }
    }
    private static readonly System.DateTime Jan1st1970 = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

    public static long CurrentTimeMillis()
    {
        return (long)(System.DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
    } 

    public class PauseHandler
    {
        public static void Pause(iTweenImplementation impl, MoveData data)
        {
            impl.action = Tick;
            impl.DataContainer["startTime"] = CurrentTimeMillis();
            impl.DataContainer["time"] = data.PauseSeconds*1000;
        }

        public static void Tick(iTweenImplementation impl)
        {
            if ((CurrentTimeMillis() - (float)impl.DataContainer["startTime"]) >= (float)impl.DataContainer["time"])
            {
                impl.action = null;
            }
        }
    }

    private Hashtable DataContainer = new Hashtable();
    public class RandomRotationHandler
    {
        public static void StartRotation(iTweenImplementation impl, MoveData data)
        {
            float z = (impl.gameObject.transform.rotation.eulerAngles.z).Cut(1);
            impl.DataContainer["angle"] = (int)( data.MaxAdd == 0 ? UnityEngine.Random.Range(0, 360) : (data.MaxOrFixed ? z + (UnityEngine.Random.Range(-data.MaxAdd, data.MaxAdd)) : (UnityEngine.Random.Range(0, 1f) > 0.5 ? z + data.MaxAdd : z - data.MaxAdd)));
            impl.DataContainer["speed"] = data.Speed-5;
            impl.DataContainer["add"] = UnityEngine.Random.Range(0f, 1f) > 0.5f;
            impl.action = RotateTick;
        }
        public static void RotateTick(iTweenImplementation impl)
        {
            //Debug.Log(impl.gameObject.GetComponent<MeshRenderer>().bounds.center);
            //Debug.Log(((impl.gameObject.transform.rotation.eulerAngles.z).Cut(1) - (int)impl.DataContainer["angle"]) + " " + Mathf.Abs((float)impl.DataContainer["speed"]));
            if (Mathf.Abs((impl.gameObject.transform.rotation.eulerAngles.z).Cut(1) - (int)impl.DataContainer["angle"]) <= Mathf.Abs((float)impl.DataContainer["speed"])+10)
                impl.action = null;
            else
                impl.gameObject.transform.RotateAround(impl.gameObject.GetComponent<MeshRenderer>().bounds.center, new Vector3(0, 0,1), ((bool)impl.DataContainer["add"] ? 1 : -1) * (float)impl.DataContainer["speed"]);
        }

    }
    public Vector2 debugDraw, DEBUG2;
    /// <summary>
    /// Methods that does nothing
    /// </summary>
    public void NullNothing(iTweenImplementation impl) { }

    public void OnCompleteDirected()
    {
        action = null;
    }

    public void StopMovingAndBlock()
    {
        block = true;
        iTween.Stop(gameObject);
        action = null;
        MoveCounter = 0;
    }

    public void StartMovingAndUnblock()
    {
        block = false;
    }

    public Action<iTweenImplementation> action;

    public void MainSystemTick()
    {
        if (block)
            return;
        if(action != null)
        {
            action.Invoke(this);
            return;
        }
        if (MoveCounter == MovementDatas.Length)
        {
            ExecuteAttack();
            MoveCounter = 0;
            return;
        }
        
        MoveData data = MovementDatas[MoveCounter];
        if (data.MoveType == MoveType.DIRECTED)
        {
            DirectedMovementHandler.Move(this, data);
        }
        else if(data.MoveType == MoveType.RANDOM_ROTATION)
        {
            RandomRotationHandler.StartRotation(this, data);
        }
        MoveCounter++;

    }


    public void FixedUpdate()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            foreach (MoveData data in MovementDatas)
            {
                if (data.MoveType == MoveType.DIRECTED)
                {
                    data.line = Line.getLine(data.StartVector, data.EndVector);
                }
            }
        }
 #endif
        MainSystemTick();
    }


    void ExecuteAttack()
    {
        if(gameObject.GetComponent<AttackManager>() != null)
        {
            AttackManager mng = gameObject.GetComponent<AttackManager>();
            bool b = mng.ExecuteAfterAttack(Reset);
            if (!b)
                block = true;

        }
    }

    bool block;

    void Reset()
    {
        block = false;


    }

    // Use this for initialization
    void Start() {
        iTween.Init(gameObject);
        //ExecuteMovement(MovementDatas[0]);
    }


   


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
    public float GetX(float y)
    {
        //y = ax + b |-b
        //y-b = ax |:a
        //(y-b)/a = x
        return ((y - b) / a);
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
