using UnityEngine;

namespace Assets.Scripts
{
    public class ItemPickup : MonoBehaviour
    {
        public ItemType itemType;
        public int healthAmount = 20;
        public int coinAmount = 10;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            PlayerHealth health = other.GetComponent<PlayerHealth>();

            switch (itemType)
            {
                case ItemType.HealthPotion:
                    inventory.potionCount++;
                    Debug.Log("Potion Picked");
                    break;
                case ItemType.Coin:
                    inventory.Coins += coinAmount;
                    Debug.Log("Coin Picked");
                    break;
                case ItemType.Key:
                    inventory.hasKey = true;
                    Debug.Log("Key Picked");
                    break;
            }

            Destroy(gameObject);
        }
    }
}