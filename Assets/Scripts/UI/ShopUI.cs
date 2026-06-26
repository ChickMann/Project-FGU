using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Assets.Scripts.Dialogue
{
    // Xử lý LOẠI 3: shop. Mua hàng trừ Coins trong PlayerInventory.
    public class ShopUI : MonoBehaviour
    {
        [Header("Panel shop")]
        public GameObject shopPanel;
        public TMP_Text coinText;
        public Button closeButton;

        [Header("Danh sách item")]
        public Button shopItemPrefab;       // prefab 1 dòng item (TMP_Text con để hiện tên/giá)
        public Transform itemContainer;

        [Header("Tham chiếu")]
        public PlayerInventory inventory;   // kéo Player vào, hoặc tự tìm theo tag

        private readonly List<Button> _spawned = new List<Button>();
        private DialogueManager _mgr;

        private void Start()
        {
            _mgr = DialogueManager.Instance;
            if (_mgr != null) _mgr.OnShopOpen += OpenShop;

            if (closeButton != null) closeButton.onClick.AddListener(CloseShop);
            if (shopPanel != null) shopPanel.SetActive(false);

            if (inventory == null)
            {
                var p = GameObject.FindGameObjectWithTag("Player");
                if (p != null) inventory = p.GetComponent<PlayerInventory>();
            }
        }

        private void OnDestroy()
        {
            if (_mgr != null) _mgr.OnShopOpen -= OpenShop;
        }

        private void OpenShop(DialogueNode node)
        {
            if (shopPanel != null) shopPanel.SetActive(true);
            ClearItems();
            RefreshCoins();

            foreach (var item in node.shopItems)
            {
                ShopItem captured = item;
                Button b = Instantiate(shopItemPrefab, itemContainer);
                var label = b.GetComponentInChildren<TMP_Text>();
                if (label != null) label.text = $"{item.itemName} - {item.price} xu";
                b.onClick.AddListener(() => TryBuy(captured));
                _spawned.Add(b);
            }
        }

        private void TryBuy(ShopItem item)
        {
            if (inventory == null) return;
            if (inventory.Coins < item.price)
            {
                Debug.Log("Không đủ xu!");
                return;
            }

            inventory.Coins -= item.price;

            switch (item.itemType)
            {
                case ItemType.HealthPotion:
                    inventory.potionCount += item.amount;
                    break;
                case ItemType.Key:
                    inventory.hasKey = true;
                    break;
                case ItemType.Coin:
                    inventory.Coins += item.amount; // hiếm khi dùng, để đủ case
                    break;
            }

            Debug.Log($"Đã mua {item.itemName}");
            RefreshCoins();
        }

        private void RefreshCoins()
        {
            if (coinText != null && inventory != null)
                coinText.text = $"Xu: {inventory.Coins}";
        }

        private void CloseShop()
        {
            ClearItems();
            if (shopPanel != null) shopPanel.SetActive(false);
            if (_mgr != null) _mgr.CloseShop();   // trả quyền điều khiển về DialogueManager
        }

        private void ClearItems()
        {
            foreach (var b in _spawned)
                if (b != null) Destroy(b.gameObject);
            _spawned.Clear();
        }
    }
}
