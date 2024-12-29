using UnityEngine;
using System.Collections;
using System.IO;

public class InteractivityScript : MonoBehaviour{

    public string fileToLoadName;
    public Hashtable data = new Hashtable();
    public Action actualAction = Action.IDLE;
    private Hashtable DATA;
    private Hashtable actualHashTable;
    public int pos = 0;
    public int count;

    public static class CONST
    {
        public const string
        ACTION_DATA = "actiondata", DATA_DATA = "dataData", DATA_COUNT = "dataCount",

        MOVE_START_POSITION = "movestartposition", MOVE_END_POSITION = "moveendposition", MOVE_SPEED = "movespeed", MOVE_LAST_FACTOR = "movelastfactor",
        MOVE_DATA_CONTAINER_END_POSITION = "movedataendposition", MOVE_DATA_CONTAINER_SPEED = "movedataspeed",

        STOP_TIME = "stoptime", STOP_START_MILISECONDS = "stopstartmiliseconds", STOP_ACTUAL_MILISECONDS = "stopactualmiliseconds",
        STOP_DATA_TIME = "stopdatatime",

        ROTATION_ANGLE_FINAL_AXIS = "rotaanglefinal", ROTATION_ANGLE_SPEED = "rotaanglespeed",
        ROTATION_ANGLE_DATA_SPEED = "rotationangledataspeed",
            
        ROTATION_TIME_DATA_TIME = "rotatetimedatatime", ROTATION_TIME_DATA_SPEED = "rotatetimedataspeed",
        ROTATION_TIME_TIME = "rotatetimetime", ROTATION_TIME_TARGET_AXIS = "rotatetimetargetaxis", ROTATION_TIME_SPEED = "rotatetimetargetspeed", ROTATION_TIME_CONSTANT_SPEED = "rotatetimeconstantspeed", ROTATION_TIME_START_MILIS = "rottimestartmili",

        RANDOM_MANDR_START_POSITION = "rmandrmovestartposition", RANDOM_MANDR_END_POSITION = "rmandmoveendposition", RANDOM_MANDR_MOVE_SPEED = "rmandmovespeed", RANDOM_MANDR_LAST_FACTOR = "rmandmovelastfactor",
        RANDOM_MANDR_ROTATION_CHANCE = "rmandrrotationchance", RANDOM_MANDR_ROTATION_SPEED = "rmandrotationspeed",
        RANDOM_MANDR_LAST_STATE = "rmandrlaststate",
        RANDOM_MANDR_DATA_ROTATION_SPEED = "rmandrdatarotationspeed", RANDOM_MANDR_DATA_MOVE_SPEED = "rmandrdatamovespeed", RANDOM_MANDR_DATA_ROTATION_CHANCE = "rmanddatarotationchance";
    }

    void Start()
    {
        DATA = (Hashtable)DataThread.GetData(new MemoryStream(((Resources.Load("AttackDatas/" +fileToLoadName.ToLower())) as TextAsset).bytes));
        count = (int)(DATA[CONST.DATA_DATA] as Hashtable)[CONST.DATA_COUNT];
    }

    void Update()
    {
        ActionCheck();
    }


    public void ActionCheck()
    {
        if(actualAction == Action.IDLE)
        {
            //Debug.Log("IDLE");
            actualHashTable = DATA[pos] as Hashtable;
            foreach(DictionaryEntry e in actualHashTable)
            {
                Debug.Log("DBG AHT: " + e.Key + " " + e.Value);
            }
            actualAction = (Action)actualHashTable[CONST.ACTION_DATA];
            pos++;
            if(pos > count-1)
            {
                pos = 0;
            }
            if(actualAction == Action.MOVEMENT)
            {
                MoveAction.prepareToMoveRandomly(this, (float)actualHashTable[CONST.MOVE_DATA_CONTAINER_SPEED]);
            }
            else if(actualAction == Action.STOP)
            {
                StopAction.prepareToStop(this, (float)actualHashTable[CONST.STOP_DATA_TIME]);
            }
            else if(actualAction == Action.ROTATION_ANGLE)
            {
                RotationAngleAction.prepareToRotate(this, (float)actualHashTable[CONST.ROTATION_ANGLE_DATA_SPEED]);
            }
            else if(actualAction == Action.ROTATION_TIME)
            {
                RotationTimeAction.prepareToTimedRotation(this, (float)actualHashTable[CONST.ROTATION_TIME_DATA_TIME], (float)actualHashTable[CONST.ROTATION_TIME_DATA_SPEED]);
            }
            else if(actualAction == Action.RANDOM_MOVE_ROTATE)
            {
                RandomRotateMoveAction.prepareToRandomMoveRotate(this, (float)actualHashTable[CONST.RANDOM_MANDR_DATA_MOVE_SPEED], (float)actualHashTable[CONST.RANDOM_MANDR_DATA_ROTATION_SPEED], (float)actualHashTable[CONST.RANDOM_MANDR_DATA_ROTATION_CHANCE]);
            }
        }
        if(actualAction == Action.MOVEMENT)
        {
            //Debug.Log("MOVE");
            MoveAction.moveCommand(this);
        }
        else if(actualAction == Action.STOP)
        {
            StopAction.stop(this);
        }
        else if(actualAction == Action.ROTATION_ANGLE)
        {
            RotationAngleAction.rotateAngle(this);
        }
        else if(actualAction == Action.ROTATION_TIME)
        {
            RotationTimeAction.Rotate(this);
        }
        else if(actualAction == Action.RANDOM_MOVE_ROTATE)
        {
            RandomRotateMoveAction.MOVE(this);
        }
    }
    public enum Action
    {
        MOVEMENT, ROTATION_ANGLE, ROTATION_TIME, STOP, IDLE, RANDOM_MOVE_ROTATE
    }




    public void ActionEnd(Action action)
    {
        Debug.Log("CANCEL");
        actualAction = Action.IDLE;
        if(action == Action.MOVEMENT)  
        {
            MoveAction.cleanUp(this);
        }
        else if(action == Action.STOP)
        {
            StopAction.cleanUP(this);
        }
        else if(action == Action.ROTATION_ANGLE)
        {
            RotationAngleAction.cleanUp(this);
        }
        else if(action == Action.ROTATION_TIME)
        {
            RotationTimeAction.CleanUP(this);
        }
        else if(action == Action.RANDOM_MOVE_ROTATE)
        {
            RandomRotateMoveAction.cleanUp(this);
        }
    }

    public static class MoveAction
    {
        public static void prepareToMove(InteractivityScript script, Hashtable table, object moveData, object speedData)
        {
            script.data[CONST.MOVE_START_POSITION] = script.gameObject.transform.position;
            script.data[CONST.MOVE_END_POSITION] = script.data[moveData];
            script.data[CONST.MOVE_SPEED] = script.data[speedData];
            script.data[CONST.MOVE_LAST_FACTOR] = 0f;
        }

        public static void prepareToMoveRandomly(InteractivityScript script, float speed = 0f)
        {
            script.data[CONST.MOVE_START_POSITION] = script.gameObject.transform.position;
            Vector2 v2 = Camera.main.ScreenToWorldPoint(new Vector3(Random.Range(0, Screen.width-100), Random.Range(0, Screen.height - 100),0f));
            script.data[CONST.MOVE_END_POSITION] = v2;
            if (speed != 0)
                script.data[CONST.MOVE_SPEED] = speed;
            else
            {
                float speedf = Random.Range(0f, 1f).Cut(2);
                if (speedf < 0.05f)
                    speedf = 0.1f;
                script.data[CONST.MOVE_SPEED] = speedf;
            }
            script.data[CONST.MOVE_LAST_FACTOR] = 0f;

        }

        
        public static void moveCommand(InteractivityScript script)
        {
            float lastFactor = (float)script.data[CONST.MOVE_LAST_FACTOR];
            float factor = (float)script.data[CONST.MOVE_SPEED];
            if (lastFactor < 1f)
            {
                lastFactor += factor;
                if (lastFactor > 1)
                    lastFactor = 1f;
                script.data[CONST.MOVE_LAST_FACTOR] = lastFactor;
                script.gameObject.GetComponent<Rigidbody2D>().MovePosition(calculatePoint((Vector3)script.data[CONST.MOVE_START_POSITION], (Vector2)script.data[CONST.MOVE_END_POSITION], lastFactor));
            }
            else
            {
                script.ActionEnd(Action.MOVEMENT);
            }
        }
        public static void cleanUp(InteractivityScript script)
        {
            script.data[CONST.MOVE_LAST_FACTOR] = 0;
            script.data[CONST.MOVE_START_POSITION] = null;
            script.data[CONST.MOVE_END_POSITION] = null;
            script.data[CONST.MOVE_SPEED] = null;
        }
        public static Vector2 calculatePoint(Vector2 start, Vector2 end, float k)
        {
            //(x,y) = (x1 + k(x2 - x1), y1 + k(y2 - y1)
            float x = (start.x + (k * (end.x - start.x)));
            float y = (start.y + (k * (end.y - start.y)));
            return new Vector3(x, y, 0f);
        }
    }

    public static class StopAction
    {
        public static void prepareToStop(InteractivityScript script, float secondToStop)
        {
            //Debug.Log("PREPARE TO STOP!");
            script.data[CONST.STOP_TIME] = secondToStop;
            script.data[CONST.STOP_START_MILISECONDS] = 0L;
        }

        public static void stop(InteractivityScript script)
        {
            if((long)script.data[CONST.STOP_START_MILISECONDS] == 0)
            {
                script.data[CONST.STOP_START_MILISECONDS] = CurrentTimeMillis();
            }
            if((CurrentTimeMillis())-(long)script.data[CONST.STOP_START_MILISECONDS] > ((float)script.data[CONST.STOP_TIME])*1000)
            {
                script.ActionEnd(Action.STOP);
            }
        }
        public static void cleanUP(InteractivityScript script)
        {
            script.data[CONST.STOP_START_MILISECONDS] = null;
            script.data[CONST.STOP_TIME] = null;
        }

        private static readonly System.DateTime Jan1st1970 = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

        public static long CurrentTimeMillis()
        {
            return (long)(System.DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }
    }

    public static class RotationAngleAction
    {
        public static void prepareToRotate(InteractivityScript script, float speed = 1f)
        {
            //Debug.Log("PREPARING TO ROTATE");
            script.data[CONST.ROTATION_ANGLE_FINAL_AXIS] = Random.Range(0, 360);
            script.data[CONST.ROTATION_ANGLE_SPEED] = speed;
        }
        public static void rotateAngle(InteractivityScript script)
        {
            //Debug.Log(((script.gameObject.transform.rotation.eulerAngles.z).Cut(1) - (int)script.data[CONST.ROTATION_ANGLE_FINAL_AXIS]) + " " + Mathf.Abs((float)script.data[CONST.ROTATION_ANGLE_SPEED]));
            if (Mathf.Abs((script.gameObject.transform.rotation.eulerAngles.z).Cut(1) - (int)script.data[CONST.ROTATION_ANGLE_FINAL_AXIS]) <= Mathf.Abs((float)script.data[CONST.ROTATION_ANGLE_SPEED]))
                script.ActionEnd(Action.ROTATION_ANGLE);
            else
                script.gameObject.transform.RotateAround(script.gameObject.GetComponent<MeshRenderer>().bounds.center, new Vector3(0, 0, 1), (float)script.data[CONST.ROTATION_ANGLE_SPEED]);
        }
        
        public static void cleanUp(InteractivityScript script)
        {
            script.data[CONST.ROTATION_ANGLE_FINAL_AXIS] = null;
            script.data[CONST.ROTATION_ANGLE_SPEED] = null;
        }
    }

    public static class RotationTimeAction
    {

        private static readonly System.DateTime Jan1st1970 = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

        public static long CurrentTimeMillis()
        {
            return (long)(System.DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }

        public static void prepareToTimedRotation(InteractivityScript script, float seconds = 10f, float speed = 0f)
        {
            script.data[CONST.ROTATION_TIME_CONSTANT_SPEED] = speed;
            script.data[CONST.ROTATION_TIME_TIME] = seconds;
            script.data[CONST.ROTATION_TIME_SPEED] = 0f;
            script.data[CONST.ROTATION_TIME_START_MILIS] = 0L;
        }

        public static void Rotate(InteractivityScript script)
        {
            if((long)script.data[CONST.ROTATION_TIME_START_MILIS] == 0L)
            {
                script.data[CONST.ROTATION_TIME_START_MILIS] = CurrentTimeMillis();
            }

            if (((float)script.data[CONST.ROTATION_TIME_SPEED]) == 0f || (Mathf.Abs((script.gameObject.transform.rotation.eulerAngles.z).Cut(1) - (int)script.data[CONST.ROTATION_TIME_TARGET_AXIS]) <= Mathf.Abs((float)script.data[CONST.ROTATION_TIME_SPEED])))
            {
                RandomizeData(script);
            }
            if (CurrentTimeMillis() - (long)script.data[CONST.ROTATION_TIME_START_MILIS] > ((float)script.data[CONST.ROTATION_TIME_TIME]) * 1000)
            {
                script.ActionEnd(Action.ROTATION_ANGLE);
            }
            else
            {
                script.gameObject.transform.RotateAround(script.gameObject.GetComponent<MeshRenderer>().bounds.center, new Vector3(0, 0, 1), (float)script.data[CONST.ROTATION_TIME_SPEED]);
            }

        }

        private static void RandomizeData(InteractivityScript script)
        {
            int RotationAxis = Random.Range(0, 360);
            if((float)script.data[CONST.ROTATION_TIME_CONSTANT_SPEED] == 0f)
            {
                float rotation = Random.Range(-5f, 5f);
                if(Mathf.Abs(rotation) < 2f)
                {
                    if(rotation < 0f)
                    {
                        rotation = -2f;
                    }
                    else
                    {
                        rotation = 2f;
                    }
                }
                script.data[CONST.ROTATION_TIME_SPEED] = (float )rotation;
            }
            else
            {
                script.data[CONST.ROTATION_TIME_SPEED] = script.data[CONST.ROTATION_TIME_CONSTANT_SPEED];
            }
            script.data[CONST.ROTATION_TIME_TARGET_AXIS] = RotationAxis;
  
        }
        public static void CleanUP(InteractivityScript script)
        {
            script.data[CONST.ROTATION_TIME_CONSTANT_SPEED] = null;
            script.data[CONST.ROTATION_TIME_TIME] = null ;
            script.data[CONST.ROTATION_TIME_SPEED] = null;
            script.data[CONST.ROTATION_TIME_START_MILIS] = null;
        }
    }
    public class RandomRotateMoveAction
    {

        public static void prepareToRandomMoveRotate(InteractivityScript script, float movespeed, float rotationspeed, float chance)
        {
            prepareMove(script, movespeed);
            prepareRotation(script, rotationspeed, chance);
            script.data[CONST.RANDOM_MANDR_LAST_STATE] = true;
        }

        private static void prepareMove(InteractivityScript script, float speed = 0.1f)
        {
            script.data[CONST.RANDOM_MANDR_START_POSITION] = script.gameObject.transform.position;
            Vector2 v2 = Camera.main.ScreenToWorldPoint(new Vector3(Random.Range(0, Screen.width - 100), Random.Range(0, Screen.height - 100), 0f));
            script.data[CONST.RANDOM_MANDR_END_POSITION] = v2;
            if (speed != 0)
                script.data[CONST.RANDOM_MANDR_MOVE_SPEED] = speed;
            else
            {
                float speedf = Random.Range(0f, 1f).Cut(2);
                if (speedf < 0.05f)
                    speedf = 0.1f;
                script.data[CONST.RANDOM_MANDR_MOVE_SPEED] = speedf;
            }
            script.data[CONST.RANDOM_MANDR_LAST_FACTOR] = 0f;
            Debug.Log(script.data[CONST.RANDOM_MANDR_START_POSITION] + " " + script.data[CONST.RANDOM_MANDR_END_POSITION] + " " + script.data[CONST.RANDOM_MANDR_MOVE_SPEED] + " " + script.data[CONST.RANDOM_MANDR_LAST_FACTOR]);

        }
        private static void prepareRotation(InteractivityScript script, float rotationspeed = 1f, float chance = 0.5f)
        {
            script.data[CONST.RANDOM_MANDR_ROTATION_SPEED] = rotationspeed;
            script.data[CONST.RANDOM_MANDR_ROTATION_CHANCE] = rotationspeed;
        }

     //   private static bool temp = true;
        public static void MOVE(InteractivityScript script)
        {
          //  bool temp = script.data[CONST.RANDOM_MANDR_LAST_STATE];
            if ((bool)script.data[CONST.RANDOM_MANDR_LAST_STATE])
            {
                if (!move(script))
                {
                    Debug.Log("ENDING");
                    script.ActionEnd(Action.RANDOM_MOVE_ROTATE);
                }
            }
            else
            {
                rotate(script);
            }
            script.data[CONST.RANDOM_MANDR_LAST_STATE] = !(bool)script.data[CONST.RANDOM_MANDR_LAST_STATE];
        }

        private static bool move(InteractivityScript script)
        {
                    
            float lastFactor = (float)script.data[CONST.RANDOM_MANDR_LAST_FACTOR];
            float factor = (float)script.data[CONST.RANDOM_MANDR_MOVE_SPEED];
            if (lastFactor < 1f)
            {
                lastFactor += factor;
                if (lastFactor > 1)
                    lastFactor = 1f;
                script.data[CONST.RANDOM_MANDR_LAST_FACTOR] = lastFactor;
                //Debug.Log("MOVE IN A RIGHT DIRECTION " + lastFactor);
                script.gameObject.GetComponent<Rigidbody2D>().MovePosition(calculatePoint((Vector3)script.data[CONST.RANDOM_MANDR_START_POSITION], (Vector2)script.data[CONST.RANDOM_MANDR_END_POSITION], lastFactor));
                return true;
            }
            else
            {
                return false;
            }
        
        }
        private static void rotate(InteractivityScript script)
        {
                
            float randomized = Random.Range(0f, 1f);
            Debug.Log(randomized);
            if(randomized < (float)script.data[CONST.RANDOM_MANDR_ROTATION_CHANCE])
            {
                script.data[CONST.RANDOM_MANDR_ROTATION_SPEED] = ((float)script.data[CONST.RANDOM_MANDR_ROTATION_SPEED])*-1;
            }
            script.gameObject.transform.RotateAround(script.gameObject.GetComponent<MeshRenderer>().bounds.center, new Vector3(0, 0, 1), (float)script.data[CONST.RANDOM_MANDR_ROTATION_SPEED]);


        }

        public static void cleanUp(InteractivityScript script)
        {
            script.data[CONST.RANDOM_MANDR_ROTATION_SPEED] = null;
            script.data[CONST.RANDOM_MANDR_ROTATION_CHANCE] = null;

            script.data[CONST.RANDOM_MANDR_START_POSITION] = null;
            script.data[CONST.RANDOM_MANDR_END_POSITION] = null;
            script.data[CONST.RANDOM_MANDR_MOVE_SPEED] = null;
            script.data[CONST.RANDOM_MANDR_LAST_FACTOR] = null;
        }

        public static Vector2 calculatePoint(Vector2 start, Vector2 end, float k)
        {
            //(x,y) = (x1 + k(x2 - x1), y1 + k(y2 - y1)
            float x = (start.x + (k * (end.x - start.x)));
            float y = (start.y + (k * (end.y - start.y)));
            return new Vector3(x, y, 0f);
        }

    }


    }
