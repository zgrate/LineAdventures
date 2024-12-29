using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class RandomScript : MonoBehaviour {

    public Toggle Hard, Normal, Easy;
    public Text NumberText;
    public Slider Slider;

    public Button SeedButton, StartButton;
    public InputField SeedTextField;

    [System.NonSerialized]
    public Difficulty Diff = Difficulty.NORMAL;

    public enum Difficulty
    {
        EASY, NORMAL, HARD
    }

    public void StartButtonClick()
    {
        ulong SEED;
        if (SeedTextField.text == "" || SeedTextField.text == null)
        {
            SEED = GetRandomULongSeed();
        }
        else {
            try
            {
                SEED = ulong.Parse(SeedTextField.text);
            }
            catch (Exception ex)
            {
                try
                {
                    SEED = TransformToNumbers(SeedTextField.text);
                }
                catch (Exception e)
                {
                    SEED = GetRandomULongSeed();
                }
            }
        }
        Debug.Log("Parsed Seed: " + SEED);
        DataPersistant.Instance.SetData("diff", Diff);
        DataPersistant.Instance.SetData("seed", SEED);
        DataPersistant.Instance.SetData("enemies", Slider.value);
        DataPersistant.Instance.SetData("nextscene", "LVL 1");
        SceneSwitcher.switchScene("LoadingScreen");
        
    }

    public ulong TransformToNumbers(string s)
    {
        ulong l = (ulong)s.Length;
        foreach(char c in s.ToCharArray())
        {
            l += c;
        }
        return l;
    }

    public void SeedButtonClick()
    {
        SeedTextField.text = GetRandomULongSeed() +"";
    }

    public ulong GetRandomULongSeed()
    {
        byte[] bytes = new byte[8];
        UnityEngine.Random.seed = (int)iTweenImplementation.CurrentTimeMillis();
        for(int i = 0; i < 8; i++)
        {
            bytes[i] = (byte)UnityEngine.Random.Range(0, 256);
        }
        return BitConverter.ToUInt64(bytes, 0);
    }


     

    public void ValueUpdate(bool togg)
    {
        Difficulty diff;
        if (Hard.Equals(togg))
            diff = Difficulty.HARD;
        else if (Normal.Equals(togg))
            diff = Difficulty.NORMAL;
        else diff = Difficulty.EASY;

        if (Diff == diff)// && togg.isOn)
        {
            return;
        }
        else {
            Diff = diff;
            return ;
            //if (diff == Difficulty.EASY)
            //{
            //    Hard.isOn = false;
            //    Normal.isOn = false;
            //    Easy.isOn = true;
            //}
            //else if (diff == Difficulty.NORMAL)
            //{
            //    Hard.isOn = false;
            //    Easy.isOn = false;
            //    Normal.isOn = true;
            //}
            //else
            //{
            //    Hard.isOn = true;
            //    Normal.isOn = false;
            //    Easy.isOn = false;
            //}
        }
        return ;
    }
    bool locked = false;

    public void HardUpdate()
    {
        if (locked)
            return;
        else if(Diff != Difficulty.HARD)
        {
            locked = true;
            Hard.isOn = true;
            Normal.isOn = false;
            Easy.isOn = false;
            Diff = Difficulty.HARD;
            locked = false;
        }
    }

    public void NormalUpdate()
    {
        if (locked)
            return;
        else if (Diff != Difficulty.NORMAL)
        {
            locked = true;
            Hard.isOn = false;
            Normal.isOn = true;
            Easy.isOn = false;
            Diff = Difficulty.NORMAL;
            locked = false;
        }
    }

    public void EasyUpdate()
    {
        if (locked)
            return;
        else if (Diff != Difficulty.EASY)
        {
            locked = true;
            Hard.isOn = false;
            Normal.isOn = false;
            Easy.isOn = true;
            Diff = Difficulty.EASY;
            locked = false;
        }
    }

    public void UpdateText()
    {
        NumberText.text = Slider.value.ToString();
    }

	// Use this for initialization
	void Start () {
        ;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
