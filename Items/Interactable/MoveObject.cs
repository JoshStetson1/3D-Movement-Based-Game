using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : Action
{
    public Vector3 destination;
    public float speed;

    Vector3 startPos;
    Vector3 currentDest;

    Vector3 velocity;

    void Start()
    {
        startPos = transform.position;
        currentDest = startPos;
    }
    public override void Activate()
    {
        if (currentDest == startPos) currentDest = destination;
        else if (currentDest == destination) currentDest = startPos;
    }

    void FixedUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, currentDest, ref velocity, speed);
    }
}
