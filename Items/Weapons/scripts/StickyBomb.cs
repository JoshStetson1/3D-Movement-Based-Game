using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyBomb : MonoBehaviour
{
    public LayerMask stickable;
    public float timeToBoom = 1.5f;
    public float checkRadius = 0.5f;
    public GameObject boom;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(Physics.CheckSphere(rb.position, checkRadius, stickable))
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;

            Invoke("Explode", timeToBoom);
        }
    }

    void Stick()
    {
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
    }

    void Explode()
    {
        Instantiate(boom, transform.position, Quaternion.Euler(-90, 0, 0));

        Destroy(this.gameObject);
    }
}
