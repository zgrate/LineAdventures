using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneSwitcher : MonoBehaviour {


    public static void switchScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }
    public static void switchScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void switchScenes(int scene)
    {
        SceneManager.LoadScene(scene);
    }
    public void switchScenes(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
