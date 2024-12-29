using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[ExecuteInEditMode]
public class LevelsManager : MonoBehaviour {

    public GameObject panel;
    public GameObject InstanceToClone;

    public Sprite PassedImage;
    public Sprite TooShortImageError;
    public Sprite LockImage;
    public float WindowWidth, WindowHeight;
    public float HeightSpace, WidthSpace;
    public float WidhtCounter;

    private Sprite BlendedPassedImage, BlendedTooShortImage, BlendedLockImage;
    // Use this for initialization
    RectTransform trans;
    void Start () {
        trans = (RectTransform)panel.transform;
        Vector2 v2 = trans.sizeDelta;
        //createLook();
        InstanceToClone.SetActive(false);
        int i = 1;
        foreach (LevelData data in LevelDatas)
        {
            GameObject obj = Instantiate(InstanceToClone);
            obj.transform.SetParent(panel.transform, false);
            obj.transform.position = data.PositionInWindow;
            ((RectTransform)obj.transform).sizeDelta = data.Size;
            obj.SetActive(true);

            if (data.BlendedLockImage == null)
            {
                Texture2D d2 = data.NormalImage.texture.AddWatermark(LockImage.texture);
                data.BlendedLockImage = Sprite.Create(d2, new Rect(0, 0, d2.width, d2.height), new Vector2(0.5f, 0.5f));
            }
            if (data.BlendedPassedImage == null)
            {
                Texture2D d2 = data.NormalImage.texture.AddWatermark(PassedImage.texture);
                data.BlendedPassedImage = Sprite.Create(d2, new Rect(0, 0, d2.width, d2.height), new Vector2(0.5f, 0.5f));
            }
            if (!DataPersistant.Instance.Playerdata.CompletedLevels.Contains(i) && DataPersistant.Instance.TheBiggestNumber+1 != i)
            {
                obj.GetComponent<Image>().sprite = data.BlendedLockImage;
                obj.GetComponent<Button>().enabled = false;
            }

            else if (DataPersistant.Instance.Playerdata.CompletedLevels.Contains(i))
            {
                obj.GetComponent<Image>().sprite = data.BlendedPassedImage;
                obj.GetComponent<Button>().enabled = true;
            }
            else
            {
                obj.GetComponent<Image>().sprite = data.NormalImage;
                obj.GetComponent<Button>().enabled = true;
            }
            i++;
        }
    }
	
    public void createLook()
    {
        float widthCount = 0, heightCount = 1;
        float widthLastPos = 0, heightLastPos = 0;

        float width = ((WidhtCounter*WindowWidth)+((WidhtCounter-1)*WidthSpace));
        int x = (int)(LevelDatas.Count / WidhtCounter);
        float height = ((x * WindowHeight) + ((x - 1) * HeightSpace));
        trans.sizeDelta.Set(trans.sizeDelta.x, height);

         /*
            Szerokość
            Ilość obiektów
            Szerokość obiektu

            x - ilość obiektów
            x*width + (x-1)*space = panel;
            x*width + x*space - space = panel
            x(widht + space) - space = panel
            x(w + s) = p + s |:(w+s)
            x = (panel + space)/(widht + space)

            
        */
        foreach (LevelData lvl in LevelDatas)
        {
            Debug.Log("ONE");
            widthCount++;
            GameObject obj = Instantiate<GameObject>(InstanceToClone);
            ((RectTransform)obj.transform).sizeDelta = new Vector2(WindowWidth, WindowHeight);
            obj.transform.SetParent(panel.transform, false);
            obj.transform.position = new Vector3((widthCount - 1)*(WindowWidth + WidthSpace), ((heightCount-1) * (WindowHeight + HeightSpace)));
            
            if(widthCount == WidhtCounter)
            {
                widthCount = 0;
                heightCount++;
            }
            
            //(widthCount-1)*WindowWidth+((widthCount-1)*WidthSpace)
            //(widthCount-1)(WindownWidth + widhtSpace)
        }
    }

    private List<GameObject> gm = new List<GameObject>();

    [System.Serializable]
    public class LevelData
    {
        [Tooltip("Name of Scene To Switch")]
        public string SceneName;
        [Tooltip("Target time of level")]
        public float Time;
        [Tooltip("Normal Image")]
        public Sprite NormalImage;
        [Tooltip("Lenght of line in lvl")]
        public float LineLenght;
        [Tooltip("Position in Window")]
        public Vector2 PositionInWindow;
        [Tooltip("Size")]
        public Vector2 Size;

        [System.NonSerialized]
        public Sprite BlendedPassedImage, BlendedTooShortImage, BlendedLockImage;
    }

    public List<LevelData> LevelDatas = new List<LevelData>();

    void OnDestroy()
    {
        Debug.Log("DESTROYING");
        foreach (GameObject g in gm)
        {
            DestroyImmediate(g);
        }
        gm.Clear();
    }
    public bool TestMode;
    // Update is called once per frame
    void Update () {
        if (TestMode)
        {
            RectTransform trans = (RectTransform)panel.transform;
            InstanceToClone.SetActive(false);
            foreach (GameObject g in gm)
            {
                DestroyImmediate(g);
            }
            gm.Clear();
            foreach (LevelData data in LevelDatas)
            {
                GameObject obj = Instantiate(InstanceToClone);
                obj.transform.SetParent(panel.transform, false);
                obj.transform.position = data.PositionInWindow;
                obj.GetComponent<Image>().sprite = data.NormalImage;
                ((RectTransform)obj.transform).sizeDelta = data.Size;
                obj.SetActive(true);
                gm.Add(obj);
            }
        }
        //Debug.Log(v2);
    }
}




public static class extenstion
{

    public static float Round(this float value, int digits)
    {
        float multi = Mathf.Pow(10f, digits);
        return Mathf.Round(value * multi) / multi;
    }
    public static float Cut(this float value, int digits)
    {
        float multi = Mathf.Pow(10f, digits);
        return ((int)(value * multi)) / multi;
    }

    public static Texture2D AlphaBlend(this Texture2D aBottom, Texture2D aTop)
    {

        if (aBottom.width != aTop.width || aBottom.height != aTop.height)
        {
            int width = Mathf.Min(aBottom.width, aTop.width);
            int height = Mathf.Min(aBottom.height, aTop.height);

            aTop.Resize(width, height);
        }
        var bData = aBottom.GetPixels();
        var tData = aTop.GetPixels();
        int count = bData.Length;
        var rData = new Color[count];
        for (int i = 0; i < count; i++)
        {
            Color B = bData[i];
            Color T = tData[i];
            float srcF = T.a;
            float destF = 1f - T.a;
            float alpha = srcF + destF * B.a;
            Color R = (T * srcF + B * B.a * destF) / alpha;
            R.a = alpha;
            rData[i] = R;
        }
        var res = new Texture2D(aTop.width, aTop.height);
        res.SetPixels(rData);
        res.Apply();
        return res;
    }

    public static Texture2D AddWatermark(this Texture2D background, Texture2D watermark)
    {
        Texture2D clone = Texture2D.Instantiate(background);
        int startX = 0;
        int startY = clone.height - watermark.height;

        for (int x = startX; x < clone.width; x++)
        {

            for (int y = startY; y < clone.height; y++)
            {
                Color bgColor = clone.GetPixel(x, y);
                Color wmColor = watermark.GetPixel(x - startX, y - startY);

                Color final_color = Color.Lerp(bgColor, wmColor, wmColor.a / 1.0f);

                clone.SetPixel(x, y, final_color);
            }
        }

        clone.Apply();
        return clone;
    }
}