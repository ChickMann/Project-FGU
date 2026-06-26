using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Dialogue
{
    // 3 loại node hội thoại theo yêu cầu
    public enum NodeType
    {
        Line,    // 1) hội thoại bình thường - dẫn dắt cốt truyện
        Choice,  // 2) chọn 1 trong nhiều câu trả lời
        Shop     // 3) mở shop
    }

    [System.Serializable]
    public class DialogueChoice
    {
        [TextArea] public string text;     // nội dung lựa chọn hiện trên nút
        public string nextNodeId;          // nhảy tới node nào sau khi chọn (để trống = kết thúc)
    }

    [System.Serializable]
    public class ShopItem
    {
        public string itemName;
        public ItemType itemType;          // dùng lại enum sẵn có (HealthPotion, Coin, Key)
        public int price = 10;
        public int amount = 1;             // số lượng nhận khi mua
        public Sprite icon;                // optional
    }

    [System.Serializable]
    public class DialogueNode
    {
        public string id = "node_0";       // id duy nhất để liên kết
        public NodeType type = NodeType.Line;

        [Header("Line / Choice")]
        public string speakerName = "NPC";
        [TextArea(2, 5)] public string text;
        public Sprite portrait;            // optional, ảnh chân dung

        [Header("Next (chỉ dùng cho Line)")]
        public string nextNodeId;          // node tiếp theo; để trống = kết thúc

        [Header("Choices (chỉ dùng cho Choice)")]
        public List<DialogueChoice> choices = new List<DialogueChoice>();

        [Header("Shop (chỉ dùng cho Shop)")]
        public List<ShopItem> shopItems = new List<ShopItem>();
        public string afterShopNodeId;     // node chạy tiếp khi đóng shop (để trống = kết thúc)
    }

    // Một đoạn hội thoại = 1 asset, tạo qua menu Create > Dialogue > Conversation
    [CreateAssetMenu(menuName = "Dialogue/Conversation", fileName = "NewConversation")]
    public class DialogueData : ScriptableObject
    {
        public string startNodeId = "node_0";
        public List<DialogueNode> nodes = new List<DialogueNode>();

        public DialogueNode GetNode(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            foreach (var n in nodes)
                if (n.id == id) return n;
            return null;
        }

        public DialogueNode GetStartNode() => GetNode(startNodeId);
    }
}
