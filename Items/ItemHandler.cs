using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    public Transform itemContainer;

    [HideInInspector]
    public Item activeItem;

    [HideInInspector]
    public List<Item> items = new List<Item>();
    private int index = 0;

    bool melee;

    void Start()
    {
        //go through items already in item container and add them to list
        for (int i = 0; i < itemContainer.childCount; i++)
        {
            if (itemContainer.GetChild(i).GetComponent<Item>())
            {
                items.Add(itemContainer.GetChild(i).GetComponent<Item>());

                if (i == 0)
                {
                    //activate game object and set active item
                    itemContainer.GetChild(i).gameObject.SetActive(true);
                    activeItem = itemContainer.GetChild(i).GetComponent<Item>();
                }
                //deactivate everything else
                else itemContainer.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
    public void Update()
    {
        //swap item
        if (Input.GetAxis("Mouse ScrollWheel") < 0) swapItem(-1);
        if (Input.GetAxis("Mouse ScrollWheel") > 0) swapItem(1);

        //mouse input for active item
        if (Input.GetButtonDown("Fire1")) activeItem.Pressed();
        if (Input.GetButtonUp("Fire1")) activeItem.Released();

        if (Input.GetButtonDown("Fire2")) activeItem.Pressed2();
        if (Input.GetButtonUp("Fire2")) activeItem.Released2();
    }
    public void replaceItem(Item item)
    {
        activeItem.Drop();

        item.PickUp();
    }
    public void swapItem(int dir)
    {
        //clamp direction value
        index = Mathf.Clamp(index += dir, 0, 1);

        //activate/ deactivate items
        for (int i = 0; i < itemContainer.childCount; i++)
        {
            if (i == index)
            {
                //activate game object and set active item
                itemContainer.GetChild(i).gameObject.SetActive(true);
                activeItem = itemContainer.GetChild(i).GetComponent<Item>();
            }
            //deactivate everything else
            else itemContainer.GetChild(i).gameObject.SetActive(false);

        }
    }
}
