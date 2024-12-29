using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
public class lvlChooserScript : MonoBehaviour {

    private Game instance;
    public static List<Level> list = new List<Level>();

    // Use this for initialization
    void Start() {
        if (File.Exists("Saves/Game.data"))
        {
            FileStream stream = File.Create("Saves/Game.data");
            BinaryFormatter formatter = new BinaryFormatter();
            object obj = formatter.Deserialize(stream);
            instance = obj as Game;
        }
        else
        {
            instance = new Game();
            instance.firstGame();
        }

    }

    public void changeLevel(Level level)
    {
        Debug.Log("Changing Level!");
        SceneManager.LoadScene(level.getScene());
    }
	
    void save()
    {

    }
	// Update is called once per frame
	void Update () {
        Touch[] touches = Input.touches;
        if (touches.Length > 0)
        {
            Touch t = touches[0];
            Debug.Log(t.position + " " + gameObject.transform.position);
        }
        //Debug.Log("SIEMA");
    }

    public class Game
    {
        List<int> lvlPassed = new List<int>();
        public void firstGame()
        {
            
        }
        public void lvlPass(int i)
        {
            lvlPassed.Add(i);
        }
        public int last()
        {
            return lvlPassed[lvlPassed.Count - 1];
        }
        public bool canPlayGame(int i)
        {
            if (lvlPassed.Contains(i))
            {
                return true;
            }
            else if (last() + 1 == i)
                return true;
            return false;
        }
       
    }
}
