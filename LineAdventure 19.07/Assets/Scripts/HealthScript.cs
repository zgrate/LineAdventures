using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class HealthScript : MonoBehaviour {

    private LineRenderer renderer;
    public float MaxHP;
    public UnityEvent LosedHPMethod;
	// Use this for initialization
    //TODO: SHARED?
	void Start () {
        renderer = gameObject.GetComponent<LineRenderer>();
        renderer.SetVertexCount(2);
        renderer.SetPositions(new Vector3[] { new Vector3(0, 0), new Vector3(500, 0) });
	}

    private float LosedHP = 0;

    public void DecreaseBar(float hp)
    {
        Debug.Log("BOOOOOOOM");
        LosedHP += hp;
        if( MaxHP-LosedHP <= 0 )
        {
            LosedHPMethod.Invoke();
        }
        
    }

    public float CalculateNewPosition()
    {
        //500 - MaxHP
        //500-x - LosedHP
        //500*losed = 500MaxHP - x * MaxHP
        //-x * max = 500MaxHP - 500*losed
        //x = -(500*MaxHP - 500*losed)/max

        //500 - MaxHP
        

        return -((-500 * MaxHP + 500 * LosedHP)/MaxHP);
    }

    public float calculatePrecentage()
    {
        //100 to MaxHP
        //x to LosedHP
        return 0;
    }
    public Color GetLinerInterpolate()
    {
        //1 to MaxHP
        //1-x to LosedHP
        //MaxHP - x * MaxHP = LosedHP
        //-x * MaxHP = LosedHP - MaxHP
        //x * MaxHP = -LosedHP + MaxHP
        //x = (-LosedHP + MaxHP)/MaxHP
        return Color.Lerp(Color.red, Color.green, (-1 * LosedHP + MaxHP) / MaxHP);
    }
	// Update is called once per frame
	void Update () {
        renderer.SetPositions(new Vector3[] { Vector3.zero, new Vector3(CalculateNewPosition(), 0)});
        Color color = GetLinerInterpolate();
        renderer.SetColors(color, color);

    }
}
