using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LevelManager : MonoBehaviour {

    public const float LEVELS_FILE_VERSION = 0.1f, DATA_FILE_VERSION = 0.1f;
    public float ReadedLevelsVersion, ReadedDataVersion;
    public static List<bool> completedLevels = null;
    List<Level> list = new List<Level>();
    public static float lineLenght = 1000f;
    void OnGUI()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneSwitcher.switchScene("Menu");
        }
    }
    // Use this for initialization
    void Start () {
        Debug.Log(Application.persistentDataPath + Path.DirectorySeparatorChar + "Levels.data");
        if (list.Count == 0)
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Level"))
            {
                list.Add(obj.GetComponent<Level>());
            }
        }
        if ( completedLevels == null)
        {
            DontDestroyOnLoad(gameObject);
            completedLevels = DataThread.GetCompletedLevels();
         //   Debug.Log(completedLevels);
            if (completedLevels == null)
            {
                completedLevels = new List<bool>(new bool[list.Count+1]);
                DataThread.SaveCompletedLevels(completedLevels, LEVELS_FILE_VERSION);
                Hashtable table = new Hashtable();
                table.Add("version", DATA_FILE_VERSION);
                table.Add("linelenght", 1000f);
                DataThread.SaveHashTableData(table, "Data.data");
                Debug.Log("CREATED ALL");
            }
            else
            {
                Hashtable hash = DataThread.GetHashTableData("Data.data");
                ReadedDataVersion = ((float)hash["version"]);
                ReadedLevelsVersion = DataThread.versionOfCompletedLevels;
                if(ReadedDataVersion != DATA_FILE_VERSION)
                {
                    Debug.Log("DATA VERSION NO MATCH" + ReadedDataVersion + " " + DATA_FILE_VERSION);
                }
                if(ReadedLevelsVersion != LEVELS_FILE_VERSION)
                {
                    Debug.Log("Levels VERSION NO MATCH");

                }
                lineLenght = ((float)hash["linelenght"]);
                Debug.Log(lineLenght + " LINE ");
                Debug.Log("READED ALL");
            }
            //-475
            //-300
        }

    }

    public void Update()
    {
        foreach (Level lvl in list)
        {
            if (completedLevels.Count < lvl.levelNumber)
            {
                completedLevels.Insert(lvl.levelNumber, false);
            }
            if (HasPassedLevel(lvl.levelNumber))
                lvl.setPassedImage();
            else if (lvl.LineLength > lineLenght)
                lvl.SetTooShortLineError();
            else if (canTryLevel(lvl.levelNumber))
                lvl.setNormalImage();
            else
                lvl.setLockedImage();
        }
    }

    public Level getLevelOfNumber(int number)
    {
        foreach(Level l in list)
        {
            if (l.levelNumber == number)
                return l;
        }
        return null;
    }
    
    public static void LevelCompleted(int number)
    {
        completedLevels[number] = true;
        DataThread.SaveCompletedLevels(completedLevels, LEVELS_FILE_VERSION);
    }	

    public static List<bool> GetCompletedList()
    {
        return completedLevels;
    }
    public static bool canTryLevel(int number)
    {
        if (completedLevels.LastIndexOf(true) == -1 && number == 1)
            return true;
        return  number <= completedLevels.LastIndexOf(true) + 1;
    }

    public static bool HasPassedLevel(int number)
    {
        return completedLevels[number];
    }


    public void switchScene(string scene)
    {
        SceneSwitcher.switchScene(scene);
    }


    
}
