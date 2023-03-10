using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField]
    public List<StepInfo> ChangeSpeedSteps;
    [Range(0, 1)]
    public float MusicProgress = 0;

    public DataManager dm;
    public StepEditManager sem;
    public AudioSource audioSource;

    public float nowT = 0;
    public float targetT;
    public bool GoToTarget;
    private void FixedUpdate()
    {
        if (!sem.EditMode)
        {
            nowT = audioSource.time;
            MusicProgress = nowT / audioSource.clip.length;
            for (int i = 0; i < ChangeSpeedSteps.Count; ++i)
            {
                float min = ChangeSpeedSteps[i].Info[0].x;
                float max = ChangeSpeedSteps[i].Info[0].y;
                float spd = ChangeSpeedSteps[i].Info[0].z;
                if (min < nowT && nowT < max)
                {
                    dm.ChangeSpeed = (nowT - (max - min)) / (max - min) * spd;
                }
            }

            sem.Steps.position = Vector3.down * dm.Speed * GlobalConst.SpeedRatio * nowT;
        }

        if (GoToTarget)
        {
            GoTo(targetT);
            GoToTarget = false;
        }
    }

    public void GoTo(float time)
    {
        if (time < 0)
        {
            Debug.Log(time);
            time = -time;
        }
        audioSource.time = time;
    }

    public void AddChangeSpeed(StepInfo CSS)
    {
        if (CSS.type != StepType.Speed)
            return;
        else
        {
            ChangeSpeedSteps.Add(CSS);
        }

        ChangeSpeedSteps = (List<StepInfo>)ChangeSpeedSteps.OrderBy(v => v.Info[0].x);
    }
}
