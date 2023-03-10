using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AssetLoader : MonoBehaviour
{
    public List<string> DirList;
    public string ProjectName;

    public GameObject pre_Button;
    public Transform ButtonContents;

    public Sprite DefaultImage;

    // Start is called before the first frame update
    void Start()
    {
        GetFolderList();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            GetFolderList();
        }
    }

    /// <summary>
    /// 메인 화면에서 폴더 리스트를 정렬합니다.
    /// </summary>
    void GetFolderList()
    {
        int i;
        string[] dirTemp;
        DirList.Clear();

        dirTemp = Directory.GetDirectories(Application.streamingAssetsPath);
        
        for(i = 0; i < dirTemp.Length; ++i)
        {
            if (Name(dirTemp[i]) != "DefaultAssets")
            {
                DirList.Add(dirTemp[i]);
            }
        }

        AddButton();
    }

    void AddButton()
    {
        int i;
        GameObject temp;

        for (i = 0; i < ButtonContents.childCount; ++i)
        {
            Destroy(ButtonContents.GetChild(i).gameObject);
        }

        for (i = 0; i < DirList.Count; ++i)
        {
            int ti = i;
            temp = Instantiate(pre_Button, ButtonContents);
            temp.GetComponent<Button>().onClick.AddListener(delegate { GoToEditor(ti); });
            temp.name = Name(DirList[i]);
            temp.GetComponentInChildren<Text>().text = Name(DirList[i]);
        }

        StartCoroutine(LoadImage());
    }

    string Name(string path)
    {
        string[] temp = path.Split('/', '\\');
        return temp[temp.Length - 1];
    }

    public void GoToEditor(int index)
    {
        PlayerPrefs.SetString(GlobalConst.SaveKey.Name, Name(DirList[index]));
        PlayerPrefs.SetString(GlobalConst.SaveKey.Path, DirList[index]);
        Debug.Log(Name(DirList[index]) + "\n" + DirList[index]);
        SceneManager.LoadScene("SampleScene");
    }

    IEnumerator LoadImage()
    {
        yield return new WaitForEndOfFrame();
        int i;
        Image img;
        UnityWebRequest uwr;
        Texture2D tex;
        Rect rect;
        Vector2 Pivot = Vector2.zero;

        for (i = 0; i < ButtonContents.childCount; ++i)
        {
            img = ButtonContents.GetChild(i).Find("ImageContainer").GetComponentInChildren<Image>();
            Debug.Log(img.transform.parent.parent.name);
            using (uwr = UnityWebRequestTexture.GetTexture(DirList[i] + "/image.png"))
            {
                yield return uwr.SendWebRequest();
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    tex = DownloadHandlerTexture.GetContent(uwr);
                    rect = new Rect(0, 0, tex.width, tex.height);

                    img.sprite = Sprite.Create(tex, rect, Pivot, 1024);
                }
                else
                {
                    Debug.Log(uwr.url + "\n" + uwr.error);
                }
            }
        }
    }

}
