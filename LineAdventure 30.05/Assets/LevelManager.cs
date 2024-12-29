using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

    public const float LEVELS_FILE_VERSION = 0.1f, DATA_FILE_VERSION = 0.1f;
    public float ReadedLevelsVersion, ReadedDataVersion;
    public static List<bool> completedLevels = null;
    List<Level> list = new List<Level>();
    public static float lineLenght = 1000f;
    // Use this for initialization
    void Start () {
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
                table.Add("linelenght", 1000);
                DataThread.SaveHashTableData(table, "Data.data");
            }
            else
            {
                Hashtable hash = DataThread.GetHashTableData("Data.data");
                lineLenght = ((float)hash["linelenght"]);
                ReadedLevelsVersion = ((float)hash["version"]);
                ReadedLevelsVersion = DataThread.versionOfCompletedLevels;
                if(ReadedDataVersion != DATA_FILE_VERSION)
                {
                    Debug.Log("DATA VERSION NO MATCH" );
                }
                if(ReadedLevelsVersion != LEVELS_FILE_VERSION)
                {
                    Debug.Log("Levels VERSION NO MATCH");

                }
            }
            
        }
        


        foreach (Level lvl in list)
        {

            if (completedLevels.Count < lvl.levelNumber)
            {
                completedLevels.Insert(lvl.levelNumber, false);
            }
            if (HasPassedLevel(lvl.levelNumber))
                lvl.setPassedImage();
            else if (lvl.LineLength < lineLenght)
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

public static class extenstion{

    public static Texture2D AlphaBlend(this Texture2D aBottom, Texture2D aTop)
    {
        
        if (aBottom.width != aTop.width || aBottom.height != aTop.height)
            throw new System.InvalidOperationException("AlphaBlend only works with two equal sized images");
        var bData = aBottom.GetPixels();
        var tData = aTop.GetPixels();
        int count = bData.Length;
        var rData = new Color[count];
        for (int i = 0; i < count; i++)
        {
            Color B = bData[i];
            Color T = tData[i];
            float srcF = T.a;
            float destF = 1f - T.a;
            float alpha = srcF + destF * B.a;
            Color R = (T * srcF + B * B.a * destF) / alpha;
            R.a = alpha;
            rData[i] = R;
        }
        var res = new Texture2D(aTop.width, aTop.height);
        res.SetPixels(rData);
        res.Apply();
        return res;
    }
}