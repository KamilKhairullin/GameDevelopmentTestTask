using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPotion : MonoBehaviour
{
    [SerializeField] private float
        time,
        jumpForce;
    private float
        currentTime;

    private float[]
        buffDetails = new float[3];
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            buffDetails[0] = jumpForce;
            buffDetails[1] = time;
            buffDetails[2] = Time.time;
            PlayerMovement.instance.GetBuff(buffDetails);
        }
    }
}
