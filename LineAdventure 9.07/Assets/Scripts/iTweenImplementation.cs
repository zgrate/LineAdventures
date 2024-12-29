using UnityEngine;
using System.Collections;
using System;

public class iTweenImplementation : MonoBehaviour {

    public enum ExecuteMode
    {
        TYPICAL, ON_PROXIMETRY, AFTER_BREAK
    }
    public enum ActionMode
    {
        EXECUTE_MOVEMENT, EXECUTE_ATTACK
    }

    [Serializable]
    public struct MoveData
    {
        public string iTweenPath;
        public float Speed;

        public string CompleteAttackMethod;
        public GameObject Reference;

        public ExecuteMode ExecuteMode;
        public ActionMode ActionMode;
        public bool LocalSpace;

        public bool SimulateWorld;
        public bool RandomSimulation;
        public string iTweenPathToSimulate;
        public float SpeedOfSimulation;
    }

    public MoveData[] moveData;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
