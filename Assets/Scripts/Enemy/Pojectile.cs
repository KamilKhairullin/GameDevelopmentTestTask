using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pojectile : MonoBehaviour
{
    [SerializeField] private float speed, damage;
    private float[] attackDetails = new float[2]; 
    [SerializeField] private Rigidbody2D rigitbody;
    private float startTime;
    [SerializeField] private float timeToLive;
    
    // Start is called before the first frame update
    private void Start()
    {
        rigitbody.velocity = transform.right * speed;
        
    }
    private void Awake(){
        startTime = Time.time;
    }
    private void Update()
    {
        if (Time.time - startTime > timeToLive)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D hit)
    {
        PlayerCombat player = hit.GetComponent<PlayerCombat>();
        if (player != null)
        {
            attackDetails[0] = damage;
            attackDetails[1] = gameObject.transform.position.x;
            player.Damage(attackDetails);
        }
        Destroy(gameObject);
    }
}
