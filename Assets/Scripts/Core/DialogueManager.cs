using System;
using UnityEngine;

namespace Assets.Scripts.Dialogue
{
    // Bộ điều khiển trung tâm: nhận DialogueData và chạy lần lượt từng node.
    // UI chỉ cần lắng nghe các event dưới đây, không cần biết logic.
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        [Header("Tùy chọn: khóa input người chơi khi đang nói chuyện")]
        public PlayerController player;          // kéo Player vào (có thể để trống)

        // ===== Events cho UI / Shop lắng nghe =====
        public event Action<DialogueNode> OnLineShown;      // hiện 1 câu thoại
        public event Action<DialogueNode> OnChoiceShown;    // hiện danh sách lựa chọn
        public event Action<DialogueNode> OnShopOpen;       // mở shop
        public event Action OnDialogueEnd;                  // kết thúc hội thoại

        public bool IsRunning { get; private set; }

        private DialogueData _data;
        private DialogueNode _current;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        // Gọi từ NPC để bắt đầu
        public void StartDialogue(DialogueData data)
        {
            if (IsRunning || data == null) return;
            _data = data;
            IsRunning = true;
            if (player != null) player.enabled = false;  // tạm khóa di chuyển
            GoToNode(_data.GetStartNode());
        }

        private void GoToNode(DialogueNode node)
        {
            _current = node;
            if (node == null) { EndDialogue(); return; }

            switch (node.type)
            {
                case NodeType.Line:
                    OnLineShown?.Invoke(node);
                    break;
                case NodeType.Choice:
                    OnChoiceShown?.Invoke(node);
                    break;
                case NodeType.Shop:
                    OnShopOpen?.Invoke(node);
                    break;
            }
        }

        // UI gọi khi người chơi bấm "tiếp" trên 1 câu Line
        public void Advance()
        {
            if (!IsRunning || _current == null || _current.type != NodeType.Line) return;
            GoToNode(_data.GetNode(_current.nextNodeId));
        }

        // UI gọi khi người chơi chọn 1 đáp án (Choice)
        public void SelectChoice(int index)
        {
            if (!IsRunning || _current == null || _current.type != NodeType.Choice) return;
            if (index < 0 || index >= _current.choices.Count) return;
            GoToNode(_data.GetNode(_current.choices[index].nextNodeId));
        }

        // Shop UI gọi khi đóng shop
        public void CloseShop()
        {
            if (!IsRunning || _current == null || _current.type != NodeType.Shop) return;
            GoToNode(_data.GetNode(_current.afterShopNodeId));
        }

        public void EndDialogue()
        {
            IsRunning = false;
            _current = null;
            _data = null;
            if (player != null) player.enabled = true;   // mở lại di chuyển
            OnDialogueEnd?.Invoke();
        }
    }
}
