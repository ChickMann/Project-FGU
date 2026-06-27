using UnityEngine;

public class PlayerKey : MonoBehaviour
{
    public bool hasKey = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Key"))
        {
            hasKey = true;
            Destroy(other.gameObject);

            Debug.Log("Đã nhặt chìa khóa");
        }
    }
}