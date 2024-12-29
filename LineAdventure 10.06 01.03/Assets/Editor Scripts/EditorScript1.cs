using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

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
