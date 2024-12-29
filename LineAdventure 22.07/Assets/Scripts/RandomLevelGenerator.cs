using UnityEngine;
using System.Collections;

public class RandomLevelGenerator : MonoBehaviour {

    public RandomLevel Data;
    public MainGameScript script;
    public HealthScript health;
	// Use this for initialization
	void Start () {
        Data = (RandomLevel)DataPersistant.Instance.GetData("randomsettings");
        Data.GenerateAllVariables();
        script.maxLength = Data.LineLenght;
        health.MaxHP = Data.Health;

	}
	
   
	// Update is called once per frame
	void Update () {
	
	}
}
