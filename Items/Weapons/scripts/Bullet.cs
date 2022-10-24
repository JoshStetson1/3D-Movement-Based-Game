using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject hitParticle;

    void Start()
    {
        Invoke("destroyAfterTime", 15);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BoxCollider>() != null)
        {
            if (other.GetComponent<Button>()) other.GetComponent<Button>().Push();

            Instantiate(hitParticle, transform.position, Quaternion.identity);
            Destroy(this);
        }
    }
    void destroyAfterTime()
    {
        Destroy(this);
    }
}
