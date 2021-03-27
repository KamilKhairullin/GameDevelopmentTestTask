using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private GameObject deathObject;
    private Animator animator;
    private float currentHelth;

    private void Start()
    {
        currentHelth = maxHealth;
        animator = GetComponent<Animator>();
    }

    public void DecreseHealth(float amount)
    {
        currentHelth -= amount;

        if (currentHelth <= 0.0f)
        {
            animator.SetBool("isDead", true);
            
        }
    }

    private void Die()
    {
        Instantiate(deathObject, transform.position, transform.rotation);
        Destroy(gameObject);
        Application.LoadLevel (Application.loadedLevel);
    }
}
