using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public enum StepType
{
    L,
    R,
    Down,
    Jump,
    Speed,
}
[System.Serializable]
public class StepInfo
{
    public StepType type;
    [Tooltip("X = 위치, Y = 시간, Z = 크기")]
    /// <summary>
    /// X = 위치, Y = 시간, Z = 크기
    /// </summary>
    public List<Vector3> Info = new List<Vector3>();
}

[SelectionBase]
public class Step : MonoBehaviour
{
    [SerializeField]
    public StepInfo myinfo = new StepInfo();

    Transform TheFirstStep;


    public DataManager _dm;
    public StepManager _sm;
    public StepEditManager _sem;

    public List<Transform> StepParts = new List<Transform>();

    IEnumerator sCheck;

    DataManager dm
    {
        get
        {
            if (_dm == null)
                _dm = GameObject.Find("GameManager").GetComponent<DataManager>();
            return _dm;
        }
    }
    StepManager sm
    {
        get
        {
            if (_sm == null)
                _sm = GameObject.Find("GameManager").GetComponent<StepManager>();
            return _sm;
        }
    }
    StepEditManager sem
    {
        get
        {
            if (_sem == null)
                _sem = GameObject.Find("GameManager").GetComponent<StepEditManager>();
            return _sem;
        }
    }


    public void MakeNote(StepInfo info = null)
    {
        if (info != null)
            myinfo = info;

        if (myinfo.Info.Count == 0)
        {
            Destroy(gameObject);
            return;
        }
        Clear();

        transform.localPosition = new Vector3(myinfo.Info[0].x, myinfo.Info[0].y * dm.Speed * GlobalConst.SpeedRatio, 0);

        if (myinfo.type == StepType.Speed)
        {
            Speed();
        }

        else if (myinfo.type == StepType.Down || myinfo.type == StepType.Jump)
            DownOrJump();
        else
            DrawStep();

        for(int i = 0; i < StepParts.Count; ++i)
        {
            StepParts[i].gameObject.AddComponent<PlzSelectMe>().parentSet(this, myinfo.type != StepType.Speed);
        }
        SpeedChange();

        SpriteCheck();
    }
    public void SpeedChange()
    {
        Vector3 target = new Vector3();

        if (myinfo.type == StepType.Down || 
            myinfo.type == StepType.Jump || 
            myinfo.type == StepType.Speed)
            return;

        if (StepParts.Count != myinfo.Info.Count)
            Debug.LogWarning("스탭 개수 오류" + StepParts.Count + " != " + myinfo.Info.Count);

        target.x = myinfo.Info[0].x;
        target.y = myinfo.Info[0].y * dm.Speed * GlobalConst.SpeedRatio;
        target.z = 0;

        transform.localPosition = target;

        var Cursor = Vector2.zero;
        for (int i = 1; i < StepParts.Count; ++i)
        {
            var now = myinfo.Info[i];
            var preInfo = myinfo.Info[i - 1];
            var temp = StepParts[i];
            //일반 롱스탭
            if (now.x == 0 && preInfo.z == now.z)
            {
                temp.localPosition = new Vector3(Cursor.x, Cursor.y, 0);
                temp.localScale = new Vector3(now.z, now.y * dm.Speed * GlobalConst.SpeedRatio, 0);
                Cursor.y += now.y * dm.Speed * GlobalConst.SpeedRatio;
            }
            //런닝맨, 스폰지밥
            else if (now.x != 0 && now.y == 0)
            {
                target.x = Cursor.x;
                target.y = Cursor.y;
                target.z = 0;
                if (now.x < 0)
                {
                    temp.transform.rotation = Quaternion.identity;
                    temp.transform.Rotate(Vector3.forward * 90);
                    target.x += preInfo.z / 2;
                }
                else
                {
                    temp.transform.rotation = Quaternion.identity;
                    temp.transform.Rotate(-Vector3.forward * 90);
                    target.x -= preInfo.z / 2;
                }

                Cursor.x += now.x;

                temp.localPosition = target;
            }
            //슬라이드
            else
            {
                temp.GetComponent<DrawTrapezoid>().ChangeHeight(now.y * dm.Speed * GlobalConst.SpeedRatio);
                target.x = Cursor.x;
                target.y = Cursor.y;
                target.z = 0;

                Cursor.x += now.x;
                Cursor.y += now.y * dm.Speed * GlobalConst.SpeedRatio;

                temp.transform.localPosition = target;
            }
        }
    }

    public void SelectHighlight()
    {
        if (sem.EditMode == false)
            return;

        var c = Color.gray;
        for (int i = 0; i < StepParts.Count; ++i)
        {
            if (StepParts[i].GetComponent<SpriteRenderer>())
            {
                StepParts[i].GetComponent<SpriteRenderer>().color = c;
            }

            else if (StepParts[i].GetComponent<Renderer>())
            {
                StepParts[i].GetComponent<Renderer>().material.color = c;
            }
        }
    }
    public void SubSelectHighLight()
    {
        if (sem.EditMode == false)
            return;

        var c = new Color(0.25f, 0.25f, 0.25f, 1);
        for (int i = 0; i < StepParts.Count; ++i)
        {
            if (StepParts[i].GetComponent<SpriteRenderer>())
            {
                StepParts[i].GetComponent<SpriteRenderer>().color = c;
            }

            else if (StepParts[i].GetComponent<Renderer>())
            {
                StepParts[i].GetComponent<Renderer>().material.color = c;
            }
        }
    }
    public void NonSelectHighlight()
    {
        var c = Color.white;
        for (int i = 0; i < StepParts.Count; ++i)
        {
            if (StepParts[i].GetComponent<SpriteRenderer>())
            {
                StepParts[i].GetComponent<SpriteRenderer>().color = c;
            }
            else if (StepParts[i].GetComponent<MeshRenderer>())
            {
                StepParts[i].GetComponent<MeshRenderer>().material.color = c;
            }

            else if (StepParts[i].GetComponent<Renderer>())
            {
                StepParts[i].GetComponent<Renderer>().material.color = c;
            }
        }
    }
    public void Select()
    {
        sem.nowSelect = gameObject;
        sem.DragGap = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public void Drag()
    {
        sem.Drag(transform);
    }

    public void DragEnd()
    {
        sem.UpdateSelect();
    }

    void Clear()
    {
        int i;
        int j = 0;

        for (i = 0; i < StepParts.Count; ++i)
        {
            if (StepParts[i] == null)
            {
                StepParts.RemoveAt(i);
                --i;
            }
            ++j;

            if (j > 10000)
            {
                Debug.LogError("!!!");
                break;
            }
        }
        if (transform.childCount < 1)
            return;
        
        for (i = 0; i < StepParts.Count; ++i)
        {
            try
            {
                Destroy(StepParts[i].gameObject);
            }
            catch
            {
                Debug.Log("Editor Mode!");
            }
        }

        StepParts.Clear();
    }

    public void SizeChange()
    {
        Vector3 target = new Vector3();
        target.x = myinfo.Info[0].z;
        target.y = dm.NoteSize * GlobalConst.NoteSizeRatio;
        target.z = 1;

        MakeNote();
    }

    GameObject AddFirstNote()
    {
        var temp = new GameObject();
        temp.transform.parent = transform;
        temp.transform.localPosition = Vector3.zero;

        var ts = temp.AddComponent<SpriteRenderer>();
        ts.drawMode = SpriteDrawMode.Tiled;
        ts.tileMode = SpriteTileMode.Continuous;

        ts.size = Vector2.one;
        return temp;
    }

    void Speed()
    {
        float min = myinfo.Info[0].x;
        float max = myinfo.Info[0].y;
        float spd = myinfo.Info[0].z;
        float len = max - min;
        Vector3 size = Vector3.one;
        Vector3 target = Vector3.forward * 11;
        GameObject temp = AddFirstNote();
        SpriteRenderer sprTemp = temp.GetComponent<SpriteRenderer>();

        size.y = len * dm.Speed * GlobalConst.SpeedRatio;

        target.x = 11;
        target.y = min * dm.Speed * GlobalConst.SpeedRatio;

        if (sprTemp == null)
            sprTemp = temp.AddComponent<SpriteRenderer>();

        if (spd < 1)
            sprTemp.sprite = sm.sRLong;
        else
            sprTemp.sprite = sm.sLLong;

        temp.layer = LayerMask.NameToLayer("Step");
        temp.transform.localScale = size;
        transform.localPosition = target;

        StepParts.Add(temp.transform);
    }

    void DownOrJump()
    {
        var BaseTemp = AddFirstNote();
        var TopTemp = AddFirstNote();
        var BaseSprite = BaseTemp.GetComponent<SpriteRenderer>();
        var TopSprite = TopTemp.GetComponent<SpriteRenderer>();
        var size = Vector3.one;

        BaseTemp.transform.parent = transform;
        BaseTemp.transform.name = "Base";
        TopTemp.transform.parent = transform;
        TopTemp.transform.name = "Top";
        BaseTemp.transform.localPosition = Vector3.zero;
        TopTemp.transform.localPosition = Vector3.zero;



        if (myinfo.type == StepType.Jump)
        {
            BaseSprite.sprite = sm.sJumpBase;
            TopSprite.sprite = sm.sJumpTop;
        }
        else
        {
            BaseSprite.sprite = sm.sDownBase;
            TopSprite.sprite = sm.sDownTop;
        }

        size.x = myinfo.Info[0].z;
        size.y = 3;
        BaseTemp.transform.localScale = size;
        size.y = myinfo.Info[0].z;
        TopTemp.transform.localScale = size;
        TopTemp.transform.Rotate(Vector3.left * 70);

        TheFirstStep = BaseTemp.transform;
        TheFirstStep.gameObject.layer = LayerMask.NameToLayer("JumpDown");
        StepParts.Add(TheFirstStep);
        StepParts.Add(TopTemp.transform);
    }

    void DrawStep()
    {
        int i;
        Vector3 preInfo;
        Vector2 Cursor;

        GameObject temp;
        SpriteRenderer sprTemp;

        TheFirstStep = AddFirstNote().transform;
        TheFirstStep.name = "First";
        TheFirstStep.gameObject.layer = LayerMask.NameToLayer("Step");
        StepParts.Add(TheFirstStep);

        if (myinfo.type == StepType.L)
            TheFirstStep.GetComponent<SpriteRenderer>().sprite = sm.sL;
        else
            TheFirstStep.GetComponent<SpriteRenderer>().sprite = sm.sR;

        TheFirstStep.transform.localScale = new Vector3(myinfo.Info[0].z, dm.NoteSize * GlobalConst.NoteSizeRatio, 0);

        if (myinfo.Info.Count < 1)
            return;

        preInfo = myinfo.Info[0];
        Cursor = Vector2.zero;
        for(i = 1; i < myinfo.Info.Count; ++i)
        {
            Vector3 now = myinfo.Info[i];

            if (preInfo == now) continue;

            //일반 롱스탭
            if (now.x == 0 && preInfo.z == now.z)
            {
                temp = AddFirstNote();

                sprTemp = temp.GetComponent<SpriteRenderer>();
                if (sprTemp == null)
                    sprTemp = temp.AddComponent<SpriteRenderer>();

                if (myinfo.type == StepType.L)
                    sprTemp.sprite = sm.sLLong;
                else if (myinfo.type == StepType.R)
                    sprTemp.sprite = sm.sRLong;

                temp.transform.localPosition = new Vector3(Cursor.x, Cursor.y, 0);
                temp.transform.localScale = new Vector3(now.z, now.y * dm.Speed * GlobalConst.SpeedRatio, 0);

                Cursor.y += now.y * dm.Speed * GlobalConst.SpeedRatio;
                temp.transform.name = "Long Step" + i;
                temp.layer = LayerMask.NameToLayer("Long");
                StepParts.Add(temp.transform);
            }

            //런닝맨, 스폰지밥
            else if (now.x != 0 && now.y == 0)
            {
                temp = AddTrapezoid(
                    preInfo.z, 0,
                    now.z, 0,
                    Mathf.Abs(now.x) + preInfo.z);

                Vector3 target = new Vector3();
                target.x = Cursor.x;
                target.y = Cursor.y;
                target.z = 0;

                if (now.x < 0)
                {
                    temp.transform.rotation = Quaternion.identity;
                    temp.transform.Rotate(Vector3.forward * 90);
                    target.x += preInfo.z / 2;
                }
                else
                {
                    temp.transform.rotation = Quaternion.identity;
                    temp.transform.Rotate(-Vector3.forward * 90);
                    target.x -= preInfo.z / 2;
                }

                Cursor.x += now.x;

                temp.transform.localPosition = target;
                temp.transform.name = "ㄱ Step" + i;
                temp.layer = LayerMask.NameToLayer("Slide");
                StepParts.Add(temp.transform);
            }

            //슬라이드
            else
            {
                temp = AddTrapezoid(
                    preInfo.z, 0,
                    now.z, now.x,
                    now.y * dm.Speed * GlobalConst.SpeedRatio);


                Vector3 target = new Vector3();
                target.x = Cursor.x;
                target.y = Cursor.y;
                target.z = 0;

                Cursor.x += now.x;
                Cursor.y += now.y * dm.Speed * GlobalConst.SpeedRatio;

                temp.transform.localPosition = target;
                temp.transform.name = "Slide Step" + i;
                temp.layer = LayerMask.NameToLayer("Slide");
                StepParts.Add(temp.transform);
            }
            Vector2 p = transform.position;
            Debug.DrawLine(p + Cursor + Vector2.up, p + Cursor - Vector2.up, Color.blue, 10);
            Debug.DrawLine(p + Cursor + Vector2.left, p + Cursor - Vector2.left, Color.blue, 10);

            //Debug.Log(Cursor);

            preInfo = now;
        }

        SpriteRenderer[] s = transform.GetComponentsInChildren<SpriteRenderer>();
        for(i = 0; i < s.Length; ++i)
        {
            s[i].size = Vector2.one;
        }
    }

    void SpriteCheck()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            if (transform.GetChild(i).GetComponent<SpriteRenderer>())
            {
                var sr = transform.GetChild(i).GetComponent<SpriteRenderer>();
                if (sr.sprite == null)
                {
                    //Debug.Log("스프라이트 비어있음" + gameObject.name + "/" + transform.GetChild(i).name);
                    StartCoroutine(ReloadSprite(sr));
                }
            }
        }
    }

    IEnumerator ReloadSprite(SpriteRenderer sr)
    {
        while (!sm.LoadEnd)
            yield return new WaitForEndOfFrame();
        while (sr == null)
        {
            try
            {
                var color = sr.color;
                sr.color = Color.white;
                sr.color = color;
            }
            catch { }
            yield return new WaitForEndOfFrame();
        }
    }
    GameObject AddTrapezoid(float botSize, float botPosition, float topSize, float topPosition, float height)
    {
        GameObject temp = new GameObject();
        MeshRenderer meshTemp = temp.AddComponent<MeshRenderer>();
        temp.AddComponent<MeshFilter>();
        temp.AddComponent<DrawTrapezoid>();
        temp.transform.parent = transform;

        meshTemp.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshTemp.receiveShadows = false;
        meshTemp.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        meshTemp.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

        temp.GetComponent<DrawTrapezoid>().Draw(
            botSize, botPosition,
            topSize, topPosition,
            height);

        return temp;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        GUIStyle gs = new GUIStyle();
        gs.alignment = TextAnchor.MiddleCenter;
        Handles.Label(transform.position, "> " + transform.name + " <", gs);
    }
#endif
}
#if UNITY_EDITOR
[CustomEditor(typeof(Step))]
public class StepInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Step s = target as Step;

        if (GUILayout.Button("Make"))
            s.MakeNote();
        if (GUILayout.Button("Speed Change"))
            s.SpeedChange();
    }
}
#endif