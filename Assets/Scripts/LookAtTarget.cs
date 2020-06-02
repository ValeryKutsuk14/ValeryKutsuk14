using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    public Transform target;
    private Transform self_transform;

    public Vector3 offset;
    public float smoothSpeed = 0.125f;

    private void Start()
    {
        self_transform = transform;
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPos = target.position + offset;

            Vector3 smoothedPos = Vector3.Lerp(self_transform.position, desiredPos, smoothSpeed);

            self_transform.position = smoothedPos;
        }
    }
}
