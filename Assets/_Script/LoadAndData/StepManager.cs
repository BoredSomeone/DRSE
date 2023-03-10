using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class OutSprite
{
    public Sprite value;
}

public class StepManager : MonoBehaviour
{
    public List<GameObject> Steps;
    /*
    public SpriteRenderer testObject;

    public GameObject L;
    public GameObject R;
    public GameObject LSlide;
    public GameObject RSlide;
    public GameObject LLong;
    public GameObject RLong;

    public GameObject Down;
    public GameObject Jump;
    */
    [Space]
    public Sprite sL;
    public Sprite sR;
    public Sprite sLSlide;
    public Sprite sRSlide;
    public Sprite sLLong;
    public Sprite sRLong;

    public Sprite sDownBase;
    public Sprite sDownTop;
    public Sprite sJumpBase;
    public Sprite sJumpTop;

    [Space]
    public Shader shader;

    public List<string[]> Texts = new List<string[]>();

    string dfp = Application.streamingAssetsPath + "/DefaultAssets/";

    public bool LoadEnd;

    private void Awake()
    {
#if !UNITY_EDITOR
        dfp = "file://" + dfp;
#endif
        StartCoroutine(LoadAsset("L", "R", "LS", "RS", "LL", "RL", "DB", "DT", "JB", "JT"));
    }

    // Start is called before the first frame update
    void Start()
    {

        Steps.Clear();
        try
        {
            var temp = GameObject.Find("Steps").GetComponentsInChildren<Step>();
            for (int i = 0; i < temp.Length; ++i)
            {
                Steps.Add(temp[i].gameObject);
            }
        }
        catch
        {

        }
    }


    public string Text(string key)
    {
        int i;
        if(LoadEnd == false)
        {
            Debug.Log("아직 로드 안 됨");
            return "";
        }
        for(i = 0; i < Texts.Count; ++i)
        {
            if (Texts[i][0] == key)
                return Texts[i][1];
        }

        Debug.Log("없는데요:\t" + key);
        return "";
    }

    string FilePath(string FileName)
    {

        return dfp + FileName + ".png";
    }

    void ApplyImage(Sprite s, string FileName)
    {
        if (s == null) return;
        switch (FileName)
        {
            case "L":
                sL = s;
                break;

            case "R":
                sR = s;
                break;

            case "LS":
                sLSlide = s;
                break;

            case "RS":
                sRSlide = s;
                break;

            case "LL":
                sLLong = s;
                break;

            case "RL":
                sRLong = s;
                break;


            case "DB":
                sDownBase = s;
                break;

            case "DT":
                sDownTop = s;
                break;

            case "JB":
                sJumpBase = s;
                break;

            case "JT":
                sJumpTop = s;
                break;
        }
    }

    IEnumerator LoadAsset(params string[] FileNames)
    {
        int i;
        UnityWebRequest uwr;
        Texture2D tex;
        Sprite result;
        Rect rect;
        Vector2 Pivot = Vector2.zero;

        for (i = 0; i < FileNames.Length; ++i)
        {
            using (uwr = UnityWebRequestTexture.GetTexture(FilePath(FileNames[i])))
            {
                yield return uwr.SendWebRequest();
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    tex = DownloadHandlerTexture.GetContent(uwr);

                    rect = new Rect(0, 0, tex.width, tex.height);

                    if (FileNames[i] == "DT" || FileNames[i] == "JT" || FileNames[i] == "LL" || FileNames[i] == "RL")
                        Pivot.Set(0.5f, 0);
                    else
                        Pivot.Set(0.5f, 0.5f);
                    result = Sprite.Create(tex, rect, Pivot, 256);
                }
                else
                {
                    Debug.Log(FilePath(FileNames[i]) + "\n" + uwr.error);
                    result = null;
                }
            }
            ApplyImage(result, FileNames[i]);
        }

        using (uwr = UnityWebRequest.Get(dfp + "/Texts.txt"))
        {
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                string[] allText;

                allText = uwr.downloadHandler.text.Split('\n');

                for (i = 0; i < allText.Length; ++i)
                {
                    string[] line = new string[2];
                    line[0] = allText[i].Split(':')[0];
                    line[1] = allText[i].Split(':')[1];
                    Texts.Add(line);
                }
            }
            else
            {
                Debug.Log(uwr.url + "\n" + uwr.error);
            }
        }

        LoadEnd = true;
    }

}