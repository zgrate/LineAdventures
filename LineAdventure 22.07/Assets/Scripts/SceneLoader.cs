using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    private bool loadScene = false;
    public Camera CameraMain;
    public Color BackgroundStartColor, BackgroundEndColor;
    private float f = 0;
    private bool up = true;
    public void Start()
    {
        string s = (string)DataPersistant.Instance.GetData("nextscene");
        ChangeScene(s);
        StartCoroutine(ColorChanger());
        
    }

    public IEnumerator ColorChanger()
    {
        for(;;)
        {
            CameraMain.backgroundColor = Color.Lerp(BackgroundStartColor, BackgroundEndColor, up ? f += 0.005f : f -= 0.005f);
            if (f >= 1)
                up = false;
            if (f <= 0)
                up = true;
            yield return null;
        }
    }

    public Text loadingText;
    // Updates once per frame
    public void ChangeScene(string s)
    {
        // ...set the loadScene boolean to true to prevent loading a new scene more than once...
        loadScene = true;

        // ...change the instruction text to read "Loading..."
        loadingText.text = "Loading...";

        // ...and start a coroutine that will load the desired scene.
        StartCoroutine(LoadNewScene(s));

    }
    
    void Update()
    {
        // If the new scene has started loading...
        if (loadScene == true)
        {
            // ...then pulse the transparency of the loading text to let the player know that the computer is still working.
            loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));
            
            
        }
    }


    // The coroutine runs on its own at the same time as Update() and takes an integer indicating which scene to load.
    IEnumerator LoadNewScene(string load)
    {

        // This line waits for 3 seconds before executing the next line in the coroutine.
        // This line is only necessary for this demo. The scenes are so simple that they load too fast to read the "Loading..." text.
       // yield return new WaitForSeconds(3);

        AsyncOperation async = SceneManager.LoadSceneAsync(load);
        // Start an asynchronous operation to load the scene that was passed to the LoadNewScene coroutine.
        //AsyncOperation async = SceneMana

        // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
        while (!async.isDone)
        {
            yield return null;
        }
    }

}