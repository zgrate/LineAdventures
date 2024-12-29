
using UnityEngine;
using System.Collections;

public class InteractivityScript : MonoBehaviour{

    public static class CONST
    {
        public const string MOVE_START_POSITION = "movestartposition", MOVE_END_POSITION = "moveendposition", MOVE_SPEED = "movespeed", MOVE_LAST_FACTOR = "movelastfactor";
    }

    void Start()
    {   

    }

    

    void Update()
    {
        ActionCheck();
    }

    public void ActionCheck()
    {
        if(actualAction == Action.IDLE)
        {
            //TODO
            actualAction = Action.MOVEMENT;
            MoveAction.prepareToMoveRandomly(this, 0.1f);
        }
        if(actualAction == Action.MOVEMENT)
        {
            MoveAction.moveCommand(this);
        }
    }
    public enum Action
    {
        MOVEMENT, ROTATION, STOP, IDLE
    }

    
    public Hashtable data = new Hashtable();

    public Action actualAction = Action.IDLE;

    public void ActionEnd(Action action)
    {
        actualAction = Action.IDLE;
        if(action == Action.MOVEMENT)
        {
            MoveAction.cleanUp(this);
        }
    }
    
    public Vector2 StartPosition;

    public static class MoveAction
    {
        public static void prepareToMove(InteractivityScript script, object moveData, object speedData)
        {
            script.data[CONST.MOVE_START_POSITION] = script.gameObject.transform.position;
            script.data[CONST.MOVE_END_POSITION] = script.data[moveData];
            script.data[CONST.MOVE_SPEED] = script.data[speedData];
            script.data[CONST.MOVE_LAST_FACTOR] = 0f;
        }

        public static void prepareToMoveRandomly(InteractivityScript script, float speed = 0f)
        {
            script.data[CONST.MOVE_START_POSITION] = script.gameObject.transform.position;
            Vector2 v2 = Camera.main.ScreenToWorldPoint(new Vector3(Random.Range(0, Screen.width), Random.Range(0, Screen.height),0f));
            script.data[CONST.MOVE_END_POSITION] = v2;
           // script.StartPosition = v2;
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
            script.lastFactor = lastFactor;
            script.factortomove = factor;
            script.StartPosition = (Vector3)script.data[CONST.MOVE_START_POSITION];
            script.finalPosition = (Vector2)script.data[CONST.MOVE_END_POSITION];

            if (lastFactor + factor < 1f)
            {
                lastFactor += factor;
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

    public Vector2 finalPosition;
    private Vector2 temp;
    public float factortomove = 0.01f;
    private Enemy enemy;



    public float lastFactor;
    public float lasttime;
    public void moveCommand(Vector2 startPosition, float factor, Vector2 finalPosition)
    {
        if (lastFactor + factor < 1)
        {
            lastFactor += factor;
            
        }
        else
        {
            //Debug.Log("Rotating");
            randomRotationAroundAxies();
        }
    }
    public float finalAxis = 90f;
    public int rounding = 1;
    public float axisSpeed = 1;


    public void randomRotationAroundAxies()
    {
        if (((gameObject.transform.rotation.eulerAngles.z).Cut(rounding)-finalAxis) <= Mathf.Abs(axisSpeed))
            RecalculateRandomness();
        gameObject.transform.RotateAround(gameObject.GetComponent<MeshRenderer>().bounds.center, new Vector3(0,0,1), axisSpeed);
    }
    public void RecalculateRandomness()
    {
        finalAxis = Random.Range(0, 360);
        axisSpeed = Random.Range(-5f, 5f).Round(1);
        
        if (axisSpeed == 0) axisSpeed = 2f;
        if(Mathf.Abs(axisSpeed) < 1f)
        {
            if (axisSpeed < 2) axisSpeed -= 1f;
            else axisSpeed += 1f;
        }
    }

}
