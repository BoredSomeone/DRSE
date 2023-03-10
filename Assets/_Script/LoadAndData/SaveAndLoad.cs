using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class SaveAndLoad : MonoBehaviour
{
    public List<StepInfo> stepInfos = new List<StepInfo>();

    public Transform steps;
    public LoadedData ld;
    public StepEditManager sem;
    private void Awake()
    {
        StartCoroutine(LoadFile());
    }
    public void Save()
    {
        /*
        stepInfos.Clear();
        for (int i = 0; i < steps.childCount; ++i)
        {
            stepInfos.Add(steps.GetChild(i).GetComponent<Step>().myinfo);
        }
        */
        StartCoroutine(SaveFile());
    }

    public void Load()
    {
        if (!ld.LoadEnd)
        {
            Debug.Log("로딩 덜 끝남");
            return;
        }
        else if(!ld.hasSave)
        {
            Save();
        }

        stepInfos.Clear();

        string[] line, vec3;
        Vector3 info = Vector3.zero;
        StepInfo si;
        for(int i = 0; i < ld.RawSave.Length; ++i)
        {
            if (!string.IsNullOrWhiteSpace(ld.RawSave[i]) || !string.IsNullOrEmpty(ld.RawSave[i]))
            {
                si = new StepInfo();
                line = ld.RawSave[i].Split(':');
                Enum.TryParse(line[0], out si.type);
                for (int j = 1; j < line.Length - 1; ++j)
                {
                    try
                    {
                        vec3 = line[j].Split(',');
                        float.TryParse(vec3[0], out info.x);
                        float.TryParse(vec3[1], out info.y);
                        float.TryParse(vec3[2], out info.z);
                    }
                    catch
                    {
                        Debug.Log(line[j] + "\n" + ld.RawSave[i]);
                    }

                    si.Info.Add(info);
                }
                stepInfos.Add(si);
            }
        }
        
        for (int i = 0; i < stepInfos.Count; ++i)
        {
            GameObject temp = new GameObject();
            Step st = temp.AddComponent<Step>();

            temp.name = i.ToString();
            temp.transform.parent = steps;

            st.myinfo = stepInfos[i];
            st.MakeNote();
        }

        StartCoroutine(DelayRemake());
    }

    IEnumerator DelayRemake()
    {
        yield return new WaitForSecondsRealtime(1);
        sem.ReMake();
    }

    StepType TextToType(string text)
    {
        switch (text)
        {
            case "L":
                return StepType.L;
            case "R":
                return StepType.R;
            case "﻿﻿Down":
                return StepType.Down;
            case "Jump":
                return StepType.Jump;
            case "Speed":
                return StepType.Speed;
            default:
                Debug.Log("저장 값 오류" + text);
                return StepType.L;
        }
    }

    IEnumerator LoadFile()
    {
        while (!ld.LoadEnd)
            yield return null;

        if (ld.LoadEnd && ld.hasSave)
            Load();
    }

    IEnumerator SaveFile()
    {
        yield return null;
        int i = 0, j = 0;
        string ProjectPath = "";
        string SaveData = "";
        FileStream fs;
        StreamWriter sw;

        stepInfos.Clear();

        for(i = 0; i < steps.childCount; ++i)
        {
            stepInfos.Add(steps.GetChild(i).GetComponent<Step>().myinfo);
        }

        try
        {
            for (i = 0; i < stepInfos.Count; ++i)
            {
                SaveData += stepInfos[i].type.ToString() + ":";
                for (j = 0; j < stepInfos[i].Info.Count; ++j)
                {
                    SaveData += stepInfos[i].Info[j].x + "," + stepInfos[i].Info[j].y + "," + stepInfos[i].Info[j].z + ":";
                }
                SaveData += "\n";
            }
        }
        catch (Exception e)
        {
            Debug.Log(i);
            Debug.Log(j);
            Debug.Log(stepInfos.Count);
            Debug.Log(stepInfos[i].Info.Count);
            Debug.Log(e);
        }

        Debug.Log(SaveData);

        ProjectPath += PlayerPrefs.GetString(GlobalConst.SaveKey.Path) + "/" + GlobalConst.SaveFileFullName;

        //ProjectPath = Application.streamingAssetsPath + "/" + PlayerPrefs.GetString(GlobalConst.SaveKey.Name) + "/" + GlobalConst.SaveFileFullName;

        Debug.Log(ProjectPath);
        fs = new FileStream(ProjectPath , FileMode.Create, FileAccess.Write);
        sw = new StreamWriter(fs);

        sw.Write(SaveData);
        sw.Flush();
        sw.Close();
    }
}
