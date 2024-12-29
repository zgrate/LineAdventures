using UnityEditor;
using UnityEngine;



[CustomEditor(typeof(Enemy))]
[CanEditMultipleObjects]
public class Editor3 : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if(GUILayout.Button("GENERATE MESH"))
            ((Enemy)target).gameObject.GetComponent<MeshFilter>().mesh = ((Enemy)target).startVertexes.GenerateMesh();
    } 
}

[CustomEditor(typeof(RandomPreset))]
[CanEditMultipleObjects]
public class Editor4 : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if(GUILayout.Button("Geddan"))
        {
            ((RandomPreset)target).GeddanData();
        }
    }
}