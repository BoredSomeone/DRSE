using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlzSelectMe : MonoBehaviour
{
    public Step parent;

    public bool DragAble;
    public bool Selected;
    /*
    private void Start()
    {
        EventTrigger.Entry entey = new EventTrigger.Entry();
        entey.eventID = EventTriggerType.PointerClick;
        entey.callback.AddListener(delegate { parent.Select(); });

        gameObject.AddComponent<EventTrigger>().triggers.Add(entey);
    }
    */
    public void parentSet(Step p, bool drag)
    {
        parent = p;

        gameObject.AddComponent<PolygonCollider2D>();
        if (GetComponent<DrawTrapezoid>())
        {
            float ts, tp, h, bs, bp;
            var dt = GetComponent<DrawTrapezoid>();
            Vector2[] col = new Vector2[4];

            ts = dt.TopSize;
            tp = dt.TopPosition;
            h = dt.Height;
            bs = dt.BotSize;
            bp = dt.BotPosition;

            col[0] = new Vector2(bs / 2 + bp, 0);
            col[1] = new Vector2(-bs / 2 + bp, 0);
            col[2] = new Vector2(-ts / 2 + tp, h);
            col[3] = new Vector2(ts / 2 + tp, h);

            gameObject.GetComponent<PolygonCollider2D>().points = col;
        }
        DragAble = drag;
    }

    private void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            parent.Select();
            Selected = true;
        }
        else
        {
            if (EventSystem.current.currentSelectedGameObject)
                Debug.Log("block " + EventSystem.current.currentSelectedGameObject.name);
            else
                Debug.Log("¤±?¤©");
        }
        return;
    }

    private void OnMouseDrag()
    {
        if (DragAble && Selected && !EventSystem.current.IsPointerOverGameObject())
            parent.Drag();
    }

    private void OnMouseUp()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
            parent.DragEnd();
        Selected = false;
    }
}
