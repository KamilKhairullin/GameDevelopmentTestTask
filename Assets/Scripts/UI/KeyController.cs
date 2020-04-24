using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyController : MonoBehaviour
{

    public static KeyController
        instance;
    
    private bool
        holdingKey;

    [SerializeField] private Image image;
    
    
    void Start()
    {
        image.enabled = false;
        if (instance == null)
        {
            instance = this;
        }
    }
    public void AddKey()
    {
        holdingKey = true;
        image.enabled = true;
    }

    public void RemoveKey()
    {
        holdingKey = false;
        image.enabled = false;
    }

    public bool IsHoldingKey()
    {
        return holdingKey;
    }
}
