using UnityEngine;

namespace Assets.Scripts
{
    public class Door : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if(!other.CompareTag("Player"))
                return;

            PlayerInventory inventory = other.GetComponent<PlayerInventory>();

            if (inventory.hasKey)
            {
                Destroy(gameObject);
                Debug.Log("Door opened!");
            }
            else
            {
                Debug.Log("You need a key to open this door.");
            }
        }
    }
}

