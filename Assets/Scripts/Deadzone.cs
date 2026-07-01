using System;
using UnityEngine;

public class Deadzone : MonoBehaviour
{
    public GameObject spawnPoint;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
           other.gameObject.transform.position = spawnPoint.transform.position;
        }
    }

 
}
