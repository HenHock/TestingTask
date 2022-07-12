using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HandType
{
    nothing,
    leftHand,
    rightHand,
    doubleHand
}

public class Item : MonoBehaviour
{
    [SerializeField] private protected HandType handType; // In which hand need put this item
    [SerializeField] private Vector3 positionInHand; // position weapon in hand
    [SerializeField] private Vector3 rotationInHand; // rotation weapon in hand

    private bool isSelected = false; // If item is selected
    public bool inHand { get; set; } = false; // If item is in one of hand

    private void Update()
    {
        if (isSelected && !inHand)
        {
            if (Input.GetKey(KeyCode.E))
            {
                if (Physics.CheckSphere(transform.position, 2.5f, LayerMask.GetMask("Player")))
                {
                    Inventory.AddItem(this);
                    gameObject.SetActive(false);
                }
            }
        }
    }

    //Set outline around item
    public void SelectedItem(bool flag)
    {
        if (GetComponent<Outline>() != null && !inHand)
        {
            GetComponent<Outline>().enabled = flag;
            isSelected = flag;
        }
    }

    public HandType GetHand()
    {
        return handType;
    }

    public void SetInHand()
    {
        transform.localPosition = positionInHand;
        transform.localRotation = Quaternion.Euler(rotationInHand);
        inHand = true;
    }

    private void OnDisable()
    {
        inHand = false;
    }
}
