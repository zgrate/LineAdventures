using UnityEngine;
using System.Collections;

public class LevelCompletedScript : MonoBehaviour {

    // Use this for initialization
    public TextMesh levelCompletedMesh, scoreMesh, timeMesh;
	void Start () {

        string s = (string)DataPersistant.Instance.GetData("lvlcompleted");
        LvLStatus status = (LvLStatus)DataPersistant.Instance.GetData("lvlstatus");
        if(s.Equals("randomlevel"))
        {
            RandomLevel Data = (RandomLevel)DataPersistant.Instance.GetData("randomdata");
            levelCompletedMesh.text = "LEVEL COMPLETED!";
            timeMesh.text = "Time: " + format(status.getCurrentTime());
            scoreMesh.text = "Score: " + getScore(status.getCurrentTime(), Data.TargetTime);
        }
        /*
        levelCompletedMesh.text = "LEVEL " + lvl.level.levelNumber + " COMPLETED!";
        timeMesh.text = "Time: " + format(lvl.getCurrentTime());
        scoreMesh.text = "Score: " + getScore(lvl);
        //LevelManager.LevelCompleted(lvl.level.levelNumber);
        Destroy(lvl);
        lvl = null;
        */
    }
    public static int getScore(float Time, float MaxTimeSeconds)
    {
        float scoreMultiplayer = ((float)MaxTimeSeconds * 1000) / ((float)Time);
        return (int)(scoreMultiplayer*1000);
    }



    private string format(long miliseconds)
    {
        long seconds = miliseconds / 1000;
        long mili = miliseconds - (seconds * 1000);
        long minutes = seconds / 60;
        seconds -= minutes * 60;
        string min = minutes + "";
        if (minutes < 10)
        {
            min = "0" + min;
        }
        string sec = seconds + "";
        if (seconds < 10)
        {
            sec = "0" + sec;
        }
        string mil = mili + "";
        if (mili < 10)
        {
            mil = "00" + mil;
        }
        else if (mili < 100)
        {
            mil = "0" + mil;
        }

        return min + ":" + sec + ":" + mil;
    }
}
