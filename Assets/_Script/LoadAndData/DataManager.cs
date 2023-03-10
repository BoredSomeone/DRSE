using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    [SerializeField]
    float _speed = 1;
    [SerializeField]
    public float ChangeSpeed = 1;
    [SerializeField]
    public float NoteSize = 1;

    public StepEditManager sem;

    public float Speed
    {
        get
        {
            if (ChangeSpeed <= 0 || sem.EditMode)
                ChangeSpeed = 1;
            return _speed * ChangeSpeed;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey(GlobalConst.SaveKey.Speed))
            PlayerPrefs.SetFloat(GlobalConst.SaveKey.Speed, _speed);

        _speed = PlayerPrefs.GetFloat(GlobalConst.SaveKey.Speed);
    }

    public void SpeedChange(bool Add)
    {
        if (Add)
            _speed += 0.5f;
        else
        {
            _speed -= 0.5f;
            if (_speed < 0.5f)
                _speed = 0.5f;
        }

        PlayerPrefs.SetFloat(GlobalConst.SaveKey.Speed, _speed);
    }
}
