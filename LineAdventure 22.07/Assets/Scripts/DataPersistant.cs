using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
[ExecuteInEditMode]
public class DataPersistant : MonoBehaviour {

    [System.NonSerialized]
    public static DataPersistant Instance;

    // Use this for initialization
    void Start() {
        Debug.Log("START");
        if (Instance == null)
        {
            Debug.Log("Setting instance");
            DontDestroyOnLoad(gameObject);
            Instance = this;
            Load();
        }
    }

    public List<GameObject> EasyEnemies;
    public List<GameObject> NormalEnemies;
    public List<GameObject> HardEnemies;



    private Hashtable DataContainer = new Hashtable();

    public object GetData(object key)
    {
        if (!DataContainer.ContainsKey(key))
            return null;
        return DataContainer[key];
    }
    public object SetData(object key, object value)
    {
        object obj = GetData(key);
        DataContainer[key] = value;
        return obj;
    }

    public PlayerData Playerdata;

    [System.Serializable]
    public class PlayerData
    {
        [HideInInspector]
        public List<int> CompletedLevels = new List<int>();
    }

    public int TheBiggestNumber
    {
        get
        {
            int i = 0;
            foreach(int a in Playerdata.CompletedLevels)
            {
                if (a > i)
                    i = a;
            }
            return i;
        }
    }

    public void SAVE()
    {
        //DataThread.SaveDataUsingXML(Playerdata, Application.persistentDataPath, "playerdata.yml");
    }

    public void Load()
    {
        
    }
    public void OnApplicationQuit()
    {
        SAVE();
    }
    public void OnApplicationPause()
    {
        SAVE();
    }

}
