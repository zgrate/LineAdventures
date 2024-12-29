#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
[ExecuteInEditMode]
public class EditorScript1 : MonoBehaviour
{
    public Hashtable h;
    public GameObject obj;
    
    public void simulateMovement(GameObject obj, Hashtable hash)
    {
        if (h != null)
            return;
        h = hash;
        this.obj = obj;
        startPrepare3();
    }
    public void Update()
    {
        if(!line.Equals(rendering))
        {
            rendering = line;
        }
        if(h != null)
            if (AttackerScript.VertexShooterAttack.isOutsideScreen(rendering.transform.TransformPoint(obj.transform.TransformPoint((Vector2)h[AttackerScript.CONST.VS_DATA_END_VERTEX]))))
            {
                rendering.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                rendering.gameObject.transform.position = Vector2.zero;
                Debug.Log("END");
                h = null;
                obj = null;
            }
    }
    
    private static LineRenderer rendering;
    public LineRenderer line;

    public void startPrepare3()
    {
        Vector2 start = (Vector2)h[AttackerScript.CONST.VS_DATA_START_VERTEX];
        Vector2 end = (Vector2)h[AttackerScript.CONST.VS_DATA_END_VERTEX];
        rendering.SetVertexCount(2);
        rendering.SetPositions(new Vector3[] { obj.transform.TransformPoint(start), obj.transform.TransformPoint(end) });
        float speed = (float)h[AttackerScript.CONST.VS_DATA_VELOCITY_ADDITION];
        //float xend = (end.x - start.x).specialAdd(speed);
        //y2 = x2*y1/x1
        //float yend = (end.x*start.y)/start.x;   
        float xend = (end.x - start.x) * speed;
        float yend = (end.y - start.y) * speed;
        Debug.Log(xend + " " + yend);
        Vector2 test = new Vector2(xend, yend);
        rendering.gameObject.GetComponent<Rigidbody2D>().velocity = test;
    }
}


public class EditorSctript1 : EditorWindow {
    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

    [MenuItem("Window/My Window")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(EditorSctript1));
    }
    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        myString = EditorGUILayout.TextField("Text Field", myString);
        if(GUILayout.Button("EXECUTE"))
        {
            Debug.Log("SIEMA");
        }
        groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        myBool = EditorGUILayout.Toggle("Toggle", myBool);
        myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup();
    }
}


[CustomEditor(typeof(InteractiveScriptHelper))]
public class InteractivityScriptCreator : Editor
{
    InteractiveScriptHelper helper;
    bool b = true;
    int count = 0;
    InteractivityScript.Action action = InteractivityScript.Action.IDLE;
    string name = "Siema";
    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();
        helper = (InteractiveScriptHelper)target;
        b = GUILayout.Toggle(b, "Czesc");
        count = EditorGUILayout.IntField(count);
       // Debug.Log(helper.operations.Count + " " + count);
        if(helper.operations.Count > count)
        {
            do
            {
                helper.operations.RemoveAt(helper.operations.Count - 1);
            } while (helper.operations.Count != count);
        }
        else if(helper.operations.Count < count)
        {
            for(int i = helper.operations.Count-1; i < count; i++)
            {
                helper.addNewOperation(InteractivityScript.Action.IDLE);
            }
        }
        
        GUILayout.Space(10);
        GUILayout.Label("Instances: ");
        for(int i = 0; i < count; i++)
        {
            GUILayout.BeginVertical();
            InteractiveScriptHelper.Operation operation = helper.operations[i];
            operation.action = (InteractivityScript.Action)EditorGUILayout.EnumPopup("Type: ", operation.action);
            if(operation.action == InteractivityScript.Action.MOVEMENT)
            {
             //   Debug.Log(operation.actionData);
                if(!operation.actionData.ContainsKey(InteractivityScript.CONST.MOVE_DATA_CONTAINER_SPEED))
                {
                    operation.actionData[InteractivityScript.CONST.MOVE_DATA_CONTAINER_SPEED] = 0.1f;
                 }
                operation.actionData[InteractivityScript.CONST.MOVE_DATA_CONTAINER_SPEED] = EditorGUILayout.FloatField("Speed:", (float)operation.actionData[InteractivityScript.CONST.MOVE_DATA_CONTAINER_SPEED]);
            }
            else if(operation.action == InteractivityScript.Action.STOP)
            {
                if (!operation.actionData.ContainsKey(InteractivityScript.CONST.STOP_DATA_TIME))
                {
                    operation.actionData[InteractivityScript.CONST.STOP_DATA_TIME] = 5f;
                }
                operation.actionData[InteractivityScript.CONST.STOP_DATA_TIME] = EditorGUILayout.FloatField("Time to stop:", (float)operation.actionData[InteractivityScript.CONST.STOP_DATA_TIME]);
            }
            else if (operation.action == InteractivityScript.Action.ROTATION_ANGLE)
            {
                if (!operation.actionData.ContainsKey(InteractivityScript.CONST.ROTATION_ANGLE_DATA_SPEED))
                {
                    operation.actionData[InteractivityScript.CONST.ROTATION_ANGLE_DATA_SPEED] = 1f;
                }
                operation.actionData[InteractivityScript.CONST.ROTATION_ANGLE_DATA_SPEED] = EditorGUILayout.FloatField("Rotation speed:", (float)operation.actionData[InteractivityScript.CONST.ROTATION_ANGLE_DATA_SPEED]);
            }
            else if (operation.action == InteractivityScript.Action.ROTATION_TIME)
            {
                if (!operation.actionData.ContainsKey(InteractivityScript.CONST.ROTATION_TIME_DATA_SPEED))
                {
                    operation.actionData[InteractivityScript.CONST.ROTATION_TIME_DATA_SPEED] = 0f;
                }
                if (!operation.actionData.ContainsKey(InteractivityScript.CONST.ROTATION_TIME_DATA_TIME))
                {
                    operation.actionData[InteractivityScript.CONST.ROTATION_TIME_DATA_TIME] = 10f;
                }
                operation.actionData[InteractivityScript.CONST.ROTATION_TIME_DATA_SPEED] = EditorGUILayout.FloatField("Rotation speed:", (float)operation.actionData[InteractivityScript.CONST.ROTATION_TIME_DATA_SPEED]);
                operation.actionData[InteractivityScript.CONST.ROTATION_TIME_DATA_TIME] = EditorGUILayout.FloatField("Rotation time:", (float)operation.actionData[InteractivityScript.CONST.ROTATION_TIME_DATA_TIME]);

            }
            else if (operation.action == InteractivityScript.Action.RANDOM_MOVE_ROTATE)
            {
                if (!operation.actionData.ContainsKey(InteractivityScript.CONST.RANDOM_MANDR_DATA_MOVE_SPEED))
                {
                    operation.actionData[InteractivityScript.CONST.RANDOM_MANDR_DATA_MOVE_SPEED] = 0.1f;
                }
                if (!operation.actionData.ContainsKey(InteractivityScript.CONST.RANDOM_MANDR_DATA_ROTATION_SPEED))
                {
                    operation.actionData[InteractivityScript.CONST.RANDOM_MANDR_DATA_ROTATION_SPEED] = 3f;
                }
                if (!operation.actionData.ContainsKey(InteractivityScript.CONST.RANDOM_MANDR_DATA_ROTATION_CHANCE))
                {
                    operation.actionData[InteractivityScript.CONST.RANDOM_MANDR_DATA_ROTATION_CHANCE] = 0.5f;
                }
                operation.actionData[InteractivityScript.CONST.RANDOM_MANDR_DATA_MOVE_SPEED] = EditorGUILayout.FloatField("Move speed:", (float)operation.actionData[InteractivityScript.CONST.RANDOM_MANDR_DATA_MOVE_SPEED]);
                operation.actionData[InteractivityScript.CONST.RANDOM_MANDR_DATA_ROTATION_SPEED] = EditorGUILayout.FloatField("Rotation speed:", (float)operation.actionData[InteractivityScript.CONST.RANDOM_MANDR_DATA_ROTATION_SPEED]);
                operation.actionData[InteractivityScript.CONST.RANDOM_MANDR_DATA_ROTATION_CHANCE] = EditorGUILayout.FloatField("Rotation chance:", (float)operation.actionData[InteractivityScript.CONST.RANDOM_MANDR_DATA_ROTATION_CHANCE]);

            }
            GUILayout.EndVertical();
            GUILayout.Space(10);

        }
        //action = (InteractivityScript.Action)EditorGUILayout.EnumPopup(action);
        name = EditorGUILayout.TextField("FilePath: ", name);
        if (GUILayout.Button("SAVE"))
        {
            Debug.Log("YES!");
            DataThread.SaveHashTableData(helper.GetHashtable(), name);
        }
    }
}

[CustomEditor(typeof(AttackerScriptHelper))]
public class Editoable : Editor
{
    string location;
    int temp = 0;
    //bool start = false;
    public override void OnInspectorGUI()
    {
        AttackerScriptHelper helper = (AttackerScriptHelper)target;
        // DrawDefaultInspector();
        helper.GameObject = (GameObject)EditorGUILayout.ObjectField("GameObject", helper.GameObject, typeof(GameObject));
        if (helper.GameObject == null)
            return;
        int count = helper.attackdatas.Count;
       // Debug.Log("ADD");
        GUILayout.Space(20);
        count = EditorGUILayout.IntField(count);
        helper.list.Clear();
        //Debug.Log(helper.attackdatas.Count);
        if (helper.attackdatas.Count > count)
        {
            do
            {
                helper.attackdatas.RemoveAt(helper.attackdatas.Count - 1);
            } while (helper.attackdatas.Count != count);
        }
        else if(count == 0)
        {
            helper.attackdatas.Clear();
        }
        else if (helper.attackdatas.Count < count)
        {
            for (int i = helper.attackdatas.Count; i < count; i++)
            {
                helper.addNewOperation(AttackerScript.AttackType.IDLE);
            }
        }

        GUILayout.Space(10);
        for (int i = 0; i < helper.attackdatas.Count; i++)
        {
            GUILayout.BeginVertical();
            AttackerScriptHelper.AttackData attackData = helper.attackdatas[i];
            attackData.type = (AttackerScript.AttackType)EditorGUILayout.EnumPopup("Type: ", attackData.type);
            if (!attackData.data.ContainsKey(AttackerScript.CONST.DATA_EXECUTE_ON_THE_END))
                attackData.data[AttackerScript.CONST.DATA_EXECUTE_ON_THE_END] = true;
            attackData.data[AttackerScript.CONST.DATA_EXECUTE_ON_THE_END] = (bool)EditorGUILayout.Toggle("Execute on the end of script: ", (bool)attackData.data[AttackerScript.CONST.DATA_EXECUTE_ON_THE_END]);

            if (!attackData.data.ContainsKey(AttackerScript.CONST.DATA_REACT_ON_CIRCLE))
                attackData.data[AttackerScript.CONST.DATA_REACT_ON_CIRCLE] = true;
            attackData.data[AttackerScript.CONST.DATA_REACT_ON_CIRCLE] = (bool)EditorGUILayout.Toggle("Execute on circle: ", (bool)attackData.data[AttackerScript.CONST.DATA_REACT_ON_CIRCLE]);

            if (!attackData.data.ContainsKey(AttackerScript.CONST.DATA_REACT_ON_PROXIMETRY))
                attackData.data[AttackerScript.CONST.DATA_REACT_ON_PROXIMETRY] = true;
            attackData.data[AttackerScript.CONST.DATA_REACT_ON_PROXIMETRY] = (bool)EditorGUILayout.Toggle("Execute on proxymentry detection: ", (bool)attackData.data[AttackerScript.CONST.DATA_REACT_ON_PROXIMETRY]);

            if (!attackData.data.ContainsKey(AttackerScript.CONST.DATA_WARNING_TIME))
                attackData.data[AttackerScript.CONST.DATA_WARNING_TIME] = 10f;
            attackData.data[AttackerScript.CONST.DATA_WARNING_TIME] = (float)EditorGUILayout.FloatField("Seconds of warning: ", (float)attackData.data[AttackerScript.CONST.DATA_WARNING_TIME]);

            if (!attackData.data.ContainsKey(AttackerScript.CONST.DATA_PROXIMETRY_DETECTION_RADIUS))
                attackData.data[AttackerScript.CONST.DATA_PROXIMETRY_DETECTION_RADIUS] = 10f;
            attackData.data[AttackerScript.CONST.DATA_PROXIMETRY_DETECTION_RADIUS] = (float)EditorGUILayout.FloatField("Proximetry Detection Range: ", (float)attackData.data[AttackerScript.CONST.DATA_PROXIMETRY_DETECTION_RADIUS]);

            if (!attackData.data.ContainsKey(AttackerScript.CONST.DATA_PROXIMETRY_TIME_OUT))
                attackData.data[AttackerScript.CONST.DATA_PROXIMETRY_TIME_OUT] = 10f;
            attackData.data[AttackerScript.CONST.DATA_PROXIMETRY_TIME_OUT] = (float)EditorGUILayout.FloatField("Proximetry Time Out: ", (float)attackData.data[AttackerScript.CONST.DATA_PROXIMETRY_TIME_OUT]);

            if (!attackData.data.ContainsKey(AttackerScript.CONST.DATA_CIRCLE_TIME_OUT))
                attackData.data[AttackerScript.CONST.DATA_CIRCLE_TIME_OUT] = 10f;
            attackData.data[AttackerScript.CONST.DATA_CIRCLE_TIME_OUT] = (float)EditorGUILayout.FloatField("Circle Time Out: ", (float)attackData.data[AttackerScript.CONST.DATA_CIRCLE_TIME_OUT]);

            if(attackData.type != AttackerScript.AttackType.IDLE)
            {
                GUILayout.Space(5);
                if (attackData.type == AttackerScript.AttackType.VERTEX_SHOOTER)
                {
                    if(!attackData.data.ContainsKey(AttackerScript.CONST.VS_DATA_VERTEX))
                    {
                        attackData.data[AttackerScript.CONST.VS_DATA_VERTEX] = new List<Hashtable>();
                    }
                    GUILayout.Space(2);
                    List<Hashtable> table = (List<Hashtable>)attackData.data[AttackerScript.CONST.VS_DATA_VERTEX];
                    if (GUILayout.Button("ADD VERTEX"))
                    {
                        table.Add(new Hashtable());
                    }
                    // Debug.Log("Oh Yea!");
                    Mesh mesh = helper.GameObject.GetComponent<MeshFilter>().sharedMesh;
                    Vector3[] vectorlist = mesh.vertices;
                    
                    for(int a = 0; a < table.Count; a++)
                    {

                        Hashtable ht = (Hashtable)table[a];
                        if(!ht.ContainsKey(AttackerScript.CONST.VS_DATA_START_VERTEX))
                        {
                            ht[AttackerScript.CONST.VS_DATA_START_VERTEX] = new Vector2(0, 0);
                        }
                        if (!ht.ContainsKey(AttackerScript.CONST.VS_DATA_END_VERTEX))
                        {
                            ht[AttackerScript.CONST.VS_DATA_END_VERTEX] = new Vector2(0, 0);
                        }
                        if (!ht.ContainsKey(AttackerScript.CONST.VS_DATA_VELOCITY_ADDITION))
                        {
                            ht[AttackerScript.CONST.VS_DATA_VELOCITY_ADDITION] = 1f;
                        }
                        
                        GUILayout.BeginHorizontal();
                        temp = EditorGUILayout.Popup(temp, vectorlist.ToStringListAll());
                        if(GUILayout.Button("Click to set start vertex to this"))
                        {
                            ht[AttackerScript.CONST.VS_DATA_START_VERTEX] = (Vector2)vectorlist[temp];
                        }
                        GUILayout.EndHorizontal();
                        ht[AttackerScript.CONST.VS_DATA_START_VERTEX] = ((Vector2)EditorGUILayout.Vector2Field("First Vector", (Vector2)ht[AttackerScript.CONST.VS_DATA_START_VERTEX]));
                        EditorGUILayout.LabelField("Position Point: " + ht[AttackerScript.CONST.VS_DATA_START_VERTEX] + " Distance: " + Vector2.Distance((Vector2)ht[AttackerScript.CONST.VS_DATA_START_VERTEX], (Vector2)ht[AttackerScript.CONST.VS_DATA_END_VERTEX]));
                        ht[AttackerScript.CONST.VS_DATA_END_VERTEX] = ((Vector2)EditorGUILayout.Vector2Field("Second Vector", (Vector2)ht[AttackerScript.CONST.VS_DATA_END_VERTEX]));
                        ht[AttackerScript.CONST.VS_DATA_VELOCITY_ADDITION] = (float)EditorGUILayout.FloatField("How much Velocity add?", (float)ht[AttackerScript.CONST.VS_DATA_VELOCITY_ADDITION]);
                        if(GUILayout.Button("REMOVE VERTEX"))
                        {
                            table.Remove(ht);
                            ht.Clear();
                        }
                        if(GUILayout.Button("Symulate"))
                        {
                            GameObject.Find("AttackMaker").GetComponent<EditorScript1>().simulateMovement(helper.GameObject, ht);
                        }
                        GUILayout.Space(10);
                    }
                    
                }
            }

            GUILayout.EndVertical();
            GUILayout.Space(10);
            
        }
        location = "attackdata";
        location = EditorGUILayout.TextField("Location of Save: ", location);
        if (GUILayout.Button("SAVE"))
        {
            Hashtable tbl = (Hashtable)helper.GetHashtable().Clone();
            //DataThread.convertHashtableToMyVectorWithClone(tbl);
            DataThread.SaveHashTableData(tbl.convertHashtableToMyVectorWithClone(), location);
            Debug.Log("SAVED");
        }
    }


}

[CustomEditor(typeof(AttackerScript))]

public class AttackerScriptEditor : Editor
{
    AttackerScript script;
    private const int DATA_STORAGE = 123, COUNT = 81274;
    public override void OnInspectorGUI()
    {
        script = (AttackerScript)target;
        DrawDefaultInspector();
        SerializedObject obj = serializedObject;

       
        if (script.DataFile == null)
        {
            return;
        }
      //  Debug.Log("Loading...");
        if (!script.tempHashData.ContainsKey("tempTextData") || (script.DataFile != null && (TextAsset)script.tempHashData["tempTextData"] != script.DataFile))
        {
            //Debug.Log("We have to load it");
            script.tempHashData["tempTextData"] = script.DataFile;
            if (script.DataFile != null)
            {
                script.tempHashData[DATA_STORAGE] = ((Hashtable)DataThread.GetData(new MemoryStream(script.DataFile.bytes)));
                script.tempHashData[COUNT] = (int)((Hashtable)((Hashtable)script.tempHashData[DATA_STORAGE])[AttackerScript.CONST.DATA_DATA])[AttackerScript.CONST.DATA_COUNT];


                if (script.tempHashData[DATA_STORAGE] == null)
                    return;
                Hashtable DATA = (Hashtable)script.tempHashData[DATA_STORAGE];
              //  Debug.Log("Calculating..");
                for (int i = 0; i < (int)script.tempHashData[COUNT]; i++)
                {
                    Hashtable table = (Hashtable)DATA[i];
                    if (((AttackerScript.AttackType)table[AttackerScript.CONST.DATA_ATTACK_TYPE]) == AttackerScript.AttackType.VERTEX_SHOOTER)
                    {
                        List<Hashtable> list = (List<Hashtable>)table[AttackerScript.CONST.VS_DATA_VERTEX];
                        // Debug.Log("Oh oh yea... " + list.Count + " " + script.RenderingLinesList.Count);
                        SerializedProperty prop = obj.FindProperty("linerendering");
                        if (prop.arraySize != list.Count)
                        {
                            prop.arraySize = list.Count;
                            serializedObject.ApplyModifiedProperties();
                        }
                    }

                }
            }
        }
        
       
    }
    
}
#endif
/*

                        for(int a = 0; a < table.Count; a++)
                    {
                        Vector2 v2 = mesh.vertices[a];
                        Hashtable ht = (Hashtable)table[a];
                        if(!ht.ContainsKey(AttackerScript.CONST.VERTEX_DATA_VERTEX_MAIN))
                        {
                            ht[AttackerScript.CONST.VERTEX_DATA_VERTEX_MAIN] = v2;
                        }
                        if (!ht.ContainsKey(AttackerScript.CONST.VERTEX_DATA_VERTEX_SECOND))
                        {
                            ht[AttackerScript.CONST.VERTEX_DATA_VERTEX_SECOND] = new Vector2(v2.x, v2.y);
                        }
                        if (!ht.ContainsKey(AttackerScript.CONST.VERTEX_DATA_X_ADDITIONAL))
                        {
                            ht[AttackerScript.CONST.VERTEX_DATA_X_ADDITIONAL] = true;
                        }
                        if(!ht.ContainsKey(AttackerScript.CONST.VS_DATA_SPEED))
                        {
                            ht[AttackerScript.CONST.VS_DATA_SPEED] = 5f;
                        }
                        Vector2 second;
                        EditorGUILayout.LabelField("Position Point: " + v2 + " Distance: " + Vector2.Distance(v2, (Vector2)ht[AttackerScript.CONST.VERTEX_DATA_VERTEX_SECOND]));
                        ht[AttackerScript.CONST.VERTEX_DATA_VERTEX_SECOND] = second = ((Vector2)EditorGUILayout.Vector2Field("Second Vector", (Vector2)ht[AttackerScript.CONST.VERTEX_DATA_VERTEX_SECOND]));
                        AttackerScript.Line line= AttackerScript.Line.getLine(v2, (Vector2)ht[AttackerScript.CONST.VERTEX_DATA_VERTEX_SECOND]);
                        ht[AttackerScript.CONST.VERTEX_DATA_LINE] = line;
                        EditorGUILayout.LabelField("CALCULATED LINE: a: " + line.a + " b: " + line.b);

                        ht[AttackerScript.CONST.VS_DATA_SPEED] = (float)EditorGUILayout.FloatField("Speed of line: ", (float)ht[AttackerScript.CONST.VS_DATA_SPEED]);
                        ht[AttackerScript.CONST.VERTEX_DATA_X_ADDITIONAL] = (bool)GUILayout.Toggle((bool)ht[AttackerScript.CONST.VERTEX_DATA_X_ADDITIONAL], "Add X");
                        
                        float f = Vector2.Distance(v2, (Vector2)ht[AttackerScript.CONST.VERTEX_DATA_VERTEX_SECOND]);
                        float x = (bool)ht[AttackerScript.CONST.VERTEX_DATA_X_ADDITIONAL] ? second.x+f : second.x-f;
                        helper.list.Add(new Vector2(x, line.getY(x)));
                    }
    */
