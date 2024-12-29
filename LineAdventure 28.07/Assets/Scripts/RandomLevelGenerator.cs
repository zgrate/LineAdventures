using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomLevelGenerator : MonoBehaviour {

    public RandomLevel Data;
    public MainGameScript script;
    public HealthScript health;
    public LvLStatus Parent;
    private List<GameObject> InstancedEnemies = new List<GameObject>();


	// Use this for initialization
	void Start () {
        Time.timeScale = 0;
        Data = (RandomLevel)DataPersistant.Instance.GetData("randomsettings");     
        Data.GenerateEnemiesAndTime();
        foreach(GameObject obj in Data.EnemiesRandomPresets)
        {
            RandomPreset preset = obj.GetComponent<RandomPreset>();
            InstancedEnemies.Add(GenerateEnemy(preset));
        }
        Data.GenerateHealth(InstancedEnemies);
        Data.GenerateLenght(InstancedEnemies);
        script.maxLength = Data.LineLenght;
        health.MaxHP = Data.Health;
        
        //foreach(GameObject e in Data.Enemies)
        //{
        //    GameObject obj = Instantiate(e);
        //    PrepareEnemy(obj);
        //    InstancedEnemies.Add(obj);
        //}
	}

    private GameObject GenerateEnemy(RandomPreset Preset)
    {
        GameObject Enemy = new GameObject();
        Enemy.tag = "Enemy";
        Enemy.transform.position = Camera.main.ScreenToWorldPoint(new Vector2(Data.NextInt(0, Screen.width), Data.NextInt(0, Screen.height)));
        Enemy.SetActive(false);
        //GENERATING VIEW
        MeshFilter filter = Enemy.AddComponent<MeshFilter>();
        MeshRenderer renderer = Enemy.AddComponent<MeshRenderer>();
        renderer.material = Preset.Enemy.Material;
        filter.mesh = Preset.Enemy.Vertices.GenerateMesh();
        //GENERATING ENEMY
        Enemy e = Enemy.AddComponent<Enemy>();
        e.startVertexes = Preset.Enemy.Vertices;
        e.timesToCircle = Preset.Enemy.TimesToCircle;
        //GENERATING ATTACK
        AttackManager mng = Enemy.AddComponent<AttackManager>();
        mng.GetData(Preset);
        mng.MainGameScriptObject = script;
        //GENERATING MOVEMENT
        iTweenImplementation impl = Enemy.AddComponent<iTweenImplementation>();
        mng.GetData(Preset);
        return Enemy;
    }
    
	
    private void PrepareEnemy(GameObject game)
    {
        game.transform.parent = Parent.transform;
        game.tag = "Enemy";
        game.transform.position = (Camera.main.ScreenToWorldPoint(new Vector3(Data.NextFloat(0, Screen.width), Data.NextFloat(0, Screen.height))));
        game.SetActive(true);
        game.GetComponent<AttackManager>().MainGameScriptObject = script;
    }
   
	// Update is called once per frame
	void Update () {
	
	}
}
