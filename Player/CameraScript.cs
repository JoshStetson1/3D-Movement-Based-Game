using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform headPos, orientation;
    public float landDampen = 5;

    //position
    public float posMoveSpeed = 35;
    public float posReturnSpeed = 20;

    //rotation
    public float rotMoveSpeed = 12;
    public float rotReturnSpeed = 5;
    public float clampAngle = 85;

    Vector3 camPos;
    Vector3 camRot;

    Vector3 tempPos;
    Vector3 position;

    Vector3 tempRot;
    Vector3 rotation;

    Vector3 moveVel;
    Vector3 returnVel;

    private void Start()
    {
        camPos = headPos.position;
    }
    void LateUpdate()
    {
        camPos = headPos.position;

        tempPos = Vector3.SmoothDamp(tempPos, Vector3.zero, ref returnVel, posReturnSpeed * Time.deltaTime);
        position = Vector3.SmoothDamp(position, tempPos, ref moveVel, posMoveSpeed * Time.deltaTime);
        transform.position = camPos + position;

        //return
        tempRot = Vector3.Lerp(tempRot, Vector3.zero, rotReturnSpeed * Time.deltaTime);
        rotation = Vector3.Slerp(rotation, tempRot, rotMoveSpeed * Time.deltaTime);

        transform.localRotation = Quaternion.Euler(camRot + rotation);
    }
    public void kickCam(float recoil)
    {
        tempRot.x -= recoil;
    }
    public void setRotation(Vector3 rotation)
    {
        camRot = rotation;
    }
    public void resetPosition()
    {
        tempPos = Vector3.zero;
        position = Vector3.zero;
    }
    public void land()
    {
        tempPos.y -= landDampen;
    }
}
