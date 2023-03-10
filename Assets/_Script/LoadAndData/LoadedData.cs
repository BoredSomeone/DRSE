using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class LoadedData : MonoBehaviour
{
    public AudioClip Music;
    string ProjectPath;
    public string[] RawSave;

    [SerializeReference]
    bool loadEnd = false;
    public bool hasSave = true;
    StepManager _sm;
    StepManager sm
    {
        get
        {
            if (_sm == null)
                _sm = GameObject.Find("GameManager").GetComponent<StepManager>();
            return _sm;
        }
    }

    public bool LoadEnd
    {
        get { return loadEnd; }
    }
    // Start is called before the first frame update
    void Awake()
    {
        if (PlayerPrefs.HasKey(GlobalConst.SaveKey.Path))
            ProjectPath = PlayerPrefs.GetString(GlobalConst.SaveKey.Path);
        else
            Debug.LogError("경로 지정되지 않음");
        StartCoroutine(AssetLoad());
    }

    IEnumerator AssetLoad()
    {
        int i = 0;
        string[] Contents = Directory.GetFiles(ProjectPath);
        string temp;
        AudioType type = AudioType.UNKNOWN;
        UnityWebRequest uwr;


        for (i = 0; i < Contents.Length; ++i)
        {
            temp = Contents[i];
            temp = temp[temp.Length - 4].ToString() + temp[temp.Length - 3].ToString() + temp[temp.Length - 2].ToString() + temp[temp.Length - 1].ToString();

            temp.ToLower();
            if (temp == ".wav")
            {
                type = AudioType.WAV;
                break;
            }

            else if (temp == ".mp3")
            {
                type = AudioType.MPEG;
                break;
            }

            Debug.Log(temp);
        }

        using (uwr = UnityWebRequestMultimedia.GetAudioClip(Contents[i], type))
        {
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                Music = DownloadHandlerAudioClip.GetContent(uwr);
                Debug.Log(uwr.url);
            }

            else
            {
                Debug.Log(uwr.error);
            }
        }

        using (uwr = UnityWebRequest.Get(ProjectPath + "/" + GlobalConst.SaveFileFullName))
        {
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                RawSave = uwr.downloadHandler.text.Split('\n'); 
                hasSave = true;
                Debug.Log(uwr.url);
            }
            else
            {
                hasSave = false;
                Debug.Log("No Save File\n" + uwr.error);
            }
        }
    

        loadEnd = true;
    }
}
