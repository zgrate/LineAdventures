using UnityEngine;
using System.Collections;

public class AttackManager : MonoBehaviour {

    public class AttackData
    {

    }

    [Header("AfterMoveAttack Properties")]
    [Tooltip("Attack executed after all movement proggress")]
    public AttackData AfterMoveAttack;
    [Tooltip("If > 1, attack will be executed after given move loops, if 0 - attack will be executed every loop, if < 0, loop methods will be random.")]
    public int AttackExecuteTime;

    [Header("ProximetryAttack Properties")]
    [Tooltip("Attack executed if line will be detected near vertex")]
    public AttackData ProximetryAttack;
    [Tooltip("Range of detection")]
    public float Range;

    public AttackData CircledAttack;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
