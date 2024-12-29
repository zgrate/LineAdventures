using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
[ExecuteInEditMode]
public class AttackerScriptHelper : MonoBehaviour {

    Hashtable data = new Hashtable();
    public GameObject GameObject;
    public List<AttackData> attackdatas = new List<AttackData>();
    public List<Vector2> list = new List<Vector2>();
    public void Start()
    {
        attackdatas.Clear();
    }
    public void OnDrawGizmos()
    {
        if (GameObject != null)
        {
            foreach (AttackData data in attackdatas)
            {
                if (data.type == AttackerScript.AttackType.VERTEX_SHOOTER)
                {
                    List<Hashtable> list = (List<Hashtable>)data.data[AttackerScript.CONST.VS_DATA_VERTEX];
                    foreach (Hashtable hash in list)
                    {
                        Gizmos.DrawLine(GameObject.transform.TransformPoint((Vector2)hash[AttackerScript.CONST.VS_DATA_START_VERTEX]), GameObject.transform.TransformPoint((Vector2)hash[AttackerScript.CONST.VS_DATA_END_VERTEX]));
                    }
                }
            }
            foreach(Vector2 v in list)
            {
                Gizmos.DrawCube(GameObject.transform.TransformPoint(v), new Vector2(5,5));
            }
        }
    }
    public Hashtable GetHashtable()
    {
        Hashtable hashtable = new Hashtable();
        data.Add(AttackerScript.CONST.DATA_COUNT, attackdatas.Count);
        hashtable.Add(AttackerScript.CONST.DATA_DATA, data);
        for (int i = 0; i < attackdatas.Count; i++)
        {
            hashtable[i] = attackdatas[i].getSerialized();
        }
        return hashtable;
    }
    public int addNewOperation(AttackerScript.AttackType action)
    {
        AttackData op;
        attackdatas.Add(op = new AttackData(action));
        return attackdatas.IndexOf(op);
    }
    [Serializable]
    public class AttackData
    {
        public AttackerScript.AttackType type;
        public Hashtable data = new Hashtable();

        public Hashtable getSerialized()
        {
            data[AttackerScript.CONST.DATA_ATTACK_TYPE] = type;
            return data;
        }
        
        public AttackData(AttackerScript.AttackType type)
        {
            this.type = type;
        }
    }
}
