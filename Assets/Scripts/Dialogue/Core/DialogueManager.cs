using System;
using UnityEngine;

namespace Assets.Scripts.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        [Header("Tùy chọn: khóa input người chơi khi đang nói chuyện")]
        public MonoBehaviour player;   // kéo Player vào (có thể để trống) - chấp nhận mọi script

        [Header("Bật log chẩn đoán")]
        public bool debugLog = true;

        public event Action<DialogueNode> OnLineShown;
        public event Action<DialogueNode> OnChoiceShown;
        public event Action<DialogueNode> OnShopOpen;
        public event Action OnDialogueEnd;

        public bool IsRunning { get; private set; }

        private DialogueData _data;
        private DialogueNode _current;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void StartDialogue(DialogueData data)
        {
            if (debugLog) Debug.Log($"[DM] StartDialogue. data={(data == null ? "NULL" : data.name)}, IsRunning={IsRunning}");
            if (IsRunning || data == null) return;
            _data = data;
            IsRunning = true;
            if (player != null) player.enabled = false;

            var start = _data.GetStartNode();
            if (debugLog)
            {
                if (start == null)
                    Debug.LogError($"[DM] KHONG tim thay node bat dau! startNodeId='{_data.startNodeId}'. Kiem tra Id node co khop khong.");
                else
                    Debug.Log($"[DM] Node bat dau: id='{start.id}', type={start.type}. So nguoi nghe OnLineShown: {(OnLineShown == null ? 0 : OnLineShown.GetInvocationList().Length)}");
            }
            GoToNode(start);
        }

        private void GoToNode(DialogueNode node)
        {
            _current = node;
            if (node == null) { if (debugLog) Debug.Log("[DM] node=null -> ket thuc."); EndDialogue(); return; }

            if (debugLog) Debug.Log($"[DM] GoToNode id='{node.id}', type={node.type}");

            switch (node.type)
            {
                case NodeType.Line:
                    if (OnLineShown == null && debugLog) Debug.LogWarning("[DM] OnLineShown KHONG co nguoi nghe (DialogueUI chua dang ky)!");
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

        public void Advance()
        {
            if (!IsRunning || _current == null || _current.type != NodeType.Line) return;
            GoToNode(_data.GetNode(_current.nextNodeId));
        }

        public void SelectChoice(int index)
        {
            if (!IsRunning || _current == null || _current.type != NodeType.Choice) return;
            if (index < 0 || index >= _current.choices.Count) return;
            GoToNode(_data.GetNode(_current.choices[index].nextNodeId));
        }

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
            if (player != null) player.enabled = true;
            OnDialogueEnd?.Invoke();
        }
    }
}