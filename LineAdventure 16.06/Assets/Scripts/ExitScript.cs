using UnityEngine;
using System.Collections;

public class ExitScript : MonoBehaviour {

    public void OnGUI()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ExitGame();
        }
    }
    public void ExitGame()
    {
        Debug.Log("Bye Bye!"); 
        Application.Quit();
    }
}
