using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Dialogue
{
    // Gắn lên NPC. Player ở trong vùng trigger -> bấm F -> chạy hội thoại.
    // Dùng OnTriggerStay2D nên nhận diện được cả khi Player đứng sẵn trong vùng từ đầu.
    [RequireComponent(typeof(Collider2D))]
    public class DialogueTrigger : MonoBehaviour
    {
        [Header("Đoạn hội thoại của NPC này")]
        public DialogueData conversation;

        [Header("Phím tương tác")]
        public Key interactKey = Key.F;

        [Header("Tùy chọn hiện gợi ý (vd: 'Bấm F')")]
        public GameObject interactHint;

        [Header("Bật log để chẩn đoán (tắt khi xong)")]
        public bool debugLog = true;

        private bool _playerInRange;

        private void Awake()
        {
            var col = GetComponent<Collider2D>();
            col.isTrigger = true;
            if (interactHint != null) interactHint.SetActive(false);
        }

        private void Update()
        {
            if (!_playerInRange)
            {
                if (interactHint != null) interactHint.SetActive(false);
                return;
            }

            if (interactHint != null) interactHint.SetActive(true);

            if (DialogueManager.Instance == null)
            {
                if (debugLog) Debug.LogWarning("[DialogueTrigger] Khong tim thay DialogueManager trong scene!");
                return;
            }
            if (DialogueManager.Instance.IsRunning) return;

            var kb = Keyboard.current;
            if (kb == null) return;

            if (kb[interactKey].wasPressedThisFrame)
            {
                if (conversation == null)
                {
                    if (debugLog) Debug.LogWarning("[DialogueTrigger] Chua gan Conversation cho NPC nay!");
                    return;
                }
                if (debugLog) Debug.Log("[DialogueTrigger] Bat dau hoi thoai.");
                DialogueManager.Instance.StartDialogue(conversation);
            }
        }

        // Dùng Stay thay vì Enter: nhận diện được cả khi đã đứng sẵn trong vùng
        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            if (!_playerInRange && debugLog) Debug.Log("[DialogueTrigger] Player trong vung. Bam F de noi chuyen.");
            _playerInRange = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            _playerInRange = false;
        }
    }
}