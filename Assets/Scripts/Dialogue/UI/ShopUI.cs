using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

namespace Assets.Scripts.Dialogue
{
    // Xử lý LOẠI 3: shop. Mua hàng trừ Coins trong PlayerInventory.
    // Mỗi dòng item dùng prefab có component ShopItemRow (icon + tên + giá + nút mua).
    public class ShopUI : MonoBehaviour
    {
        [Header("Panel shop")]
        public GameObject shopPanel;
        public TMP_Text coinText;
        public Button closeButton;

        [Header("Danh sách item")]
        public ShopItemRow shopItemPrefab;  // prefab dòng item (có component ShopItemRow)
        public Transform itemContainer;

        [Header("Tham chiếu")]
        public PlayerInventory inventory;   // kéo Player vào, hoặc tự tìm theo tag

        private readonly List<GameObject> _spawned = new List<GameObject>();
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

        private bool _shopOpen;

        private void OpenShop(DialogueNode node)
        {
            _shopOpen = true;
            if (shopPanel != null) shopPanel.SetActive(true);
            ClearItems();
            RefreshCoins();

            foreach (var item in node.shopItems)
            {
                ShopItem captured = item;
                ShopItemRow row = Instantiate(shopItemPrefab, itemContainer);
                row.Setup(captured, () => TryBuy(captured));
                _spawned.Add(row.gameObject);
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
                case ItemType.ItemTag:
                    inventory.hasItem += item.amount;
                    break;
                case ItemType.Key:
                    inventory.hasKey = true;
                    break;
                case ItemType.Coin:
                    inventory.Coins += item.amount;
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
            _shopOpen = false;
            ClearItems();
            if (shopPanel != null) shopPanel.SetActive(false);
            if (_mgr != null) _mgr.CloseShop();
        }

        private void ClearItems()
        {
            foreach (var go in _spawned)
                if (go != null) Destroy(go);
            _spawned.Clear();
        }
    
        private void Update()
        {
            if (!_shopOpen) return;
            var kb = Keyboard.current;
            if (kb != null && kb.escapeKey.wasPressedThisFrame)
            {
                CloseShop();
            }
        }

}
}