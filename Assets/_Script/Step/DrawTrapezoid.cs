using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DrawTrapezoid : MonoBehaviour
{

    public float TopSize;
    public float TopPosition;

    public float Height;

    public float BotSize;
    public float BotPosition;

    [Space]
    public bool refresh = false;
    public Vector3[] ver;
    public Vector2[] uvs = new Vector2[]{
        new Vector2(0, 1),new Vector2(1, 1),new Vector2(0, 1),
        new Vector2(0, 0),new Vector2(1, 0),new Vector2(0, 0) };
    public int[] Triangle;

    Vector2[] BaseUV = new Vector2[]{
        new Vector2(0, 1),new Vector2(1, 1),new Vector2(0, 1),
        new Vector2(0, 0),new Vector2(1, 0),new Vector2(0, 0) };

    DataManager _dm;
    StepManager _sm;

    Mesh mesh;

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

    private void OnDrawGizmos()
    {
        if (refresh)
        {
            Draw();
            refresh = false;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (ver == null)
            return;
        for(int i = 0; i < ver.Length; ++i)
        {
            GUIStyle gs = new GUIStyle();
            gs.alignment = TextAnchor.UpperCenter;

            Handles.Label(ver[i] + transform.position, "" + i, gs);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(ver[0] + transform.position, ver[2] + transform.position);
            Gizmos.DrawLine(ver[2] + transform.position, ver[5] + transform.position);
            Gizmos.DrawLine(ver[5] + transform.position, ver[3] + transform.position);
            Gizmos.DrawLine(ver[3] + transform.position, ver[0] + transform.position);
            Gizmos.DrawLine(ver[1] + transform.position, ver[4] + transform.position);
        }
    }
#endif
    public void Draw(float botSize, float botPosition, float topSize, float topPosition, float height)
    {
        TopSize = topSize;
        TopPosition = topPosition;
        Height = height;
        BotSize = botSize;
        BotPosition = botPosition;

        StartCoroutine(WaitLoad());
    }

    public void ChangeHeight(float height)
    {
        Height = height;

        StartCoroutine(WaitLoad());
    }

    public void Draw()
    {
        //initSize(dm.NoteSize * DataManager.NoteSizeRatio / 2);
        initSize(0);
        initUv();

        if (mesh == null)
            mesh = new Mesh();

        mesh.vertices = ver;
        mesh.triangles = Triangle;
        mesh.uv = uvs;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        if (transform.GetComponent<MeshFilter>() == null)
            gameObject.AddComponent<MeshFilter>();
        transform.GetComponent<MeshFilter>().mesh = mesh;
        var mat = new Material(sm.shader);

        try
        {
            if (transform.parent.GetComponent<Step>().myinfo.type == StepType.L)
                mat.mainTexture = sm.sLSlide.texture;
            else if (transform.parent.GetComponent<Step>().myinfo.type == StepType.R)
                mat.mainTexture = sm.sRSlide.texture;
        }
        catch
        {
            mat = transform.GetComponent<MeshRenderer>().materials[0];
            Debug.LogWarning("텍스처를 못 찾았어요");
        }

        transform.GetComponent<MeshRenderer>().material = mat;
    }

    void initSize(float StartPoint)
    {
        float y = StartPoint + Height;
        float z = 0;

        ver = new Vector3[6];

        ver[0].Set(TopPosition - (TopSize / 2), y, z);
        ver[1].Set(TopPosition, y, z);
        ver[2].Set(TopPosition + (TopSize / 2), y, z);

        ver[3].Set(BotPosition - (BotSize / 2), 0, z);
        ver[4].Set(0, 0, z);
        ver[5].Set(BotPosition + (BotSize / 2), 0, z);

        Triangle = new int[]
        {
            0,1,3,
            1,4,3,
            1,5,4,
            1,2,5
        };
    }
    void initUv()
    {
        uvs = new Vector2[]
            {
                new Vector2(0, 1),new Vector2(1, 1),new Vector2(0, 1),
                new Vector2(0, 0),new Vector2(1, 0),new Vector2(0, 0)
            };
        uvs[4].x *= BotSize / TopSize;
    }

    private void Start()
    {
        if (!mesh)
            mesh = new Mesh();
    }

    IEnumerator WaitLoad()
    {
        while (!sm.LoadEnd)
        {
            yield return null;
        }

        Draw();
    }
}