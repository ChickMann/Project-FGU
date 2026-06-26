using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Dialogue
{
    // Gắn lên BẤT KỲ NPC nào để test.
    // Player vào vùng trigger -> bấm phím tương tác (mặc định F) -> chạy hội thoại.
    [RequireComponent(typeof(Collider2D))]
    public class DialogueTrigger : MonoBehaviour
    {
        [Header("Đoạn hội thoại của NPC này")]
        public DialogueData conversation;       // kéo asset Conversation vào

        [Header("Phím tương tác")]
        public Key interactKey = Key.F;

        [Header("Tùy chọn hiện gợi ý (vd: 'Bấm F')")]
        public GameObject interactHint;

        private bool _playerInRange;

        private void Awake()
        {
            var col = GetComponent<Collider2D>();
            col.isTrigger = true;
            if (interactHint != null) interactHint.SetActive(false);
        }

        private void Update()
        {
            if (!_playerInRange) return;
            if (DialogueManager.Instance != null && DialogueManager.Instance.IsRunning) return;

            if (Keyboard.current != null && Keyboard.current[interactKey].wasPressedThisFrame)
            {
                DialogueManager.Instance?.StartDialogue(conversation);
                if (interactHint != null) interactHint.SetActive(false);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            _playerInRange = true;
            if (interactHint != null) interactHint.SetActive(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            _playerInRange = false;
            if (interactHint != null) interactHint.SetActive(false);
        }
    }
}
