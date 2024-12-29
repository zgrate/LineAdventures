using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public class DataThread {

    /*
    public static void SaveData(object obj, string directory, string FileString)
    {
        if (Directory.Exists(directory));
            Directory.CreateDirectory(directory);
        FileStream file = File.Create(FileString);
        BinaryWriter writer = new BinaryWriter(file);
        writer.Write(JsonUtility.ToJson(obj));
        file.Close();
    }

    public static object GetData(string fileString)
    {
        if (!Exists(fileString))
            return null;
        FileStream file = File.Create(fileString);
        BinaryReader reader = new BinaryReader(file);
        string s = reader.ReadString();
        object obj = JsonUtility.FromJson<object[]>(s);
        file.Close();
        return obj;
    }
    */
    public static void SaveData(object obj, string directory, string FileString)
    {
        if (Directory.Exists(directory)) ;
        Directory.CreateDirectory(directory);
        FileStream file = File.Create(FileString);
        BinaryFormatter writer = new BinaryFormatter();
        writer.Serialize(file, obj);
        file.Close();
    }

    public static object GetData(string fileString)
    {
        if (!Exists(fileString))
            return null;
        FileStream file = File.Open(fileString, FileMode.Open);
        BinaryFormatter reader = new BinaryFormatter();
        object obj = reader.Deserialize(file);
        file.Close();
        return obj;
    }

    public static bool Exists(string fileString)
    {
        return File.Exists(fileString);
    }
    public static void DeleteFile(string FileS)
    {
        File.Delete(FileS);
    }
    public static float versionOfCompletedLevels = 0.0f;
    public static List<bool> GetCompletedLevels()
    {
        Hashtable hash = (GetData(Application.persistentDataPath + Path.DirectorySeparatorChar + "Levels.data") as Hashtable);
        if (hash == null)
            return null;
        versionOfCompletedLevels = ((float)hash["version"]);
        Debug.Log(hash.Count);
        List<bool> list = new List<bool>(new bool[hash.Count+1]);
        foreach(DictionaryEntry e in hash)
        {
            list[(int)e.Key] =  (bool)e.Value;
        }
        return list;
    }

    public static void SaveCompletedLevels(List<bool> list, float version)
    {
        Hashtable hash = new Hashtable();
        hash.Add("version", version);
        for(int i = 0; i < list.Count; i++)
        {
            hash.Add(i, list[i]);
        }
        SaveData(hash, Application.persistentDataPath, Application.persistentDataPath + Path.DirectorySeparatorChar + "Levels.data");
    }
    public static void SaveHashTableData(Hashtable table, string pos)
    {
        SaveData(table, Application.persistentDataPath, Application.persistentDataPath + Path.DirectorySeparatorChar + pos);
    }
    public static Hashtable GetHashTableData(string pos)
    {
        return GetData(Application.persistentDataPath + Path.DirectorySeparatorChar + pos) as Hashtable;
    }
}
