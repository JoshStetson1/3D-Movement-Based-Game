using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Item
{
    public Camera fpsCam;
    public Transform cam;

    public ParticleSystem muzzelFlash;

    [Header("Settings")]
    public float recoil;
    public float fireRate;
    public float damage;
    public float magSize;
    public float relodeTime;

    [HideInInspector]
    public bool readyToShoot, isReloading;
    float magCount;

    WeaponMove moveScript;
    CameraScript camScript;

    private void Awake()
    {
        if (GetComponent<WeaponMove>()) moveScript = GetComponent<WeaponMove>();
        if (cam.GetComponent<CameraScript>()) camScript = cam.GetComponent<CameraScript>();

        magCount = magSize;
        readyToShoot = true;
    }
    public override void Use()
    {
        Shoot();
    }

    //shooting mechanics
    public override void Update()
    {
        base.Update();

        if (magCount <= 0)
        {
            if (!isReloading) Invoke("relode", relodeTime);
            isReloading = true;
            readyToShoot = false;
            return;
        }
    }

    public void Shoot()
    {
        if (!readyToShoot) return;

        shoot();

        if (moveScript) moveScript.activateRecoil();
        if (camScript) camScript.kickCam(recoil);

        if(muzzelFlash) muzzelFlash.Play();

        magCount--;
        readyToShoot = false;
        Invoke("getReady", fireRate);

        //FindObjectOfType<AudioManager>().Play("gunShot");
    }
    public virtual void shoot() {}

    void getReady()
    {
        readyToShoot = true;
    }
    public void relode()
    {
        if (magCount == magSize) return;

        magCount = magSize;
        isReloading = false;
        readyToShoot = true;
    }
}
