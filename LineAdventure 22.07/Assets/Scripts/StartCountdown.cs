using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Threading;

public class StartCountdown : MonoBehaviour {

    public UnityEvent EndExecution;
    public UnityEvent StartExecution;
    public TextMesh text;
    public int WaitTime = 2;
    public long temp;
    public Color ThreeColor, TwoColor, OneColor, ZeroColor, StartColor;
    public bool StopTime;

    private bool started = false;
    int timer = 3;


    IEnumerator StartCountDown()
    {
        while(true)
        {
            
            if (!started)
            {
                if (StopTime)
                    Time.timeScale = 0F;
                text.text = "3";
                text.color = ThreeColor;
                started = true;
            }
            else if (text.text.Equals("3"))
            {
                text.text = "2";
                text.color = TwoColor;
            }
            else if (text.text.Equals("2"))
            {
                text.text = "1";
                text.color = OneColor;
            }
            else if(text.text.Equals("1"))
            {
                text.text = "0";
                text.color = ZeroColor;
            }
            else if(text.text.Equals("0"))
            {
                text.text = "START";
                text.color = StartColor;
            }
            else
            {
                if (StopTime)
                    Time.timeScale = 1;
                EndExecution.Invoke();
                text.gameObject.SetActive(false);
                yield break;
            }
            Debug.Log("YES");
            yield return new WaitForRealSeconds(WaitTime);
        }
    }

    public IEnumerator Wait(float seconds)
    {
        temp = iTweenImplementation.CurrentTimeMillis();
        Debug.Log((iTweenImplementation.CurrentTimeMillis() - temp));
        while ((iTweenImplementation.CurrentTimeMillis() - temp) < seconds*1000)
        {
            Debug.Log("START " + (iTweenImplementation.CurrentTimeMillis() - temp));
            yield return null;
            Debug.Log("END" + (iTweenImplementation.CurrentTimeMillis() - temp));
        }
        Debug.Log("OUT " + (iTweenImplementation.CurrentTimeMillis() - temp));
    }

	// Use this for initialization
	void Start () {
        StartCoroutine(StartCountDown());	    
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log("UPDATE");
	}

}

public sealed class WaitForRealSeconds : CustomYieldInstruction
{
    private readonly long _endTime;

    public override bool keepWaiting
    {
        get {return _endTime > iTweenImplementation.CurrentTimeMillis(); }
    }

    public WaitForRealSeconds(long seconds)
    {
        _endTime = iTweenImplementation.CurrentTimeMillis() + seconds*1000;
    }
}