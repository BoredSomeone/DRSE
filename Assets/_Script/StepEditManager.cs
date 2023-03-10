using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StepEditManager : MonoBehaviour
{
    public bool EditMode = true;
    public bool UIEnable = true;
    public Transform Steps;
    public Transform StartEndLine;
    public Transform InfoField;
    [Space]
    public GameObject[] EditUI;
    public GameObject[] PlayUI;
    public GameObject StepInfoUI;
    public GameObject EditCameras;
    public GameObject PlayCameras;

    [Space, SerializeField]
    GameObject _nowSelect;
    [SerializeField]
    GameObject _subSelect;

    [SerializeField]
    public StepInfo SelectedInfo;
    public StepInfo SubSelectedInfo;

    [Space]
    public DataManager dm;
    public LoadedData ld;
    public StepManager sm;
    public TimeManager tm;
    public Dropdown InfoUI_Type;
    public GameObject InfoUIPrefab;

    public Vector3 DragGap;

    public GameObject nowSelect
    {
        get { return _nowSelect; }
        set
        {
            if (_nowSelect != value && EditMode)
            {
                if (_nowSelect)
                {
                    if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                    {
                        if (_subSelect)
                            _subSelect.GetComponent<Step>().NonSelectHighlight();

                        _subSelect = value.gameObject;
                        value.GetComponent<Step>().SubSelectHighLight();
                    }
                    else
                    {
                        if (_subSelect)
                        {
                            _subSelect.GetComponent<Step>().NonSelectHighlight();
                            _subSelect = null;
                        }

                        nowStep.NonSelectHighlight();
                        _nowSelect = value;
                        nowStep.SelectHighlight();
                    }
                }
                else
                {
                    if (_subSelect)
                    {
                        _subSelect.GetComponent<Step>().NonSelectHighlight();
                        _subSelect = null;
                    }

                    _nowSelect = value;
                    nowStep.SelectHighlight();
                }
                UpdateSelect();
            }
        }
    }

    public GameObject subSelect
    {
        get { return _subSelect; }
    }

    public Step nowStep
    {
        get { return nowSelect.GetComponent<Step>(); }
    }
    // Start is called before the first frame update
    void Start()
    {
        EditStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (EditMode)
            Scroll();
        else
        {
            SpeedChange();
            if(Input.GetKeyDown(KeyCode.A))
                AddStep();
        }
    }

    void Scroll()
    {
        if (Input.mouseScrollDelta.y != 0 && ld.LoadEnd && EditMode)
        {
            int i;
            bool OnLine = false;
            RaycastHit2D[] hit = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            for(i = 0; i < hit.Length; ++i)
            {
                if (hit[i].transform.parent.name == "StepLine")
                {
                    OnLine = true;
                    break;
                }
            }
            if (OnLine)
            {
                float speed = 3;
                if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                    speed *= 10;


                if (Input.mouseScrollDelta.y > 0)
                {
                    Steps.position += Vector3.down * speed;
                    if (Steps.position.y < -(ld.Music.length * dm.Speed * GlobalConst.SpeedRatio))
                    {
                        Steps.position = Vector3.down * ld.Music.length * dm.Speed * GlobalConst.SpeedRatio;
                    }
                }

                else if (Input.mouseScrollDelta.y < 0)
                {
                    Steps.position += Vector3.up * speed;
                    if (Steps.position.y > 0)
                        Steps.position = Vector3.zero;
                }
            }
        }
    }

    #region UI_Button_Action
    public void AddStep()
    {
        GameObject temp = new GameObject();
        Step st = temp.AddComponent<Step>();
        float nowTime;
        if (Steps.position.y == 0)
            nowTime = 0;
        else
        {
            nowTime = -Steps.position.y / (dm.Speed * GlobalConst.SpeedRatio);
        }

        temp.name = Steps.childCount.ToString();
        temp.transform.parent = Steps;

        st.myinfo.type = StepType.L;
        st.myinfo.Info.Add(new Vector3(0, nowTime, 7));
        st.MakeNote();
    }

    public void ConnectStep()
    {
        if (nowSelect && subSelect)
        {
            //y(시간)값에 따라서 메인과 서브를 바꿈
            if (nowSelect.transform.position.y > subSelect.transform.position.y)
            {
                var now = nowSelect;

                nowSelect = subSelect;
                _subSelect = now;
            }
            var subInfotemp = subSelect.GetComponent<Step>().myinfo.Info;
            var nowInfoTemp = nowStep.myinfo.Info;
            float nowLastX = 0;
            float nowLastY = 0;


            for (int i = 0; i < nowInfoTemp.Count; ++i)
            {
                nowLastX += nowInfoTemp[i].x;
                nowLastY += nowInfoTemp[i].y;
            }

            var nowP = subInfotemp[0];
            nowP.x -= nowLastX;
            nowP.y -= Mathf.Abs(nowLastY);
            subInfotemp[0] = nowP;

            nowStep.myinfo.Info.AddRange(subInfotemp);

            Destroy(subSelect);

            nowStep.MakeNote();
            nowStep.SelectHighlight();
            UpdateSelect();
        }
    }

    public void DeleteStep()
    {
        Destroy(nowSelect);
        if (subSelect)
            nowSelect = subSelect;
    }

    public void AddInfo()
    {
        if (nowSelect)
        {
            Vector3 temp = nowStep.myinfo.Info[nowStep.myinfo.Info.Count - 1];
            temp.x = 0;
            temp.y = 1;
            nowStep.myinfo.Info.Add(temp);
            nowStep.MakeNote();
            nowStep.SelectHighlight();

            UpdateSelect();
        }
    }

    public void DelInfo(int i)
    {
        if (nowSelect)
        {
            nowStep.myinfo.Info.RemoveAt(i);
            nowStep.MakeNote();
            nowStep.SelectHighlight();

            UpdateSelect();
        }
    }

    public void EditInfo(InputField field)
    {
        int index; //몇번이 바뀜
        float value; //얼마로 바뀜
        if (int.TryParse(field.transform.parent.name, out index))
        {
            Vector3 target = nowStep.myinfo.Info[index];

            if (float.TryParse(field.text, out value))
            {
                switch (field.name.ToLower()[0])
                {
                    case 'x':
                        target.x = value;
                        break;

                    case 'y':
                        target.y = value;
                        break;

                    case 'z':
                        target.z = value;
                        break;

                    default:
                        Debug.Log(field.name.ToLower()[0]);
                        break;
                }

                nowStep.myinfo.Info[index] = target;
                nowStep.MakeNote();
                nowStep.SelectHighlight();
                Debug.Log(index + "" + target + "\n" + 
                    field.transform.parent.name + ", " + field.text
                    );
            }
        }
    }

    public void EditType()
    {
        nowStep.myinfo.type = (StepType)InfoUI_Type.value;

        if (nowStep.myinfo.type == StepType.Down || nowStep.myinfo.type == StepType.Jump)
        {
            Vector3 info = nowStep.myinfo.Info[0];
            info.x = 0;
            info.z = 20;
            nowStep.myinfo.Info[0] = info;
            Debug.Log("To Down or Jump");
        }
        else if(nowStep.myinfo.type == StepType.Speed)
        {
            Vector3 info = nowStep.myinfo.Info[0];
            info.y = info.x + 1;
            info.z = 1;
            nowStep.myinfo.Info[0] = info;
            Debug.Log("To Speed");
        }
        nowStep.MakeNote();
        nowStep.SelectHighlight();
    }

    public void CopySelect()
    {
        if (nowSelect)
        {
            GameObject temp = Instantiate(nowSelect);
            temp.transform.parent = Steps;
        }
    }

    public void Pause()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            tm.audioSource.UnPause();
        }
        else
        {
            Time.timeScale = 0;
            tm.audioSource.Pause();
        }
    }

    public void GoMain()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    #endregion

    public void Drag(Transform step)
    {
        float nowTime;
        Vector3 P = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        P += DragGap;

        P.z = step.GetComponent<Step>().myinfo.Info[0].z;

        nowTime = (P.y-Steps.position.y) / (dm.Speed * GlobalConst.SpeedRatio);
        step.position = P;
        step.GetComponent<Step>().myinfo.Info[0] = new Vector3(P.x, nowTime, P.z);
    }


    public void SpeedChange()
    {
        int i;
        for (i = 0; i < Steps.childCount; ++i)
        {
            Steps.GetChild(i).GetComponent<Step>().SpeedChange();
        }
    }

    public void ReMake()
    {
        int i;
        for (i = 0; i < Steps.childCount; ++i)
        {
            Steps.GetChild(i).GetComponent<Step>().MakeNote();
        }
    }

    public void EditStart(bool FromNow = false)
    {
        int i;
        EditMode = true;
        EditCameras.SetActive(true);
        PlayCameras.SetActive(false);
        StartEndLine.gameObject.SetActive(true);
        dm.ChangeSpeed = 1;

        PlayCameras.GetComponent<AudioSource>().Stop();

        if (FromNow == false)
        {
            Steps.position = Vector3.zero;
            tm.GoTo(0);
        }

        for (i = 0; i < PlayUI.Length; ++i)
            PlayUI[i].SetActive(false);
        for (i = 0; i < EditUI.Length; ++i)
            EditUI[i].SetActive(true);

        SpeedChange();
    }

    public void PlayStart(bool FromNow = false)
    {
        int i;
        float now = 0;
        EditMode = false;
        nowSelect = null;
        EditCameras.SetActive(false);
        PlayCameras.SetActive(true);
        StartEndLine.gameObject.SetActive(false);

        PlayCameras.GetComponent<AudioSource>().clip = ld.Music;
        PlayCameras.GetComponent<AudioSource>().Play();

        if (FromNow)
            now = Steps.position.y / (dm.Speed * GlobalConst.SpeedRatio);

        Steps.position = Vector3.up * now * dm.Speed * GlobalConst.SpeedRatio;
        tm.GoTo(now);

        if (UIEnable)
            for (i = 0; i < PlayUI.Length; ++i)
                PlayUI[i].SetActive(true);
        for (i = 0; i < EditUI.Length; ++i)
            EditUI[i].SetActive(false);
    }

    public void UpdateSelect()
    {
        int i;
        Text xt, yt, zt;
        Button btnDel;
        GameObject infoUI;

        StepInfoUI.SetActive(true);

        InfoUI_Type.value = (int)nowStep.myinfo.type;

        for (i = 0; i < InfoField.childCount; ++i)
        {
            Destroy(InfoField.GetChild(i).gameObject);
        }

        for (i = 0; i < nowStep.myinfo.Info.Count; ++i) {
            int n = i;
            InputField xi, yi, zi;
            infoUI = Instantiate(InfoUIPrefab, InfoField);
            infoUI.name = n.ToString();

            btnDel = infoUI.transform.Find("btn_del").GetComponent<Button>();
            btnDel.onClick.AddListener(delegate { DelInfo(n); });

            xt = infoUI.transform.Find("x").GetComponent<Text>();
            yt = infoUI.transform.Find("y").GetComponent<Text>();
            zt = infoUI.transform.Find("z").GetComponent<Text>();

            xi = infoUI.transform.Find("xInput").GetComponent<InputField>();
            yi = infoUI.transform.Find("yInput").GetComponent<InputField>();
            zi = infoUI.transform.Find("zInput").GetComponent<InputField>();

            if (nowStep.myinfo.type == StepType.Speed)
            {
                xt.text = sm.Text("Start");
                yt.text = sm.Text("End");
                zt.text = sm.Text("Target");
            }
            else
            {
                xt.text = sm.Text("Position");
                yt.text = sm.Text("Time");
                zt.text = sm.Text("Size");
            }

            xi.text = nowStep.myinfo.Info[i].x.ToString();
            yi.text = nowStep.myinfo.Info[i].y.ToString();
            zi.text = nowStep.myinfo.Info[i].z.ToString();

            xi.onEndEdit.AddListener(delegate { EditInfo(xi); });
            yi.onEndEdit.AddListener(delegate { EditInfo(yi); });
            zi.onEndEdit.AddListener(delegate { EditInfo(zi); });
        }

        //if(nowStep.myinfo.type == StepType.Down)
    }
}
