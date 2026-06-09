using System;
using UnityEngine;

public class Deadzone : MonoBehaviour
{
    public Transform objectOther;
    public Vector3 offset;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            GameObject enemy = other.gameObject;
            enemy.transform.position = new Vector2(objectOther.transform.position.x + offset.x,enemy.transform.position.y) ;
        }
    }

 
}
