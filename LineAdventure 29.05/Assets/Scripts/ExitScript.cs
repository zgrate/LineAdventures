﻿using UnityEngine;
using System.Collections;

public class ExitScript : MonoBehaviour {

    public void ExitGame()
    {
        Debug.Log("Bye Bye!"); 
        Application.Quit();
    }
}