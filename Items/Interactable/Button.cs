using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public Action action;
    public Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Push()
    {
        if(action) action.Activate();

        anim.SetTrigger("pushed");
    }
}
