using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class InteractiveScriptHelper : MonoBehaviour{

    public List<Operation> operations = new List<Operation>();
    private Hashtable hash = new Hashtable();
    private int i = 0;

    public Operation getOperation(int i )
    {
        return operations[i];
    }

    public int addNewOperation(InteractivityScript.Action action)
    {
        Operation op;
        operations.Add(op = new Operation(action));
        return operations.IndexOf(op);
    }

    public Hashtable GetHashtable()
    {
        Hashtable hashtable = new Hashtable();
        hash.Add(InteractivityScript.CONST.DATA_COUNT, operations.Count);
        hashtable.Add(InteractivityScript.CONST.DATA_DATA, hash);
        for(int i = 0; i < operations.Count; i++)
        {
            hashtable[i] = operations[i].getSerialized();
        }
        return hashtable;
    }
    public static InteractiveScriptHelper createScriptData()
    {
        return new InteractiveScriptHelper();
    }

    [Serializable]
    public class Operation
    {
        public Operation(InteractivityScript.Action action)
        {
            this.action = action;
            actionData = new Hashtable();
        }
        public InteractivityScript.Action action;
        public Hashtable actionData;
        
        public void setValue(object key, object value)
        {
            actionData[key] = value;
        }
        public Hashtable getSerialized()
        {
            actionData[InteractivityScript.CONST.ACTION_DATA] = action;
            return actionData;
        }
    }
}
