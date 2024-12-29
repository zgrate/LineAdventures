using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class Level : MonoBehaviour {

    public int levelNumber;
    public string sceneToSwitch;
    public long maxTimeSeconds;
    public float LineLength = 1000;
    public Sprite BlockedImage;
    public Sprite NormalImage;
    public Sprite PassedImage;
    public Sprite TooShortImageError;


    public Image ImageComponent;

    public string getScene()
    {
        return sceneToSwitch;
    }

	// Use this for initialization
	void Start () {}
	
    public void setLockedImage()
    {
        Texture2D d2 = NormalImage.texture.AlphaBlend(BlockedImage.texture);
        
        setImage(Sprite.Create(d2, new Rect(0, 0, d2.width, d2.height), new Vector2(0.5f, 0.5f)));
    }

    public void setPassedImage()
    {
        setImage(PassedImage);
    }

    public void SetTooShortLineError()
    {
        setImage(TooShortImageError);
    }

    public void setNormalImage()
    {
        setImage(NormalImage);
    }
    public void setImage(Sprite sprite)
    {
        ImageComponent.sprite = sprite;
    }
}
