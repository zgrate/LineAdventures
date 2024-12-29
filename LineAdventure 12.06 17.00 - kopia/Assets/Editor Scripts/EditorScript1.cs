using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class EditorScript1 : MonoBehaviour
{
    public float i;
    
    public void Start()
    {
        object obj = Resources.Load("Assets/Resources/Siema");
        Debug.Log(obj);
    }
    public void Update()
    {
        TextAsset asset = (TextAsset)Resources.Load("AttackDatas/siema", typeof(TextAsset));
        MemoryStream stream = new MemoryStream(asset.bytes);
        Hashtable hash = DataThread.GetData(stream) as Hashtable;
        Debug.Log(hash[0].GetType());
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
                    // Debug.Log("Oh Yea!");
                    Mesh mesh = helper.GameObject.GetComponent<MeshFilter>().mesh;
                    if(table.Count != helper.GameObject.GetComponent<MeshFilter>().mesh.vertexCount)
                    {
                        table.Clear();
                        foreach(Vector2 v2 in mesh.vertices)
                        {
                            table.Add(new Hashtable());
                        }
                    }
                    
                    for(int a = 0; i < table.Count; i++)
                    {
                        Vector2 v2 = mesh.vertices[i];
                        Hashtable ht = (Hashtable)table[i];
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
                    
                }
            }

            GUILayout.EndVertical();
            GUILayout.Space(10);

        }
        location = "attackdata";
        location = EditorGUILayout.TextField("Location of Save: ", location);
        if (GUILayout.Button("SAVE"))
        {
            DataThread.SaveHashTableData(helper.GetHashtable(), location);
            Debug.Log("SAVED");
        }
    }
}

