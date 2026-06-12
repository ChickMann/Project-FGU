using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerInventory : MonoBehaviour
    {
        public int Coins = 0;
        public int potionCount = 0;
        public bool hasKey = false;
        public float potionCooldown = 5f;
        private float lastPotionTime;
        public bool CanUsePotion()
        {
            return Time.time >= lastPotionTime + potionCooldown;
        }

        public void UsePotion()
        {
            lastPotionTime = Time.time;
        }

        private PlayerHealth playerHealth;

        private void Start()
        {
            playerHealth = GetComponent<PlayerHealth>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H) )
            {
                HealPotion();
            }
        }

        void HealPotion()
        {
            if (potionCount <= 0)
                return;
            if(!CanUsePotion())
                return;

            potionCount--;

            playerHealth.Heal(20);

            UsePotion();

            Debug.Log("Potion Used");
        }
    }
}