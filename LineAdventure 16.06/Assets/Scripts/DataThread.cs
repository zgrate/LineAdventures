using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System;

public static class DataThread {

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

    public static object GetData(MemoryStream stream)
    {
        BinaryFormatter reader = new BinaryFormatter();
        object obj = reader.Deserialize(stream);
        stream.Close();
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
            if(e.Key.Equals("version"))
            {
                continue;
            }
            Debug.Log(e.Key.GetType().FullName);
            Debug.Log("Value is  " + e.Key);
            int i = (Int32)e.Key;
            bool b = (bool)e.Value;
            list[i] =  b;
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

    public static Hashtable convertHashtableToMyVectorWithClone(this Hashtable table)
    {
        Hashtable hash = new Hashtable();

        foreach (DictionaryEntry e in table)
        {
            //Debug.Log(e.Value.GetType() + " " + (e.Value.GetType() == typeof(Vector2)) + " " + (e.Value.GetType() == typeof(List<Hashtable>)));
            if (e.Value.GetType() == typeof(Vector2) || e.Value.GetType() == typeof(Vector3))
            {
                hash[e.Key] = ((Vector2)e.Value).ToMyVector();
            }
            else if (e.Value.GetType() == typeof(Hashtable))
            {
                hash[e.Key] = convertHashtableToMyVectorWithClone((Hashtable)e.Value);
            }
            else if(e.Value.GetType() == typeof(List<Hashtable>))
            {
                Debug.Log("Start of List");
                List<Hashtable> list = new List<Hashtable>();
                foreach(Hashtable h in (List<Hashtable>)e.Value)
                {
                    list.Add(convertHashtableToMyVectorWithClone(h));
                }
                hash[e.Key] = list;
                Debug.Log("Edn of llist");
            }
            else
            {
                hash[e.Key] = e.Value;
            }
            //Debug.Log("After transform " + hash[e.Key].GetType());
        }
        return hash;
    }
    public static Hashtable converHashtableToVector2WithClone(this Hashtable table)
    {
        Hashtable clone = new Hashtable();
        foreach (DictionaryEntry e in table)
        {
            if (e.Value.GetType() == typeof(MyVector))// || e.Value.GetType() == typeof(Vector3))
            {
                clone[e.Key] = ((MyVector)e.Value).toVector2();//ToSerializable();
            }
            else if (e.Value.GetType() == typeof(Hashtable))
            {
                clone[e.Key] = converHashtableToVector2WithClone((Hashtable)e.Value);
            }
            else if (e.Value.GetType() == typeof(List<Hashtable>))
            {
                List<Hashtable> list = new List<Hashtable>();
                foreach (Hashtable h in (List<Hashtable>)e.Value)
                {
                    list.Add(converHashtableToVector2WithClone(h));
                }
                clone[e.Key] = list;
            }
            else
            {
                clone[e.Key] = e.Value;
            }
        }
        return clone;
    }
    
}
