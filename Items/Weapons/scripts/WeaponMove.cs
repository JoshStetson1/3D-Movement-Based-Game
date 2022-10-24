using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.AnimatedValues;

public class WeaponMove : MonoBehaviour
{
    [Header("Sway")]
    public float swayAmount = 0.02f;
    public float swaySmoothAmount = 5;
    public float maxAmount = 0.1f;

    [Header("Recoil & Aim")]
    public Vector3 aimPos;
    public float aimSpeed = 0.5f;
    [HideInInspector]
    public bool isAiming = false;

    [Header("Positional")]
    public float posRecoilSpeed = 35;
    public float posReturnSpeed = 20;

    public Vector3 posRecoil = new Vector3(0.07f, 0, -0.2f);
    public Vector3 posRecoilAim = new Vector3(0.01f, 0, -0.2f);

    Vector3 tempPos;
    Vector3 targetPos;
    Vector3 originalPos;//used for sway too

    [Header("Rotational")]
    public float rotRecoilSpeed = 15;
    public float rotReturnSpeed = 35;

    public Vector3 rotRecoil = new Vector3(-35, 5, 5);
    public Vector3 rotRecoilAim = new Vector3(-20f, 3, 3);

    Vector3 tempRot;
    Vector3 rotation;//keep track of rotation as a vector

    void Start()
    {
        originalPos = transform.localPosition;
        targetPos = originalPos;
    }
    void Update()
    {
        if (Input.GetButtonDown("Fire2")) isAiming = true;
        if (Input.GetButtonUp("Fire2")) isAiming = false;

        if (isAiming) targetPos = aimPos;
        else targetPos = originalPos;
    }
    private void FixedUpdate()
    {
        //sway position
        float moveX = -Input.GetAxisRaw("Mouse X") * swayAmount;
        float moveY = -Input.GetAxisRaw("Mouse Y") * swayAmount;

        moveX = Mathf.Clamp(moveX, -maxAmount, maxAmount);
        moveY = Mathf.Clamp(moveY, -maxAmount, maxAmount);

        //positional
        Vector3 finalPos = new Vector3(moveX, moveY, 0);
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPos + targetPos, swaySmoothAmount * Time.deltaTime);
        if(isAiming) transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, aimSpeed * Time.deltaTime);

        //calculated recoil position added
        tempPos = Vector3.Lerp(tempPos, Vector3.zero, posReturnSpeed * Time.deltaTime);
        transform.localPosition = Vector3.Slerp(transform.localPosition, tempPos + transform.localPosition, posRecoilSpeed * Time.deltaTime);

        //rotational
        tempRot = Vector3.Lerp(tempRot, Vector3.zero, rotReturnSpeed * Time.deltaTime);
        rotation = Vector3.Slerp(rotation, tempRot, rotRecoilSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(rotation);
    }
    public void activateRecoil()
    {
        //recoil
        if (isAiming)
        {
            tempPos += new Vector3(Random.Range(-posRecoilAim.x, posRecoilAim.x), Random.Range(-posRecoilAim.y, posRecoilAim.y), posRecoilAim.z);
            tempRot += new Vector3(rotRecoilAim.x, Random.Range(-rotRecoilAim.y, rotRecoilAim.y), Random.Range(-rotRecoilAim.z, rotRecoilAim.z));
        }
        else
        {
            tempPos += new Vector3(Random.Range(-posRecoil.x, posRecoil.x), Random.Range(-posRecoil.y, posRecoil.y), posRecoil.z);
            tempRot += new Vector3(rotRecoil.x, Random.Range(-rotRecoil.y, rotRecoil.y), Random.Range(-rotRecoil.z, rotRecoil.z));
        }
    }
}
