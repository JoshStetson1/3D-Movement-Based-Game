using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGun : Gun
{
    [Header("Projectile")]
    public GameObject bullet;
    public Transform bulletPoint;

    public float speed;
    public float spread;
    public float amount;

    public override void shoot()
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 spreadDir = new Vector3(Random.Range(spread, -spread), Random.Range(spread, -spread), 0) / 10;
            Vector3 direction = cam.transform.forward + spreadDir;

            GameObject newBullet = Instantiate(bullet, bulletPoint.position, bulletPoint.rotation);
            newBullet.GetComponent<Rigidbody>().velocity = fpsCam.transform.forward * speed;

            newBullet.GetComponent<Rigidbody>().AddTorque(360, 5, 20);
        }
    }
}
