using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsInHandController : MonoBehaviour
{
    /*
     * Script that control items in hand. Install, remove, throw item.
     * 
     * leftHand, rightHand - use as parent for item.
     * weaponInLeftHand, weaponInRightHand - items that are in hand.
     */
    [SerializeField]
    private Transform leftHand;
    [SerializeField]
    private Transform rightHand;
    [SerializeField]
    private GameObject weaponInLeftHand;
    [SerializeField]
    private GameObject weaponInRightHand; 

    private Item weaponInHand; // current item in hand;

    public bool isItemInLeftHand { get; private set; } // If in left hand is item
    public bool isItemInRightHand { get; private set; } // If in right hand is item

    private IDictionary<HandType, GameObject> weaponByHandType; // Dictionary for storing items by the type of hand (left, right and etc).

    private void Awake()
    {
        isItemInLeftHand = false;
        isItemInRightHand = false;

        weaponByHandType = new Dictionary<HandType, GameObject>()
        {
            {HandType.leftHand, weaponInLeftHand},
            {HandType.rightHand, weaponInRightHand},
            {HandType.doubleHand, weaponInRightHand},
        };

        // If we add a weapon in hands in the inspector from the start
        if (weaponInRightHand != null && weaponInRightHand.GetComponent<Item>() != null)
        {
            GameObject weapon = Instantiate(weaponInRightHand);
            weapon.GetComponent<Rigidbody>().isKinematic = true;
            weaponInHand = weapon.GetComponent<Item>();
            SetWeaponInHand();
        } 
        else if (weaponInLeftHand != null && weaponInLeftHand.GetComponent<Item>() != null)
        {
            GameObject weapon = Instantiate(weaponInLeftHand);
            weapon.GetComponent<Rigidbody>().isKinematic = true;
            weaponInHand = weapon.GetComponent<Item>();
            SetWeaponInHand();
        }
        else
        {
            weaponInRightHand = new GameObject("Empty Object");
            weaponInLeftHand = new GameObject("Empty Object");

            ChangeWeaponInHand(HandType.leftHand, weaponInLeftHand);
            ChangeWeaponInHand(HandType.rightHand, weaponInRightHand);
        }
    }
    /// <summary>
    /// Return current weapon as GameObject from the hand.
    /// </summary>
    /// <param name="hand">Type of hand</param>
    /// <returns>GameObject</returns>
    public GameObject GetWeaponInHand(HandType hand)
    {
        if (hand == HandType.doubleHand)
            return weaponByHandType[HandType.rightHand];

        return weaponByHandType[hand];
    }
    /// <summary>
    /// Change item in dictionary by type of hand to which the item belongs
    /// </summary>
    /// <param name="hand">Type of hand in which need to change the weapon</param>
    /// <param name="item">Item which need to put in the hand</param>
    private void ChangeWeaponInHand(HandType hand, GameObject item)
    {
        weaponByHandType[hand] = item;

        if (hand == HandType.leftHand)
            weaponInLeftHand = item;
        else if (hand == HandType.rightHand)
            weaponInRightHand = item;
        else if (hand == HandType.doubleHand)
        {
            weaponInLeftHand = item;
            weaponInRightHand = item;
        }
    }

    /// <summary>
    /// Drop an item out of hand on the floor close to the player. 
    /// To drop an item, it must be in the player's hands. 
    /// First, the object in the left hand is drop out, then if the left hand does not hold anything, it is drop out of the right.
    /// </summary>
    public void DropItemFromHand()
    {
        if (!weaponInLeftHand.name.Equals("Empty Object"))
        {
            Inventory.DeleteItem(weaponInLeftHand.GetComponent<Item>());

            weaponInLeftHand.transform.SetParent(transform.parent);
            Vector3 newPos = transform.localPosition;
            newPos.z += 0.5f;
            weaponInLeftHand.transform.localPosition = newPos;

            weaponInLeftHand = new GameObject("Empty Object");
            ChangeWeaponInHand(HandType.leftHand, weaponInLeftHand);
            isItemInLeftHand = false;
        }
        else if(!weaponInRightHand.name.Equals("Empty Object"))
        {
            Inventory.DeleteItem(weaponInRightHand.GetComponent<Item>());

            weaponInRightHand.transform.SetParent(transform.parent);
            Vector3 newPos = transform.localPosition;
            newPos.z += 0.5f;
            weaponInRightHand.transform.localPosition = newPos;

            weaponInRightHand = new GameObject("Empty Object");
            ChangeWeaponInHand(HandType.rightHand, weaponInRightHand);
            isItemInRightHand = false;
        }
    }

    /// <summary>
    /// Select item from inventory by id and pun in the hand
    /// </summary>
    /// <param name="id">Id from invetory list</param>
    public void SelecteWeapon(int id)
    {
        Item newItem = Inventory.GetItem(id);
        if (newItem != null)
        {
            weaponInHand = GetWeaponInHand(newItem.GetHand()).GetComponent<Item>();
            if (weaponInHand?.GetHand() != HandType.nothing)
                    weaponInHand?.gameObject.SetActive(false);

            if(newItem.GetHand() == HandType.doubleHand)
            {
                weaponInLeftHand.SetActive(false);
                weaponInRightHand.SetActive(false);
            }

            if (!newItem.inHand)
                weaponInHand = newItem;

            SetWeaponInHand();
        }
    }
    /// <summary>
    /// Put weapon in the hand
    /// </summary>
    private void SetWeaponInHand()
    {
        if (weaponInHand != null)
        {
            weaponInHand.gameObject.SetActive(true);

            if (weaponInHand.GetHand() == HandType.leftHand)
            {
                weaponInHand.transform.SetParent(leftHand);
                isItemInLeftHand = true;

                ChangeWeaponInHand(weaponInHand.GetHand(), weaponInHand.gameObject);
            }
            else if (weaponInHand.GetHand() == HandType.rightHand)
            {
                weaponInHand.transform.SetParent(rightHand);
                isItemInRightHand = true;
                ChangeWeaponInHand(weaponInHand.GetHand(), weaponInHand.gameObject);
            }
            else if(weaponInHand.GetHand() == HandType.doubleHand)
            {
                weaponInHand.transform.SetParent(rightHand);
                isItemInRightHand = true;
                isItemInLeftHand = false;

                ChangeWeaponInHand(HandType.leftHand, weaponInHand.gameObject);
                ChangeWeaponInHand(HandType.rightHand, weaponInHand.gameObject);
            }

            weaponInHand.SetInHand();

            Collider parentCollider = weaponInHand.transform.root.GetComponent<Collider>();
            Physics.IgnoreCollision(weaponInHand.gameObject.GetComponent<Collider>(), parentCollider);
        }
    }
}
