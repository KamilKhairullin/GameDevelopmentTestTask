using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private GameObject openedDoor;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") &&  KeyController.instance.IsHoldingKey() && Input.GetButtonDown("Submit"))
        {
            Instantiate(openedDoor, gameObject.transform.position, gameObject.transform.rotation);
            KeyController.instance.RemoveKey();
            Destroy(gameObject);
        }
    }
}
