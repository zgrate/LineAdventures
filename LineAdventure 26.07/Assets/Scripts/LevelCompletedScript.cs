using UnityEngine;
using System.Collections;

public class LevelCompletedScript : MonoBehaviour {

    // Use this for initialization
    public static LvLStatus lvl;
    public TextMesh levelCompletedMesh, scoreMesh, timeMesh;
	void Start () {
        if (lvl == null)
            return;
        levelCompletedMesh.text = "LEVEL " + lvl.level.levelNumber + " COMPLETED!";
        timeMesh.text = "Time: " + format(lvl.getCurrentTime());
        scoreMesh.text = "Score: " + getScore(lvl);
        LevelManager.LevelCompleted(lvl.level.levelNumber);
        Destroy(lvl);
        lvl = null;

    }
    public static int getScore(LvLStatus status)
    {
        Level level = status.getLevel();
        float scoreMultiplayer = ((float)level.maxTimeSeconds*1000) / ((float)status.getCurrentTime());
        return (int)(scoreMultiplayer*1000);
    }
	
    

    private string format(long miliseconds)
    {
        long seconds = miliseconds / 1000;
        long mili = miliseconds - (seconds * 1000);
        long minutes = seconds / 60;
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
