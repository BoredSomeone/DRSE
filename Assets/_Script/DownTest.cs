using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownTest : MonoBehaviour
{
    public float Speed = 1;

    private void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, Speed * Time.fixedDeltaTime);

        if (transform.position == Vector3.zero)
            transform.position = new Vector3(0, 100, 0);
    }
}
