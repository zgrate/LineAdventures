using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomLevel {

    private ulong Seed;
    private int EnemiesCount;
    private RandomScript.Difficulty Diff;

    public RandomLevel(ulong Seed, int Enemies, RandomScript.Difficulty Diff)
    {
        this.Seed = Seed;
        this.EnemiesCount = Enemies;
        this.Diff = Diff;
    }

    private float _time;

    private List<GameObject> _enemies;

    private int _linelenght;

    private float _health;


    public int LineLenght
    {
        get
        {
            return _linelenght;
        }
    }

    public float Health
    {
        get
        {
            return _health;
        }
    }

    public float TargetTime
    {
        get
        {
            return _time;
        }
    }

    public List<GameObject> Enemies
    {
        get
        {
            return _enemies;
        }
    }

    private void GenerateLenght(RandomScript.Difficulty diff)
    {
        float longest = 0;

        foreach(GameObject go  in Enemies)
        {
            if(go.GetComponent<Enemy>().Lenght > longest)
            {
                longest = go.GetComponent<Enemy>().Lenght;
            }
        }
        Debug.Log("Longest enemy has " + longest);
        if(diff == RandomScript.Difficulty.EASY)
        {
            longest *= NextFloat(1.6f, 2f);
        }
        else if(diff == RandomScript.Difficulty.NORMAL)
        {
            longest *= NextFloat(1.4f, 1.6f);
        }
        else
        {
            longest *= NextFloat(1.2f, 1.4f);
        }
        _linelenght = (int)longest;
    }

    private void GenerateHealth(RandomScript.Difficulty Diff)
    {
        float Sum = 0, TheBiggest = 0;
        foreach(GameObject e in Enemies)
        {
            float hi = e.GetComponent<AttackManager>().HitHPLose;
            Sum += hi;
            if(hi > TheBiggest)
            {
                TheBiggest = hi;
            }
        }
        if(Diff == RandomScript.Difficulty.EASY)
        {
            _health = Sum * NextFloat(1.8f, 2.4f);
        }
        else if(Diff == RandomScript.Difficulty.NORMAL)
        {
            _health = Sum * NextFloat(1f, 1.2f);
        }
        else
        {
            _health = TheBiggest * NextFloat(1f, 2f);
        }
    }

    public void GenerateAllVariables()
    {
        switch(Diff)
        {
            case RandomScript.Difficulty.EASY:
                {
                    _time = EnemiesCount * NextInt(30, 60) * NextFloat(1, 1.5f);
                    for(int i = 0; i < EnemiesCount; i++)
                    {
                        _enemies.Add(DataPersistant.Instance.EasyEnemies[NextInt(0, DataPersistant.Instance.EasyEnemies.Count)]);
                    }
                    GenerateLenght(Diff);
                    GenerateHealth(Diff);
                    break; 
                }
            case RandomScript.Difficulty.NORMAL:
                {
                    _time = EnemiesCount * NextInt(20, 40);
                    for (int i = 0; i < EnemiesCount; i++)
                    {
                        _enemies.Add(DataPersistant.Instance.NormalEnemies[NextInt(0, DataPersistant.Instance.NormalEnemies.Count)]);
                    }
                    GenerateLenght(Diff);
                    GenerateHealth(Diff);
                    break;
                }
            case RandomScript.Difficulty.HARD:
                {
                    _time = EnemiesCount * NextInt(10, 20) * NextFloat(0.8f, 1f);
                    for (int i = 0; i < EnemiesCount; i++)
                    {
                        _enemies.Add(DataPersistant.Instance.HardEnemies[NextInt(0, DataPersistant.Instance.HardEnemies.Count)]);
                    }
                    GenerateLenght(Diff);
                    GenerateHealth(Diff);
                    break;
                }
        }
    }

    public int NextInt(int min, int max)
    {
        Random.seed = (int)Seed;
        return Random.Range(min, max);
    }

    public float NextFloat(float min, float max)
    {
        Random.seed = (int)Seed;
        return Random.Range(min, max);
    }

}
