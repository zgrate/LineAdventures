using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LvLStatus : MonoBehaviour
{

    private List<Enemy> enemies;
    private int[] times;
    public LevelsManager.LevelData level;
    long StarttimeMilisecs;
    long currentTime;
    public TextMesh text;

    public LevelsManager.LevelData getLevel()
    {
        return level;
    }

    private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static long CurrentTimeMillis()
    {
        return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
    }
    
    public void StartTimer()
    {
        StarttimeMilisecs = CurrentTimeMillis();
    }

    void Start()
    {
        enemies = new List<Enemy>(getEnemies());
        times = new int[enemies.Count];
        //StarttimeMilisecs = CurrentTimeMillis();
    }
    void FixedUpdate()
    {
        if (text != null && StarttimeMilisecs != 0)
        {
            currentTime = CurrentTimeMillis() - StarttimeMilisecs;
            text.text = "Time: " + format(currentTime);
        }
    }

    public long getCurrentTime()
    {
        return currentTime;
    }
    private string format(long miliseconds)
    {
        long seconds = miliseconds / 1000;
        long mili = miliseconds - (seconds * 1000);
        long minutes = seconds / 60;
        seconds -= minutes * 60;
        string min = minutes + "";
        if(minutes < 10)
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
    public Enemy[] getEnemies()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Enemy");
        List<Enemy> enemies = new List<Enemy>();
        for(int i = 0; i < objects.Length; i++)
        {
            if (objects[i].activeSelf)
            {
                enemies.Add(objects[i].GetComponent<Enemy>());
            }
        }
        return enemies.ToArray();
    }


    public void CircledEnemy(Enemy enemy)
    {
        int pos = enemies.IndexOf(enemy);
        times[pos]++;
        enemy.Circeled(times[pos]);
        if (times[pos] >= enemy.timesToCircle)
        {
            enemy.finished();
            enemy.gameObject.SetActive(false);
        }
        if(areAllDisabled())
        {
            LvLCompleted();
        }
    }
    public void LvLCompleted()
    {
        if(gameObject.GetComponent<RandomLevelGenerator>() != null)
        {
            DataPersistant.Instance.SetData("lvlcompleted", "randomlevel");
            DataPersistant.Instance.SetData("randomdata", gameObject.GetComponent<RandomLevelGenerator>().Data);
            DataPersistant.Instance.SetData("lvlstatus", this);
            SceneSwitcher.switchScene("LVL COMPLETED");
        }
        //TODO
    }

    public bool areAllDisabled()
    {
        foreach(Enemy enemy in enemies)
        {
            if (enemy.gameObject.activeSelf)
                return false;
        }
        return true; 
    }

    public void hitted(Enemy obje)
    {
        reset();
    }
    public void reset()
    {
        for (int i = 0; i < times.Length; i++)
        {
            if (enemies[i].gameObject.activeSelf)
            {
                times[i] = 0;
                enemies[i].cancel();
            }
        }
    }
}
