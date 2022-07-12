using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedManager : MonoBehaviour
{
    /*
     * Scrip need to select the items on which player brought mouse.
     */

    private GameObject hitObject;
    private Vector3 point;

    private void Update()
    {
        if (hitObject != null && hitObject.GetComponent<Item>() != null)
            hitObject.GetComponent<Item>().SelectedItem(false);

        point = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        Ray ray = Camera.main.ScreenPointToRay(point);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            hitObject = hit.transform.gameObject;
            if (hitObject.GetComponent<Item>() != null && !hitObject.CompareTag("Player"))
                hitObject.GetComponent<Item>().SelectedItem(true);
        }
    }
}
