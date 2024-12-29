using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class RandomPreset : MonoBehaviour
{

    [Serializable]
    public class EnemyPreset
    {
        public Vector2[] Vertices;
        public int TimesToCircle;
        public Material Material;
    }

    public EnemyPreset Enemy;
    public AttackManagerPreset AttackManager;
    public iTweenImplementationPreset iTweenImplementation;

    [Serializable]
    public class iTweenImplementationPreset
    {
        public iTweenImplementation.MoveData[] MoveDatas;
    }

    [Serializable]
    public class AttackManagerPreset
    {
        public bool ResetOnLineBreak;
        [Header("AfterMoveAttack Properties")]
        [Tooltip("Attack executed after all movement proggress")]
        public AttackManager.AttackData AfterMoveAttack;
        [Tooltip("If > 1, attack will be executed after given move loops, if 0 - attack will be executed every loop, if < 0, loop methods will be random, and f.e. -10 means execute time Range(0, 10)")]
        public int AttackExecuteTime;
        public bool ContinueMovementAfterMoveAttack;
        public float HitHPLose;

        [Header("ProximetryAttack Properties")]
        [Tooltip("Attack executed if line will be detected near vertex")]
        public AttackManager.AttackData ProximetryAttack;
        [Tooltip("Range of detection")]
        public float Range;
        public float WaitTime;
        public bool ContinueMovementAfterProximetryAttack;
        public float HPLoseOnProximetry;

    }

    public GameObject EnemyObject;

    public void GeddanData()
    {
        if (EnemyObject == null)
            return;
        Enemy.Material = EnemyObject.GetComponent<MeshRenderer>().sharedMaterial;
        Enemy.Vertices = EnemyObject.GetComponent<Enemy>().startVertexes;
        Enemy.TimesToCircle = EnemyObject.GetComponent<Enemy>().timesToCircle;

        EnemyObject.GetComponent<AttackManager>().PutData(this);
        EnemyObject.GetComponent<iTweenImplementation>().PutData(this);

    }
}

