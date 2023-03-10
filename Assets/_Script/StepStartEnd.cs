using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StepStartEnd : MonoBehaviour
{
    public Transform Steps;
    public Transform StartLine;
    public Transform EndLine;
    public List<Transform> Lines = new List<Transform>();

    [Space]

    public StepManager sm;
    public DataManager dm;
    public LoadedData ld;

    Vector3 endPosition = Vector3.zero;

    public GameObject line;


    private void Start()
    {
        StartLine.localPosition = Vector3.zero;
        StartCoroutine(WaitLoad());
    }

    private void Update()
    {
        transform.position = Steps.transform.position;
    }

    void DrawLine()
    {
        int i;
        int num = (int)ld.Music.length + 1;
        GameObject temp;
        Text t;

        for (i = 1; i < num; ++i)
        {
            temp = Instantiate(line);
            temp.transform.SetParent(transform, false);
            temp.transform.position = Vector3.up * dm.Speed * GlobalConst.SpeedRatio * i;
            temp.name = i.ToString();

            t = temp.transform.Find("Canvas").Find("Text").GetComponent<Text>();
            t.text = temp.name + sm.Text("Second");
        }
        temp = Instantiate(line);
        temp.name = ld.Music.length.ToString();
        t = temp.transform.Find("Canvas").Find("Text").GetComponent<Text>();
        t.text = temp.name + sm.Text("Second");
        temp.transform.Find("Canvas").SetParent(EndLine, false);
        Destroy(temp);
    }


    IEnumerator WaitLoad()
    {
        while (!ld.LoadEnd || !sm.LoadEnd)
        {
            yield return null;
        }
        DrawLine();

        endPosition.y = dm.Speed * GlobalConst.SpeedRatio * ld.Music.length;
        EndLine.localPosition = endPosition;
    }
}
