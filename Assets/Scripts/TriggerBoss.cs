using System;
using UnityEngine;

public class TriggerBoss : MonoBehaviour
{
    public GameObject wall;
    public GameObject boss;

    private void Awake()
    {
        wall.SetActive(false);
        boss.SetActive(false);
    }

   
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            wall.SetActive(true);
            boss.SetActive(true);
            Destroy(this.gameObject);
        }
    }
}
