using UnityEngine;
using System.Collections;

public class MovingScript  {

    public GameObject gameObject;

    public MovingScript(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }

    public void moveCommand(Vector2 v2, float angle, Vector2 finalPosition)
    {
        Vector2 vector = Quaternion.AngleAxis(angle, Vector3.forward) * v2;
    }
}
