using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[ExecuteInEditMode]
public class AttackManager : MonoBehaviour {


    public MainGameScript MainGameScriptObject;

    [Serializable]
    public enum AttackType
    {
        VERTEX_ATTACKER, LASER_ATTACK, NULL_ATTACK
    }

    [Serializable]
    public class VertexData
    {
        public float Speed;
        public Vector2 StartVector;
        public Vector2 EndVector;
        public GameObject LineRenderer;
        [NonSerialized]
        public int Identity;
    }

    
    [Serializable]
    public class AttackData
    {
        [Header("Main Attack Settings")]
        public AttackType TypeAttack;
        
        [Header("Vertex Attacker / Laser Attack Type")]
        [Tooltip("Vertexes to set")]
        public List<VertexData> Vertexes;

        [Header("Laster Attack")]
        public float RotationSpeed;

        public float MaxLenght, MinLenght;

        public float RotationTime;

    }

    public bool ResetOnLineBreak;
    [Header("AfterMoveAttack Properties")]
    [Tooltip("Attack executed after all movement proggress")]
    public AttackData AfterMoveAttack;
    [Tooltip("If > 1, attack will be executed after given move loops, if 0 - attack will be executed every loop, if < 0, loop methods will be random, and f.e. -10 means execute time Range(0, 10)")]
    public int AttackExecuteTime;
    public bool ContinueMovementAfterMoveAttack;
    public float HPLoseOnAfterAttack;

    [Header("ProximetryAttack Properties")]
    [Tooltip("Attack executed if line will be detected near vertex")]
    public AttackData ProximetryAttack;
    [Tooltip("Range of detection")]
    public float Range;
    public float WaitTime;
    public bool ContinueMovementAfterProximetryAttack;
    public float HPLoseOnProximetry;

    [Header("CirceledAttack Properties")]
    [Tooltip("Attack executed if enemy circeled")]
    public AttackData CircledAttack;

    [Header("DebugMenu")]
    public bool GizmosAfterMoveAttack;
    public bool GizmosProximetryAttackOption;
    public bool GizmosProximetryAttackAttack;
    public bool GizmosCircledAttack;

    public Hashtable DataContainer = new Hashtable();

    private int Method = 0;
    private int TimeTime = 0;

    /// <summary>
    /// Returns True if class should continue movement, false if should wait for completion
    /// </summary>
    /// <returns>True if class should continue movement, false if should wait for completion</returns>
    public bool ExecuteAfterAttack(Action EndMethod)
    {
        if (IsExecutingAttack)
            return true;
        if (AfterMoveAttack.TypeAttack == AttackType.NULL_ATTACK)
            return true;
        if (Method == 0 && AttackExecuteTime != 0)
        {
            if (AttackExecuteTime < 0)
            {
                TimeTime = UnityEngine.Random.Range(0, -AttackExecuteTime);
            }
            Method++;
            return true;
        }
        else {
            if (AttackExecuteTime != 0)
            {
                Method++;

                if (TimeTime != 0)
                {
                    if (Method != TimeTime + 1)
                        return true;
                }
                else
                {
                    if (Method != AttackExecuteTime+1)
                    {
                        return true;
                    }
                }
                Method = 0;
            }
            if (AfterMoveAttack.TypeAttack == AttackType.VERTEX_ATTACKER)
            {
                VertexAttackerHandler.ExecuteAttack(this, AfterMoveAttack);
                Block(true);
                if (!ContinueMovementAfterMoveAttack)
                    ActionAfterAttack = EndMethod;
                return ContinueMovementAfterMoveAttack;
            }
            else if(AfterMoveAttack.TypeAttack == AttackType.LASER_ATTACK)
            {
                LaserAttackHandler.StartAttack(this, AfterMoveAttack);
                Block(true);
                if (!ContinueMovementAfterMoveAttack)
                    ActionAfterAttack = EndMethod;
                return ContinueMovementAfterMoveAttack;
            }
            return true;
        }
    }

    [NonSerialized]
    public Action ActionAfterAttack;
    [NonSerialized]
    public long LastProximetryExecute;

    [NonSerialized]
    public bool IsExecutingAttack;

    /// <summary>
    /// Returns True if class should continue movement, false if should wait for completion
    /// </summary>
    /// <returns>True if class should continue movement, false if should wait for completion</returns>
    public bool ExecuteProximetryAttack(Vector2 LastCursorPosition, Action StopMethod, Action EndMethod)
    {
        Debug.Log("Proximetry");
        if (IsExecutingAttack)
            return true;
        if (ProximetryAttack.TypeAttack == AttackType.NULL_ATTACK)
            return true;
        Debug.Log("PROX 1");
        if (WaitTime != 0)
        {
            if (LastProximetryExecute != 0)
            {
                if ((iTweenImplementation.CurrentTimeMillis() - LastProximetryExecute) > WaitTime * 1000)
                {
                    LastProximetryExecute = 0;
                    goto MAIN;
                }
                else
                {
                    return true;
                }
            }
        }
        Debug.Log("TAK");
    MAIN:
        {
            Mesh m = gameObject.GetComponent<MeshFilter>().mesh;
            foreach (Vector2 v2 in m.vertices)
            {
                if(Vector2.Distance(gameObject.transform.TransformPoint(v2), LastCursorPosition) < Range)
                {
                    Debug.Log("TAKKKKK");
                    LastProximetryExecute = iTweenImplementation.CurrentTimeMillis();
                    goto ATTACK; 
                }
            }
            return true;

            ATTACK:
            if(StopMethod != null)
                StopMethod.Invoke();
            if (ProximetryAttack.TypeAttack == AttackType.VERTEX_ATTACKER)
            {
                VertexAttackerHandler.ExecuteAttack(this, ProximetryAttack);
                Block(true);
                if (!ContinueMovementAfterProximetryAttack)
                    ActionAfterAttack = EndMethod;
                return ContinueMovementAfterProximetryAttack;
            }
            else if (ProximetryAttack.TypeAttack == AttackType.LASER_ATTACK)
            {
                LaserAttackHandler.StartAttack(this, ProximetryAttack);
                Block(true);
                if (!ContinueMovementAfterMoveAttack)
                    ActionAfterAttack = EndMethod;
                return ContinueMovementAfterProximetryAttack;
            }
            return true;
        }
    }
    public void Block(bool b)
    {
        IsExecutingAttack = b;
    }
    public void ResetAfterAttack()
    {
        Method = 0;
        TimeTime = 0;
    }
    public static bool isOutsideScreen(Vector2 v2)
    {
        Vector2 v = Camera.main.WorldToViewportPoint(v2);
        if (v.x > 1f || v.x < 0f || v.y > 1f || v.y < 0f)
        {
            return true;
        }

        return false;
    }

    public class VertexAttackerHandler
    {
        public static Vector2 GetLastVector(AttackManager attacker, VertexData data)
        {
            Vector2 start = attacker.gameObject.transform.TransformPoint(data.StartVector);
            Vector2 end = attacker.gameObject.transform.TransformPoint(data.EndVector);
            AttackerScript.Line line = AttackerScript.Line.getLine(start, end);
            bool add = end.x - start.x > 0;
            float x, y;
            if (add)
            {
                x = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x;
                
            }
            else
            {
                x = Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x;
            }
            y = line.getY(x);

            //if (!isOutsideScreen(new Vector2(x, y)))
            //{
            //    y = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y;
            //    x = line.GetX(y);
            //}
            return new Vector2(x, y);
        }
        public static void ExecuteAttack(AttackManager attacker, AttackData data)
        {
            for(int i = 0; i < data.Vertexes.Count; i++)
            {
                VertexData v = data.Vertexes[i];
                Vector2 lastVector = GetLastVector(attacker, v);
                v.Identity = v.GetHashCode() + UnityEngine.Random.Range(0, 1000) + Time.frameCount;
                GameObject game = Instantiate(v.LineRenderer);
                LineRenderer render = game.GetComponent<LineRenderer>();
                render.useWorldSpace = false;
                render.SetVertexCount(2);
                Vector3[] v3;
                render.SetPositions(v3 = new Vector3[] { (v.LineRenderer.transform.InverseTransformPoint(attacker.transform.TransformPoint(v.StartVector))), (v.LineRenderer.transform.InverseTransformPoint(attacker.transform.TransformPoint(v.EndVector))) });
                render.enabled = true;
                game.SetActive(true);
                attacker.ObjectsToTrack.Add(game);
                iTween.MoveTo(game, (iTween.Hash("position", (Vector3)lastVector, "speed", v.Speed, "oncomplete",
                    "OnCompleteVertexAttacker", "oncompletetarget", attacker.gameObject, "easetype", iTween.EaseType.linear, "islocal", false, "oncompleteparams", game, "onupdate", "OnUpdateVertexAttacker", "onupdatetarget", attacker.gameObject, "onupdateparams", new object[] { game, v3 })));
            }
        }
    }

    void OnCompleteVertexAttacker(GameObject obj)
    {
        ObjectsToTrack.Remove(obj);
        Debug.Log(ObjectsToTrack.Count + " " + ActionAfterAttack);
        if (ObjectsToTrack.Count == 0 && ActionAfterAttack != null)
        {
            ActionAfterAttack.Invoke();
            ActionAfterAttack = null;
            Block(false);
        }

        DestroyObject(obj);
    }

    /// <summary>
    /// objects[0] instanceof GameObject
    /// objects[0] instanceof  Vector3[2];
    /// </summary>
    /// <param name="objects"></param>
    void OnUpdateVertexAttacker(object[] objects)
    {
        Debug.Log("DEBUG");
        GameObject obj = (GameObject)objects[0];
        Vector3[] list = (Vector3[])objects[1];
        int i = 0;
        foreach (Vector3 v2 in list)
        {
            if (!isOutsideScreen(obj.transform.TransformPoint(v2)))
            {
                i++;
            }
        }
        if (i == 0)
        {
            iTween.Stop(obj);
            DestroyObject(obj);
            ObjectsToTrack.Remove(obj);
            if (ObjectsToTrack.Count == 0 && ActionAfterAttack != null)
            {
                ActionAfterAttack.Invoke();
                ActionAfterAttack = null;

            }
            Block(false);
            return;
        }
        else  
        {
            MainGameScript.myLine myline = MainGameScriptObject.isLineColliding(new MainGameScript.myLine(obj.transform.TransformPoint(list[0]), obj.transform.TransformPoint(list[1])));
            if(myline != MainGameScript.myLine.zero_line())
            {
                MainGameScriptObject.Damage(myline, HPLoseOnAfterAttack);

            }
        }
        return;
    }
    public void OnDrawGizmos()
    {

        if (GizmosAfterMoveAttack)
        {
            if (AfterMoveAttack.TypeAttack == AttackType.VERTEX_ATTACKER)
            {
                DrawVertexAttackGizmos(AfterMoveAttack);
            }
            else if(AfterMoveAttack.TypeAttack == AttackType.LASER_ATTACK)
            {
                DrawLaserAttackGizmos(AfterMoveAttack);
            }
        
        }
        if(GizmosProximetryAttackAttack)

        {
            if (ProximetryAttack.TypeAttack == AttackType.VERTEX_ATTACKER)
            {
                DrawVertexAttackGizmos(ProximetryAttack);
            }
            else if (ProximetryAttack.TypeAttack == AttackType.LASER_ATTACK)
            {
                DrawLaserAttackGizmos(ProximetryAttack);
            }
        }
        if(GizmosProximetryAttackOption)
        {
            foreach(Vector3 v3 in this.gameObject.GetComponent<MeshFilter>().sharedMesh.vertices)
            {
                Gizmos.DrawWireSphere(gameObject.transform.TransformPoint(v3), Range);
            }
        }
        if(GizmosCircledAttack)
        {
            if (CircledAttack.TypeAttack == AttackType.VERTEX_ATTACKER)
            {
                DrawVertexAttackGizmos(CircledAttack);
            }
            else if (CircledAttack.TypeAttack == AttackType.LASER_ATTACK)
            {
                DrawLaserAttackGizmos(CircledAttack);
            }
        }

        


    }
    private void DrawVertexAttackGizmos(AttackData data)
    {
        foreach (VertexData vdata in data.Vertexes)
        {
            Gizmos.DrawLine(transform.TransformPoint(vdata.StartVector), transform.TransformPoint(vdata.EndVector));
            Gizmos.DrawSphere(VertexAttackerHandler.GetLastVector(this, vdata), 10);
        }
    }

    private void DrawLaserAttackGizmos(AttackData data)
    {
        foreach (VertexData vdata in data.Vertexes)
        {
            Gizmos.DrawLine(transform.TransformPoint(vdata.StartVector), transform.TransformPoint(vdata.EndVector));
        }
    }

    // Use this for initialization
    void Start()
    {
        if (Application.isPlaying)
        {
            if (AfterMoveAttack.TypeAttack == AttackType.VERTEX_ATTACKER)
            {
                foreach (VertexData vdata in AfterMoveAttack.Vertexes)
                {
                    vdata.LineRenderer.SetActive(false);//.. = false;
                }
            }
            iTween.Init(this.gameObject);
           // VertexAttackerHandler.ExecuteAttack(this, AfterMoveAttack);
        }
    }
    public const int GAME_OBJECT = 0, VERTEXES = 1;
    private List<GameObject> ObjectsToTrack = new List<GameObject>();
	// Update is called once per frame
	void Update () {
        if(Application.isPlaying)
        {
            if(UpdateAction != null)
            {
                UpdateAction.Invoke(this);
            }
        }
	}

    public Action<AttackManager> UpdateAction;
    

    public class LaserAttackHandler
    {
        public static void StartAttack(AttackManager attack, AttackData data)
        {
            List<GameObject> GamesObjects = new List<GameObject>();
            Hashtable hash = new Hashtable();
            foreach(VertexData v in data.Vertexes)
            {
                GameObject game = Instantiate(v.LineRenderer);
                LineRenderer render = game.GetComponent<LineRenderer>();
                render.useWorldSpace = false;
                render.SetVertexCount(2);
                Vector3[] v3;
                render.SetPositions(v3 = new Vector3[] { (v.LineRenderer.transform.InverseTransformPoint(attack.transform.TransformPoint(v.StartVector))), (v.LineRenderer.transform.InverseTransformPoint(attack.transform.TransformPoint(v.EndVector))) });
                render.enabled = true;
                game.SetActive(true);
                game.transform.parent = attack.transform;
                GamesObjects.Add(game);
                hash.Add(game, v3);
            }
            attack.DataContainer["objects"] = GamesObjects;
            attack.DataContainer["verticesObject"] = hash;
            StartRotation(attack, data);
        }
        public static void StartRotation(AttackManager impl, AttackData data)
        {
            impl.DataContainer["speed"] = data.RotationSpeed;
            impl.DataContainer["time"] = iTweenImplementation.CurrentTimeMillis();
            impl.DataContainer["rotatetime"] = data.RotationTime*1000;
            impl.UpdateAction = RotateTick;
        }
        public static void RotateTick(AttackManager attack)
        {
            Debug.Log("RotateTick");
            Hashtable hash = (Hashtable)attack.DataContainer["verticesObject"];
            foreach(GameObject obj in (List<GameObject>)attack.DataContainer["objects"])
            {
                
                Vector3[] list = (Vector3[]) hash[obj];
                MainGameScript.myLine myline = attack.MainGameScriptObject.isLineColliding(new MainGameScript.myLine(obj.transform.TransformPoint(list[0]), obj.transform.TransformPoint(list[1])));
                Debug.Log(obj + " "+ list);
                if (myline != MainGameScript.myLine.zero_line())
                {
                    attack.MainGameScriptObject.Damage(myline, attack.HPLoseOnAfterAttack);
                }
            }
            if(iTweenImplementation.CurrentTimeMillis()-(long)attack.DataContainer["time"] < (float)attack.DataContainer["rotatetime"])
                attack.gameObject.transform.RotateAround(attack.gameObject.GetComponent<MeshRenderer>().bounds.center, new Vector3(0, 0, 1), (float)attack.DataContainer["speed"]);
            else
            {
                attack.UpdateAction = null;
                foreach(GameObject obj in (List<GameObject>)attack.DataContainer["objects"])
                {
                    DestroyObject(obj);
                }
                if(attack.ActionAfterAttack != null)
                {
                    attack.ActionAfterAttack.Invoke();
                    attack.ActionAfterAttack = null;
                }
                attack.Block(false);
            }
        }

    }
}
