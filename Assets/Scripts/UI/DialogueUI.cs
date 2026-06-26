using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Assets.Scripts.Dialogue
{
    // Gắn vào 1 Canvas. Tự đăng ký nghe DialogueManager.
    // Cần bạn kéo các tham chiếu UI trong Inspector (mô tả ở dưới cùng file).
    public class DialogueUI : MonoBehaviour
    {
        [Header("Panel chung")]
        public GameObject root;                 // panel cha, bật/tắt toàn bộ

        [Header("Khung thoại (Line + Choice)")]
        public GameObject linePanel;
        public TMP_Text speakerText;
        public TMP_Text bodyText;
        public Image portraitImage;
        public Button continueButton;           // nút "tiếp" cho Line

        [Header("Lựa chọn (Choice)")]
        public GameObject choicePanel;          // chứa các nút lựa chọn
        public Button choiceButtonPrefab;       // prefab 1 nút (có TMP_Text con)
        public Transform choiceContainer;       // nơi spawn nút

        [Header("Hiệu ứng đánh chữ")]
        public bool useTypewriter = true;
        public float charsPerSecond = 40f;

        private readonly List<Button> _spawnedChoices = new List<Button>();
        private DialogueManager _mgr;
        private string _fullText;
        private bool _typing;
        private float _typeTimer;
        private int _visibleChars;

        private void Start()
        {
            _mgr = DialogueManager.Instance;
            if (_mgr == null) { Debug.LogError("Thiếu DialogueManager trong scene."); return; }

            _mgr.OnLineShown += ShowLine;
            _mgr.OnChoiceShown += ShowChoices;
            _mgr.OnShopOpen += HideAllForShop;
            _mgr.OnDialogueEnd += HideAll;

            if (continueButton != null)
                continueButton.onClick.AddListener(OnContinuePressed);

            if (root != null) root.SetActive(false);
        }

        private void OnDestroy()
        {
            if (_mgr == null) return;
            _mgr.OnLineShown -= ShowLine;
            _mgr.OnChoiceShown -= ShowChoices;
            _mgr.OnShopOpen -= HideAllForShop;
            _mgr.OnDialogueEnd -= HideAll;
        }

        // ===== LOẠI 1: Line =====
        private void ShowLine(DialogueNode node)
        {
            if (root != null) root.SetActive(true);
            linePanel.SetActive(true);
            choicePanel.SetActive(false);

            if (speakerText != null) speakerText.text = node.speakerName;
            if (portraitImage != null)
            {
                portraitImage.gameObject.SetActive(node.portrait != null);
                portraitImage.sprite = node.portrait;
            }
            if (continueButton != null) continueButton.gameObject.SetActive(true);

            SetText(node.text);
        }

        // ===== LOẠI 2: Choice =====
        private void ShowChoices(DialogueNode node)
        {
            if (root != null) root.SetActive(true);
            linePanel.SetActive(true);
            choicePanel.SetActive(true);

            if (speakerText != null) speakerText.text = node.speakerName;
            if (portraitImage != null)
            {
                portraitImage.gameObject.SetActive(node.portrait != null);
                portraitImage.sprite = node.portrait;
            }
            // Choice không cần nút "tiếp"
            if (continueButton != null) continueButton.gameObject.SetActive(false);

            SetText(node.text);
            ClearChoices();

            for (int i = 0; i < node.choices.Count; i++)
            {
                int captured = i;
                Button b = Instantiate(choiceButtonPrefab, choiceContainer);
                var label = b.GetComponentInChildren<TMP_Text>();
                if (label != null) label.text = node.choices[i].text;
                b.onClick.AddListener(() => _mgr.SelectChoice(captured));
                _spawnedChoices.Add(b);
            }
        }

        // ===== LOẠI 3: Shop -> ẩn khung thoại, ShopUI tự xử lý =====
        private void HideAllForShop(DialogueNode node)
        {
            if (linePanel != null) linePanel.SetActive(false);
            if (choicePanel != null) choicePanel.SetActive(false);
            // root vẫn bật để ShopUI dùng chung canvas nếu muốn; tùy bạn
        }

        private void HideAll()
        {
            ClearChoices();
            if (root != null) root.SetActive(false);
        }

        private void OnContinuePressed()
        {
            // Nếu đang đánh chữ thì bấm lần 1 để hiện hết, lần 2 mới qua node
            if (_typing) { FinishTyping(); return; }
            _mgr.Advance();
        }

        private void ClearChoices()
        {
            foreach (var b in _spawnedChoices)
                if (b != null) Destroy(b.gameObject);
            _spawnedChoices.Clear();
        }

        // ===== Typewriter =====
        private void SetText(string text)
        {
            _fullText = text;
            if (!useTypewriter || bodyText == null)
            {
                if (bodyText != null) bodyText.text = text;
                _typing = false;
                return;
            }
            bodyText.text = text;
            bodyText.maxVisibleCharacters = 0;
            _visibleChars = 0;
            _typeTimer = 0f;
            _typing = true;
        }

        private void FinishTyping()
        {
            _typing = false;
            if (bodyText != null) bodyText.maxVisibleCharacters = _fullText.Length;
        }

        private void Update()
        {
            if (!_typing || bodyText == null) return;
            _typeTimer += Time.unscaledDeltaTime * charsPerSecond;
            int target = Mathf.Min(_fullText.Length, Mathf.FloorToInt(_typeTimer));
            if (target != _visibleChars)
            {
                _visibleChars = target;
                bodyText.maxVisibleCharacters = _visibleChars;
            }
            if (_visibleChars >= _fullText.Length) _typing = false;
        }
    }
}
