using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityGun : Gun
{
    [Header("Gravity Gun")]
    public Rigidbody playerRB;
    public float force;

    public override void shoot()
    {
        playerRB.AddForce(-cam.forward * force, ForceMode.Impulse);
    }
}
