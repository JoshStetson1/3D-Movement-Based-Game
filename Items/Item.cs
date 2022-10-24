using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [HideInInspector]
    public bool equipped;

    public bool canHold;
    bool holding;


    public virtual void Update()
    {
        if (holding) Use();
    }

    public virtual void Use() { }

    //left click
    public void Pressed()
    {
        if (canHold) holding = true;
        else Use();
    }
    public void Released()
    {
        holding = false;
    }

    //right click
    public void Pressed2() { }
    public void Released2() { }

    public virtual void PickUp() { }
    public virtual void Drop() { }

}
