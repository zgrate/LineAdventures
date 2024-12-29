using UnityEngine;
using System.Collections;
using UnityEditor;

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
    int count = 1;
    InteractivityScript.Action action = InteractivityScript.Action.IDLE;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        helper = (InteractiveScriptHelper)target;
        b = GUILayout.Toggle(b, "Czesc");
        count = EditorGUILayout.IntField(count);
        Debug.Log(helper.operations.Count + " " + count);
        if(helper.operations.Count > count)
        {
            helper.operations.RemoveRange(count, helper.operations.Count - 1);
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
                if(!operation.actionData.ContainsKey(InteractivityScript.CONST.MOVE_DATA_CONTAINER_SPEED))
                {
                    operation.actionData[InteractivityScript.CONST.MOVE_DATA_CONTAINER_SPEED] = 0.1f;
                 }
                operation.actionData[InteractivityScript.CONST.MOVE_DATA_CONTAINER_SPEED] = EditorGUILayout.FloatField("Speed:",(float)operation.actionData[InteractivityScript.CONST.MOVE_DATA_CONTAINER_SPEED]);
            }
            GUILayout.EndVertical();
            GUILayout.Space(10);
        }
        //action = (InteractivityScript.Action)EditorGUILayout.EnumPopup(action);
        if (GUILayout.Button("TEST"))
        {
            Debug.Log("YES!");
        }
    }
}
